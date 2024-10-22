using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public enum CharacterState
{
    None,
    Default,
    Anchored,
    Length,
}
public class Sc_CharacterController : MonoBehaviour
{
    [Header("STATE")]
    public CharacterState _defaultState = CharacterState.Default;
    private CharacterState _currentState = CharacterState.None;
    public CharacterState CurrentState { get { return _currentState; } set { TransitionToState(value); } }

    [Header("MOVEMENT")]
    public float _maxGroundedMoveSpeed = 7f;
    public float _groundedMovementSharpness = 15f;
    public float _rotationSharpness = 10f;
    public float _maxAirMoveSpeed = 7f;
    public float _airMovementSharpness = 15f;
    public float _airDrag = 0f;
    public Vector3 _gravity = new Vector3(0f, -30f, 0f);
    private bool _canMove = true;
    private bool _canRotate = true;
    public bool CanMove { get { return _canMove; } set { _canMove = value; } }
    public bool CanRotate { get { return _canRotate; } set { _canRotate = value; } }
    private Vector3 _forcedLookAtDir = Vector3.zero;

    [Header("PARTICLES")]
    public ParticleSystem _runParticles;

    [Header("WEIGHT")]
    public float _weight = 10f;

    [Header("GROUNDCHECK")]
    public Transform _groundCheckEmissionPoint;
    public LayerMask _groundLayers;
    public float _maxVerticalBalancingForce = 10f;
    public float _groundCheckDistance = 1f;
    private bool _isGrounded = false;
    public bool IsGrounded { get { return _isGrounded; } }
    private RaycastHit _groundHit;
    private Rigidbody _walkingOnRb;

    [Header("ANCHORING")]
    private bool _isAnchoring = false;
    public bool IsAnchoring { get { return _anchorCo != null ? true : false; } }
    private Vector3 _anchorFromPoint;
    private Quaternion _anchorFromRot;
    private Vector3 _anchorToPoint;
    private Quaternion _anchorToRot;
    private Coroutine _anchorCo;
    private Action _anchorEndAction;
    private Transform _anchor;

    [Header("PUSHING")]
    public float _pushCheckDistance = 1f;
    public float _characterHeight = 1.4f;
    public float _characterRadius = .5f;
    public float _pushRequestTime = .5f;
    public LayerMask _pushableObjectLayers;
    private RaycastHit _pushableHit;
    private float _pushRequestTimer = 0f;
    private bool _pushRequested = false;
    private Sc_Pushable _currentPushable;
    private bool _isPushingBlock = false;
    public bool IsPushingBlock { get { return _isPushingBlock; } }
    private Vector3 _pushDirection;

    [Header("CLIMBING")]
    public float _maxClimbSpeed = 5f;
    public float _climbMovementSharpness = 15f;
    private bool _isClimbing = false;
    public bool IsClimbing { get { return _isClimbing; } }
    private Vector2 _climbInputVector;
    private Sc_Ladder _currentLadder;
    private Vector3 _topOfLadder;
    private Vector3 _bottomOfLadder;

    [Header("VALVE")]
    private Sc_Valve_Floor _currentValve;
    public Sc_Valve_Floor CurrentValve { get { return _currentValve; } }
    public bool IsAnchoredToValve { get { return _currentValve != null ? true : false; } }

    private Rigidbody _rb;
    public Rigidbody RB { get { return _rb; } }
    public bool CanBeRepelled { get { return (IsAnchoring || IsClimbing || IsPushingBlock) ? false : true; } }
    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;
    private bool _ignoreInputs = false;
    public bool IgnoreInputs{
        get { return _ignoreInputs; }
        set { _ignoreInputs = value; if (value == true) ResetInputs(); } }

    public delegate void DefaultEvent();
    public event DefaultEvent PushStart;
    public event DefaultEvent PushEnd;
    public delegate void RBEvent(Rigidbody rb);
    public event RBEvent OnGroundedMovement;
    public event RBEvent OnAerialMovement;
    public event RBEvent OnLanded;

    private void Start()
    {
        InitializeController();
    }

