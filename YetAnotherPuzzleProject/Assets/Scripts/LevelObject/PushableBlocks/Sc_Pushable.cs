using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Pushable : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Activateable _activateable;
    public CinemachineImpulseSource ImpulseSource;
    public ParticleSystem Dust;

    [Header("MOVEMENT")]
    public float _maxSpeed = 1f;
    public float _speedSharpness = 15f;
    public Vector3 _gravity = new Vector3(0f, -30f, 0f);
    protected Vector3 _pushedDirection = Vector3.zero;
    [ReadOnly] public Vector3 InheritedVelocity = Vector3.zero;
    [ReadOnly] public float InheritedYaw = 0f;

    [Header("GROUND")]
    public LayerMask _groundLayers;
    [SerializeField] protected bool _isGrounded = true;
    protected RaycastHit _groundHit;
    protected RaycastHit _obstacleHit;

    protected Sc_CharacterController _pushedBy;
    public Sc_CharacterController PushedBy { get { return _pushedBy; } set { _pushedBy = value; } }

    protected BoxCollider _collider;
    public Collider Collider { get { return _collider; } }
    protected Rigidbody _rb;
    public Rigidbody RB { get { return _rb; } }
    protected Vector3 _boxColliderCenter;
    protected Vector3 _boxColliderHalfExtents;

    protected bool _onSlope;
    protected Vector3 _cachedLastPushDirection;
    protected bool _isBeingPushed;
    public bool IsBeingPushed { get { return _isBeingPushed; } set { _isBeingPushed = value; } }
    protected bool _isSliding;
    public bool IsSliding { get { return _isSliding; } }

    private void Start()
    {
        InitializePushable();
    }

    protected virtual void InitializePushable()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogWarning(this.name + " doesn't have a Rigidbody!");
            return;
        }

        _collider = GetComponent<BoxCollider>();
        if (_collider == null)
        {
            Debug.LogWarning(this.name + " doesn't have a BoxCollider!");
            return;
        }

        _boxColliderCenter = _collider.center;
        _boxColliderHalfExtents = new Vector3(_collider.size.x*.5f, _collider.size.y*.5f, _collider.size.z*.5f);
    }

    private void FixedUpdate()
    {
        _isGrounded = Grounded();
        HandleVelocity();
        HandleRotation();
    }

    private bool Grounded()
    {
        bool localIsGrounded = Physics.Raycast(transform.position + (Vector3.up * .2f), Vector3.down, out _groundHit, .5f, _groundLayers, QueryTriggerInteraction.Ignore);
        if (localIsGrounded && !_isGrounded)
        {
            OnLand();
        }
        return localIsGrounded;
    }

    private void OnLand()
    {
        GroundShake();
    }

    private void GroundShake()
    {
        if (Sc_CameraManager.instance != null)
        {
            Sc_CameraManager.instance.CameraShake(ImpulseSource, .1f);
        }
        if (Dust)
        {
            Dust.Play();
        }
    }

    private void HandleRotation()
    {
        Vector3 smoothedLookInputDirection = Vector3.Slerp(transform.forward, transform.forward, 1 - Mathf.Exp(-10f * Time.fixedDeltaTime)).normalized;
        smoothedLookInputDirection = Quaternion.Euler(0f, InheritedYaw, 0f) * smoothedLookInputDirection;

        transform.forward = smoothedLookInputDirection;
        InheritedYaw = 0f;
    }

    private void HandleVelocity()
    {
        Vector3 targetMovementVelocity = Vector3.zero;
        if (_pushedDirection != Vector3.zero)
        {
            targetMovementVelocity = _pushedDirection * _maxSpeed;
            _pushedDirection = Vector3.zero;
        }
        //targetMovementVelocity += _gravity * Time.fixedDeltaTime;
        float rbYVelocity = _rb.velocity.y + (_gravity.y * Time.fixedDeltaTime);
        targetMovementVelocity = new Vector3(targetMovementVelocity.x, rbYVelocity, targetMovementVelocity.z);
        targetMovementVelocity = targetMovementVelocity + new Vector3(InheritedVelocity.x, 0f, InheritedVelocity.z);
        InheritedVelocity = Vector3.zero;
        _rb.velocity = Vector3.Lerp(_rb.velocity, targetMovementVelocity, 1f - Mathf.Exp(-_speedSharpness * Time.fixedDeltaTime));
        //_rb.velocity += _gravity * Time.fixedDeltaTime;
    }

    public virtual void Push(Vector3 direction)
    {
        if (CheckObstacle(direction) || _onSlope) return;
        _pushedDirection = direction;
        //Vector3 targetMovementVelocity = direction * _maxSpeed;
        //_rb.velocity = Vector3.Lerp(_rb.velocity, targetMovementVelocity, 1f - Mathf.Exp(-_speedSharpness * Time.fixedDeltaTime));
    }

    protected virtual bool CheckObstacle(Vector3 direction)
    {
        Physics.BoxCast(transform.position + _boxColliderCenter, new Vector3(_boxColliderHalfExtents.x - .1f, _boxColliderHalfExtents.y - .1f, _boxColliderHalfExtents.z - .1f), direction, out _obstacleHit, transform.rotation, .3f, _groundLayers, QueryTriggerInteraction.Ignore);
        float angle = Vector3.Angle(Vector3.up, _obstacleHit.normal);
        return Mathf.Abs(angle) <= 1 || Mathf.Abs(angle) >= 80 ? false : true;
    }

    //private bool SlopeCheck()
    //{
    //    Physics.Raycast(transform.position + _boxColliderCenter, Vector3.down, out _slopeHit, 2f, _groundLayers, QueryTriggerInteraction.Ignore);
    //    float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
    //    return Mathf.Abs(angle) > 1 ? true : false;
    //}

    //private void RegisterObject(Sc_WeightedObject wObject)
    //{
    //    _registeredObjects.Add(wObject);
    //    weightedObject._weight += wObject._weight;
    //}

    //private void UnregisterObject(Sc_WeightedObject wObject)
    //{
    //    if (!_registeredObjects.Contains(wObject)) return;

    //    _registeredObjects.Remove(wObject);
    //    weightedObject._weight -= wObject._weight;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    Sc_WeightedObject wObject = other.GetComponent<Sc_WeightedObject>();
    //    if (wObject) RegisterObject(wObject);
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    Sc_WeightedObject wObject = other.GetComponent<Sc_WeightedObject>();
    //    if (wObject) UnregisterObject(wObject);
    //}
}
