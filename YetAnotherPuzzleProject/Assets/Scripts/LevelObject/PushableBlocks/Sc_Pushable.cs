using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Pushable : Sc_Activateable
{
    [Header("PUSHABLE OBJECT REFS")]
    public Sc_PowerGenerator Generator;
    public Sc_WeightedObject WObject;
    public Transform MeshPivot;
    public CinemachineImpulseSource ImpulseSource;
    public ParticleSystem Dust;

    [Header("STATE")]
    public bool _roboArmEnergized = false;

    [Header("MOVEMENT")]
    public float _maxSpeed = 1f;
    public float _speedSharpness = 15f;
    public Vector3 _gravity = new Vector3(0f, -30f, 0f);
    protected Vector3 _pushedDirection = Vector3.zero;
    [ReadOnly] public Vector3 InheritedVelocity = Vector3.zero;
    [ReadOnly] public float InheritedYaw = 0f;
    [ReadOnly] public Vector3 _targetUp = Vector3.up;

    [Header("SOUND")]
    public AudioSource Source;
    public AudioClip PushLoop;
    public float PushLoopVolume = .3f;
    public AudioClip Land;
    public float LandVolume = .5f;
    public Vector2 MinMaxLandPitch = new Vector2(.9f, 1f);

    private float _pushLoopTimer = 0f;
    private float _pushLoopCallDuration = .3f;
    private bool _loopSound = false;

    [Header("GROUND")]
    public LayerMask _groundLayers;
    [SerializeField] protected bool _isGrounded = true;
    protected RaycastHit _groundHit;
    protected RaycastHit _obstacleHit;

    [Header("GENERATOR")]
    public List<Transform> EmissiveMeshes = new List<Transform>();
    public List<Transform> DebugGeneratorMeshes = new List<Transform>();
    public Material DeEnergizedMat;
    public Material EnergizedMat;
    private List<Renderer> _renderers = new List<Renderer>();
    private List<Material> _emissiveMats = new List<Material>();

    [Header("CORE FORM")]
    public List<Sc_ShedMesh> ShedMeshes = new List<Sc_ShedMesh>();
    public Vector3 CoreFormBoxColliderSize = new Vector3(1.5f, 1.5f, 1.5f);
    public float CoreFormCapsuleColliderRadius = .75f;
    public float CoreFormCapsuleColliderHeight = 1.5f;
    private bool _isInCoreForm = false;
    public bool IsInCoreForm { get { return _isInCoreForm; } }


    protected Coroutine _energizeCO;

    protected Sc_CharacterController _pushedBy;
    public Sc_CharacterController PushedBy { get { return _pushedBy; } set { _pushedBy = value; } }

    protected BoxCollider _boxCollider;
    public Collider BoxCollider { get { return _boxCollider; } }
    protected CapsuleCollider _capsuleCollider;
    public Collider CapsuleCOllider { get { return _capsuleCollider; } }
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

    protected override void Start()
    {
        base.Start();

        InitializePushable();
        InitializeEmissiveMeshes();
    }

    private void InitializeEmissiveMeshes()
    {
        foreach(Transform mesh in EmissiveMeshes)
        {
            Renderer rend = mesh.GetComponent<Renderer>();
            if (rend)
            {
                _emissiveMats.Add(rend.material);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        HandlePushSound();
        HandleMeshUp();
    }

    private void HandleMeshUp()
    {
        if (_isInCoreForm) return;

        if (!_isGrounded)
        {
            MeshPivot.up = Vector3.MoveTowards(MeshPivot.up, Vector3.up, .2f * Time.fixedDeltaTime);
        }
        else
        {
            MeshPivot.up = Vector3.MoveTowards(MeshPivot.up, _targetUp, 1f * Time.fixedDeltaTime);
        }
    }

    public virtual void RoboArmEnergize(bool energize)
    {
        _roboArmEnergized = energize;
        Energize(energize);
    }

    #region Activateable implementation
    public override bool Activate(bool toggleOn)
    {
        if (base.Activate(toggleOn))
        {
            Energize(toggleOn);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void ForceActivate(bool toggleOn)
    {
        base.ForceActivate(toggleOn);
        Energize(toggleOn);
    }

    public override void OnLockEngaged(bool engage)
    {

    }

    public override void OnLockDestroyed()
    {
        
    }
    #endregion

    public virtual void Energize(bool energize)
    {
        Generator.GeneratePower(energize);

        if (energize)
        {
            if (_energizeCO != null)
            {
                StopCoroutine(_energizeCO);
            }
            _energizeCO = StartCoroutine(EnergizingCO());
        }
        
        //Material toMat = energize ? EnergizedMat : DeEnergizedMat;
        //foreach(Renderer rend in _renderers)
        //{
        //    rend.material = toMat;
        //}
    }

    private IEnumerator EnergizingCO()
    {
        float duration = 1f;
        float timer = 0f;
        while (timer < duration)
        {
            foreach(Material emissiveMat in _emissiveMats)
            {
                float alpha = (timer / duration) * 150f;
                emissiveMat.SetFloat("_EmissiveStrength", alpha);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        foreach (Material emissiveMat in _emissiveMats)
        {
            emissiveMat.SetFloat("_EmissiveStrength", 150f);
        }
        
        _energizeCO = null;
    }

    private void HandlePushSound()
    {
        if (_pushLoopTimer < _pushLoopCallDuration && _loopSound)
        {
            _pushLoopTimer += Time.deltaTime;
        }
        else
        {
            StopPushSound();
        }
    }

    private void StopPushSound()
    {
        if (!_loopSound) return;

        if (Sc_GameManager.instance != null)
        {
            Sc_GameManager.instance.SoundManager.FadeOut(Source, .2f);
        }
        _pushLoopTimer = -99f;
        _loopSound = false;
    }

    private void PushSound()
    {
        if (!_loopSound)
        {
            if (Sc_GameManager.instance != null)
            {
                Sc_GameManager.instance.SoundManager.PlayLoopingSFX(Source, PushLoop);
                Sc_GameManager.instance.SoundManager.FadeIn(Source, .2f, PushLoopVolume);
            }
            _loopSound = true;
        }
        _pushLoopTimer = 0f;
    }

    protected virtual void InitializePushable()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogWarning(this.name + " doesn't have a Rigidbody!");
            return;
        }

        _boxCollider = GetComponent<BoxCollider>();
        if (_boxCollider == null)
        {
            Debug.LogWarning(this.name + " doesn't have a BoxCollider!");
            return;
        }

        _boxColliderCenter = _boxCollider.center;
        _boxColliderHalfExtents = new Vector3(_boxCollider.size.x*.5f, _boxCollider.size.y*.5f, _boxCollider.size.z*.5f);

        _capsuleCollider = GetComponent<CapsuleCollider>();
        if (_capsuleCollider == null)
        {
            Debug.LogWarning(this.name + " doesn't have a CapsuleCollider!");
            return;
        }
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
        if (!_isGrounded)
        {
            if (localIsGrounded && _rb.velocity.y <= -5f)
            {
                OnLand();
            }
            WObject.RBVelocity = _rb.velocity;
        }
        else
        {
            WObject.RBVelocity = Vector3.zero;
            _targetUp = _groundHit.normal;
        }

        return localIsGrounded;
    }

    private void OnLand()
    {
        GroundShake();
        LandSound();
    }

    private void LandSound()
    {
        if (Sc_GameManager.instance != null)
        {
            Sc_GameManager.instance.SoundManager.PlaySFX(Source, Land, LandVolume, MinMaxLandPitch);
        }
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
            //Pushable has been pushed - play continuous sound.
            PushSound();

            targetMovementVelocity = _pushedDirection * _maxSpeed;
            _pushedDirection = Vector3.zero;
        }
        else
        {
            StopPushSound();
        }
        //targetMovementVelocity += _gravity * Time.fixedDeltaTime;
        float rbYVelocity = _rb.velocity.y + (_gravity.y * Time.fixedDeltaTime);
        targetMovementVelocity = new Vector3(targetMovementVelocity.x, rbYVelocity, targetMovementVelocity.z);
        targetMovementVelocity = targetMovementVelocity + new Vector3(InheritedVelocity.x, 0f, InheritedVelocity.z);
        InheritedVelocity = Vector3.zero;
        _rb.velocity = Vector3.Lerp(_rb.velocity, targetMovementVelocity, 1f - Mathf.Exp(-_speedSharpness * Time.fixedDeltaTime));
        //_rb.velocity += _gravity * Time.fixedDeltaTime;
    }

    public void RedirectInputToPushDirection (ref CharacterInput input)
    {
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(input.moveX, 0f, input.moveY), 1f);
        float cameraRotation = input.cameraRef.transform.eulerAngles.y;
        Quaternion controlRotation = Quaternion.Euler(0, cameraRotation, 0);
        Vector3 desiredMoveInputVector = controlRotation * moveInputVector;

        float forwardDot = Vector3.Dot(desiredMoveInputVector, transform.forward);
        float rightwardDot = Vector3.Dot(desiredMoveInputVector, transform.right);
        Vector3 pushDirection = Vector3.zero;
        if (forwardDot >= .85f)
        {
            pushDirection = transform.forward;
        }
        else if (forwardDot <= -.85f)
        {
            pushDirection = -transform.forward;
        }
        else if (rightwardDot >= .85f)
        {
            pushDirection = transform.right;
        }
        else if (rightwardDot <= -.85f)
        {
            pushDirection = -transform.right;
        }

        if (pushDirection != Vector3.zero)
        {
            Push(pushDirection);
        }      
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

    public virtual void OnCodeInputComplete(int codeID)
    {
        if(codeID == 0)
        {
            Code_BreakOpen();
        }
    }

    public void Code_BreakOpen()
    {
        //spawn VFX + camerashake

        //undo collisions for base form, set collisions for core form
        _boxCollider.size = CoreFormBoxColliderSize;
        _capsuleCollider.height = CoreFormCapsuleColliderHeight;
        _capsuleCollider.radius = CoreFormCapsuleColliderRadius;
        _rb.useGravity = false;
        _gravity = Vector3.zero;

        //make interactibles non interactible
        foreach (Sc_ShedMesh shedMesh in ShedMeshes)
        {
            shedMesh.OnMeshShed();
        }
    }
}