    private void InitializeController()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogWarning(this.name + " doesn't have a Rigidbody!");
            return;
        }

        if (_groundCheckEmissionPoint == null)
        {
            Debug.LogWarning(this.name + " doesn't have a groundCheckEmissionPoint!");
            return;
        }

        CurrentState = _defaultState;
    }

    public void Update()
    {
        HandlePushRequestTimer();
        if (_runParticles != null)
        {
            HandleParticles();
        }      
    }

    private void FixedUpdate()
    {
        //_isGrounded = GroundCheck(!_isGrounded);
        _isGrounded = GroundCheck();
        if (_isGrounded && _currentState == CharacterState.Default) HandleBalancingVerticalForce();
        //If add physics based floors
        //if (_walkingOnRb) HandleWalkingOnRB();

        HandleRotation();
        HandleVelocity();

        CheckRequestPush();
    }

    public void SetInputs(ref CharacterInput input)
    {
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(input.moveX, 0f, input.moveY), 1f);
        if (_ignoreInputs) moveInputVector = Vector3.zero;
        Vector2 climbInputVector = Vector2.ClampMagnitude(new Vector2(input.moveX, input.moveY), 1f);
        if (_ignoreInputs) climbInputVector = Vector3.zero;

        float cameraRotation = input.cameraRef.transform.eulerAngles.y;
        Quaternion controlRotation = Quaternion.Euler(0, cameraRotation, 0);

        _moveInputVector = controlRotation * moveInputVector;
        if (_forcedLookAtDir != Vector3.zero)
        {
            _lookInputVector = _forcedLookAtDir;
        }
        else
        {
            _lookInputVector = _moveInputVector.normalized;
        }
        
        _climbInputVector = climbInputVector;

        if (CurrentValve != null)
        {
            CurrentValve.SetInput(_moveInputVector);
        }
    }

    #region AI
    public void SetAIInputs(ref AIInput input)
    {
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(input.moveX, 0f, input.moveY), 1f);
        if (_ignoreInputs) moveInputVector = Vector3.zero;
        Vector2 climbInputVector = Vector2.ClampMagnitude(new Vector2(input.moveX, input.moveY), 1f);
        if (_ignoreInputs) climbInputVector = Vector3.zero;

        _moveInputVector = moveInputVector;

        if (_forcedLookAtDir != Vector3.zero)
        {
            _lookInputVector = _forcedLookAtDir;
        }
        else
        {
            _lookInputVector = _moveInputVector.normalized;
        }

        _climbInputVector = climbInputVector;
    }
    #endregion

    #region TIMERS
    private void HandlePushRequestTimer()
    {
        if (_pushRequested)
        {
            if (_pushRequestTimer >= _pushRequestTime)
            {
                StartPush();
            }
            else
            {
                _pushRequestTimer += Time.deltaTime;
            }            
        }
        else
        {
            _pushRequestTimer = 0f;
        }      
    }

    #endregion

    #region ON EVENT
    private void OnLand(float landingForce)
    {
        //if (_walkingOnRb)
        //{
        //    _walkingOnRb.AddForceAtPosition(new Vector3(0f, landingForce, 0f), _groundHit.point, ForceMode.Impulse);
        //}
    }
    #endregion

    #region PHYSICS
    private void HandleBalancingVerticalForce()
    {
        if (_isClimbing) return;

        float distanceToGround = _groundHit.distance;
        float alpha = distanceToGround / _groundCheckDistance;
        float strength = Mathf.Lerp(_maxVerticalBalancingForce, -_maxVerticalBalancingForce, alpha);
        Vector3 balancingForce = new Vector3(0f, strength, 0f);
        _rb.AddForce(balancingForce, ForceMode.Force);
    }

    private void HandleWalkingOnRB()
    {
        _walkingOnRb.AddForceAtPosition(new Vector3(0f, -_weight, 0f), _groundHit.point, ForceMode.Force);
        Debug.Log(_weight);
    }

    private void HandleRotation()
    {
        switch (_currentState)
        {
            case CharacterState.None:
                break;

            case CharacterState.Default:

                if (!_isClimbing)
                {
                    if (_lookInputVector.sqrMagnitude > 0f && _rotationSharpness > 0f)
                    {
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(transform.forward, _lookInputVector, 1 - Mathf.Exp(-_rotationSharpness * Time.fixedDeltaTime)).normalized;
                        if (_isPushingBlock)
                        {
                            smoothedLookInputDirection = Vector3.Slerp(transform.forward, _pushDirection, 1 - Mathf.Exp(-_rotationSharpness * Time.fixedDeltaTime)).normalized;
                        }

                        if (!_canRotate) break;
                        transform.forward = smoothedLookInputDirection;
                    }
                }
                else
                {
                    if (_lookInputVector.sqrMagnitude > 0f && _rotationSharpness > 0f)
                    {
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(transform.forward, transform.forward, 1 - Mathf.Exp(-_rotationSharpness * Time.fixedDeltaTime)).normalized;

                        if (!_canRotate) break;
                        transform.forward = smoothedLookInputDirection;
                    }
                }

                break;

            case CharacterState.Anchored:
                if (!_canRotate) break;
                _rb.rotation = _anchor.rotation;
                break;

        }

    }

    private void HandleVelocity()
    {
        switch (_currentState)
        {
            case CharacterState.None:
                break;

            case CharacterState.Default:
                if (!_isClimbing)
                {
                    if (IsGrounded)
                    {
                        Vector3 groundNormal = _groundHit.normal;
                        Vector3 inputRight = Vector3.Cross(_moveInputVector, Vector3.up);
                        Vector3 reorientedInput = Vector3.Cross(groundNormal, inputRight).normalized * _moveInputVector.magnitude;

                        Vector3 targetMovementVelocity = reorientedInput * _maxGroundedMoveSpeed;
                        if (!_canMove) targetMovementVelocity = Vector3.zero;
                        _rb.velocity = Vector3.Lerp(_rb.velocity, targetMovementVelocity, 1f - Mathf.Exp(-_groundedMovementSharpness * Time.fixedDeltaTime));

                        OnGroundedMovement?.Invoke(_rb);
                    }
                    else
                    {
                        if (_moveInputVector.sqrMagnitude > 0f)
                        {
                            Vector3 addedVelocity = _moveInputVector * _airMovementSharpness * Time.fixedDeltaTime;
                            Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(_rb.velocity, Vector3.up);
                            if (currentVelocityOnInputsPlane.magnitude < _maxAirMoveSpeed)
                            {
                                Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, _maxAirMoveSpeed);
                                addedVelocity = newTotal - currentVelocityOnInputsPlane;
                            }
                            else
                            {
                                if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                                {
                                    addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                                }
                            }
                            if (!_canMove) addedVelocity = Vector3.zero;
                            _rb.velocity += addedVelocity;
                        }
                        _rb.velocity += _gravity * Time.fixedDeltaTime;

                        _rb.velocity *= (1f / (1f + (_airDrag * Time.fixedDeltaTime)));

                        OnAerialMovement?.Invoke(_rb);
                    }
                }
                else
                {
                    Vector3 targetClimbVelocity = new Vector3(0f, _maxClimbSpeed * _climbInputVector.y, 0f);
                    if (!_canMove) targetClimbVelocity = Vector3.zero;
                    _rb.velocity = Vector3.Lerp(_rb.velocity, targetClimbVelocity, 1f - Mathf.Exp(-_climbMovementSharpness * Time.fixedDeltaTime));

                    if (transform.position.y >= _topOfLadder.y)
                    {
                        EndClimbSequence(_currentLadder, true);
                    }
                    else if (transform.position.y <= _bottomOfLadder.y)
                    {
                        EndClimbSequence(_currentLadder, false);
                    }
                }
                break;

            case CharacterState.Anchored:
                _rb.MovePosition(_anchor.position);
                break;
        }    
    }

    private bool GroundCheck()
    {
        bool localIsGrounded = Physics.SphereCast(_groundCheckEmissionPoint.transform.position, .1f, Vector3.down, out _groundHit, _groundCheckDistance, _groundLayers, QueryTriggerInteraction.Ignore);
        //If add physics based floors
        //if (localIsGrounded)
        //{
        //    _walkingOnRb = _groundHit.collider.GetComponent<Rigidbody>();
        //    Debug.Log(_walkingOnRb);
        //}

        //bool localIsGrounded = Physics.SphereCast(transform.position + (Vector3.up * .1f), .1f, Vector3.down, out _groundHit, _groundCheckDistance + .1f, _groundLayers);

        //if (snapToGround && localIsGrounded)
        //{
        //    _rb.Move(_groundHit.point, _rb.rotation);
        //}

        if (localIsGrounded && !_isGrounded)
        {
            OnLand(_rb.velocity.y);

            OnLanded?.Invoke(_rb);
        }

        return localIsGrounded;
    }
    #endregion

    #region ANCHORING

    public void AnchorTo(Vector3 toAnchorPoint, Quaternion toAnchorRot, float overTime = 1f, Action onAnchorEnd = null)
    {
        StopAnchoringSequence();

        _anchorEndAction = onAnchorEnd;

        _anchorFromPoint = transform.position;
        _anchorFromRot = transform.rotation;
        _anchorToPoint = toAnchorPoint;
        _anchorToRot = toAnchorRot;

        IgnoreInputs = true;

        _anchorCo = StartCoroutine(AnchorToCo(overTime));
    }

    private IEnumerator AnchorToCo(float overTime)
    {
        float time = 0f;
        _rb.velocity = Vector3.zero;
        while (time < overTime)
        {
            float alpha = time / overTime;
            Vector3 toPoint = Vector3.Slerp(_anchorFromPoint, _anchorToPoint, alpha);
            Quaternion toRot = Quaternion.Slerp(_anchorFromRot, _anchorToRot, alpha);
            _rb.Move(toPoint, toRot);
            time += Time.deltaTime;
            yield return null;
        }
        _rb.Move(_anchorToPoint, _anchorToRot);
        _rb.velocity = Vector3.zero;
        _anchorCo = null;

        IgnoreInputs = false;

        if (_anchorEndAction != null)
        {
            _anchorEndAction();
            _anchorEndAction = null;
        }
    }

    public void StopAnchoringSequence()
    {
        if (_anchorCo != null)
        {
            StopCoroutine(_anchorCo);
        }
    }

    public void SetAnchor(Transform anchor)
    {
        _anchor = anchor;
        CurrentState = CharacterState.Anchored;
    }

    public void ResetAnchor(CharacterState goToState = CharacterState.Default)
    {
        CurrentState = goToState;
        _anchor = null;
    }

    #endregion

    #region PUSHING
    private void CheckRequestPush()
    {
        if (_isClimbing || !_isGrounded) return;

        if (Physics.Raycast(transform.position + new Vector3(0, (_characterHeight * .5f), 0f), transform.forward, out _pushableHit, _pushCheckDistance, _pushableObjectLayers))
        {
            Sc_Pushable newPushable = _pushableHit.collider.GetComponent<Sc_Pushable>();

            if (newPushable)
            {
                if (newPushable != _currentPushable)
                {
                    EndPush();
                }
            }

            _currentPushable = newPushable;

            if (_currentPushable)
            {
                if (_currentPushable.IsBeingPushed)
                {
                    if (_currentPushable.PushedBy != this) return;
                }

                if (_currentPushable.IsSliding) return;

                float dot = Vector3.Dot(_pushableHit.normal, _moveInputVector);
                if (-dot > .8f)
                {
                    _pushRequested = true;
                    _pushDirection = -_pushableHit.normal;

                    if (_isPushingBlock)
                    {
                        _currentPushable.Push(_pushDirection);
                    }
                }
                else
                {
                    EndPush();
                }
            }
        }
        else
        {
            EndPush();
        }
    }

    private void StartPush()
    {
        if (_isPushingBlock) return;
        Debug.Log("PUSHING");
        _isPushingBlock = true;

        if (_currentPushable)
        {
            _currentPushable.IsBeingPushed = true;
            _currentPushable.PushedBy = this;

            //_currentPushable.RB.constraints = RigidbodyConstraints.FreezeRotation;
        }       
    }

    private void EndPush()
    {
        if (!_isPushingBlock) return;
        Debug.Log("PUSH ENDED");
        _isPushingBlock = false;
        _pushRequested = false;

        if (_currentPushable)
        {
            _currentPushable.IsBeingPushed = false;
            _currentPushable.PushedBy = null;

            //_currentPushable.RB.constraints = RigidbodyConstraints.None;
            //_currentPushable.RB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void PushPushable()
    {

    }
    #endregion

    #region CLIMBING

    public void InitiateClimbSequence(Sc_Ladder ladder)
    {
        _currentLadder = ladder;
        Vector3 startPoint = _currentLadder._ladderTop ? _currentLadder._endPoint.position : _currentLadder._startPoint.position;
        Quaternion startRot = _currentLadder._startPoint.rotation;
        _topOfLadder = _currentLadder._endPoint.position + (Vector3.up * .1f);
        _bottomOfLadder = _currentLadder._startPoint.position - (Vector3.up * .1f);

        AnchorTo(startPoint, startRot, .5f, () => StartClimbing());
    }

    public void EndClimbSequence(Sc_Ladder ladder, bool top)
    {
        StopClimbing();
        Vector3 toPoint = top ? _currentLadder._topAnchor.position : _currentLadder._footAnchor.position;
        Quaternion toRot = top ? _currentLadder._topAnchor.rotation : _currentLadder._footAnchor.rotation;

        AnchorTo(toPoint, toRot, .5f);
    }

    public void StartClimbing()
    {
        _rb.velocity = Vector3.zero;
        //_rb.isKinematic = true;
        _isClimbing = true;
    }

    public void StopClimbing()
    {
        //_rb.isKinematic = false;
        _isClimbing = false;
    }

    #endregion

    #region VALVES

    public void ConnectToValve(Sc_Valve_Floor valve)
    {
        if (valve == null) return;
        _currentValve = valve;

        AnchorTo(valve._anchor.position, valve._anchor.rotation, .5f,
            () => SetAnchor(valve._anchor));
    }

    public void DisconnectFromCurrentValve()
    {
        if (_currentValve == null) return;

        _currentValve = null;
        ResetAnchor();
    }

    #endregion

    #region UTILITY
    private void TransitionToState(CharacterState toState)
    {
        if (_currentState == toState) return;
        OnStateExit(_currentState);
        _currentState = toState;
        OnStateEnter(toState);
    }

    private void OnStateExit(CharacterState fromState)
    {
        switch (fromState)
        {
            case CharacterState.None:
                break;
            case CharacterState.Default:
                break;
            case CharacterState.Anchored:
                _rb.isKinematic = false;
                break;
        }
    }

    private void OnStateEnter(CharacterState toState)
    {
        switch (toState)
        {
            case CharacterState.None:
                break;
            case CharacterState.Default:
                break;
            case CharacterState.Anchored:
                if (_anchor == null)
                {
                    Debug.LogWarning(gameObject.name + " doesn't have an anchor, returning to default character state!");
                    CurrentState = CharacterState.Default;
                    return;
                }
                _rb.isKinematic = true;
                break;
        }
    }
    public void ResetInputs()
    {
        _moveInputVector = Vector3.zero;
    }

    public void LookAt(Vector3 point)
    {
        //Doesn't take into account Y values.
        Vector3 selfIgnoreY = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targetIgnoreY = new Vector3(point.x, 0f, point.z);
        Vector3 direction = (targetIgnoreY - selfIgnoreY).normalized;
        LookInDirection(direction);
    }

    public void LookInDirection(Vector3 direction)
    {
        Vector3 toDir = direction.normalized;
        _forcedLookAtDir = toDir;
    }

    public void StopLookAt()
    {
        _forcedLookAtDir = Vector3.zero;
    }
    #endregion

    #region AESTHETICS
    private void HandleParticles()
    {       
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if (Mathf.Abs(horizontalVelocity.magnitude) >= 2f && IsGrounded && !_isPushingBlock && _moveInputVector.magnitude > 0f)
        {
            if (!_runParticles.isPlaying)
            {
                _runParticles.Play();
            }          
        }
        else
        {
            _runParticles.Stop();
        }
    }

    public void ParentToObject(Transform parent)
    {
        if (this.transform.parent == parent) return;
        this.transform.parent = parent;
    }

    #endregion
}
