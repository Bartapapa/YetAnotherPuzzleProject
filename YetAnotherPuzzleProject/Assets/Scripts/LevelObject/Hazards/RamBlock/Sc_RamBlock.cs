using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_RamBlock : MonoBehaviour
{
    [Header("RAM BLOCK OBJECT REFS")]
    public Sc_PlayerDetector PlayerDetector;
    public ParticleSystem CrashDust;
    public CinemachineImpulseSource ImpulseSource;

    [Header("PLAYER DETECTION PARAMETERS")]
    public float PlayerDetectionTimeThreshold = .5f;
    public float DetectionRange = 5f;
    private float _currentPlayerDetectionDuration = 0f;

    [Header("RAM PARAMETERS")]
    public float MaxRammingSpeed = 10f;
    public float MaxRetreatingSpeed = 3f;
    public float SpeedSharpness = 2f;
    public LayerMask WallLayers;
    public float PostCrashRestPeriod = 2f;
    private bool _isCharging = false;
    private bool _canDetect = true;
    private bool _isRetreating = false;
    private Vector3 _originPos;

    public List<Sc_CharacterController> _parentedControllers = new List<Sc_CharacterController>();
    public List<Sc_Pushable> _parentedPushables = new List<Sc_Pushable>();
    private List<Sc_CrushingHandler> _detectedCharacters = new List<Sc_CrushingHandler>();
    private Rigidbody _rb;
    private RaycastHit _crashHit;
    private Coroutine _postCrashCo;
    private Coroutine _resetIntoPlaceCo;
    private Vector3 _cachedPos;
    private Vector3 _calcVel = Vector3.zero;

    private void Start()
    {
        InitializeDetector();

        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogWarning(this.name + " doesn't have a Rigidbody!");
            return;
        }

        _rb.isKinematic = true;
        _rb.useGravity = false;

        _originPos = transform.position;
        _cachedPos = transform.position;
    }

    private void InitializeDetector()
    {
        Vector3 offset = new Vector3(0, 1f, 1f + (DetectionRange * .5f));
        Vector3 size = new Vector3(1.7f, 1.7f, DetectionRange);
        PlayerDetector.InitializeDetector(offset, size);

        PlayerDetector.CharacterDetected -= OnCharacterDetected;
        PlayerDetector.CharacterDetected += OnCharacterDetected;

        PlayerDetector.CharacterUndetected -= OnCharacterUndetected;
        PlayerDetector.CharacterUndetected += OnCharacterUndetected;
    }

    private void Update()
    {
        HandleDetectionThreshold();
    }

    private void HandleDetectionThreshold()
    {
        if (_isCharging || !_canDetect) return;

        if (_detectedCharacters.Count >= 1 && AtLeastOneUncrushedTarget())
        {
            _currentPlayerDetectionDuration += Time.deltaTime;
            if (_currentPlayerDetectionDuration >= PlayerDetectionTimeThreshold)
            {
                Charge();
                _currentPlayerDetectionDuration = 0f;
            }
        }
        else
        {
            _currentPlayerDetectionDuration = 0f;
        }
    }

    private bool AtLeastOneUncrushedTarget()
    {
        bool uncrushedTargetExists = false;
        foreach(Sc_CrushingHandler handler in _detectedCharacters)
        {
            if (!handler.Crushed) uncrushedTargetExists = true;
        }
        return uncrushedTargetExists;
    }

    private void Charge()
    {
        _isCharging = true;
        _rb.isKinematic = true;

        _detectedCharacters.Clear();
        _canDetect = false;
    }

    private void Crash()
    {
        _isCharging = false;
        _rb.isKinematic = true;
        _rb.position = _crashHit.point - (transform.forward * 1f) - (transform.up*1f);

        _calcVel = Vector3.zero;

        Sc_CameraManager.instance.CameraShake(ImpulseSource, .1f);

        CrashDust.Play();

        _postCrashCo = StartCoroutine(PostCrashCo());
    }

    private IEnumerator PostCrashCo()
    {
        float timer = 0f;
        float duration = PostCrashRestPeriod;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _isRetreating = true;
        _rb.isKinematic = true;
        _canDetect = true;
        _postCrashCo = null;
    }

    private IEnumerator ResetIntoPlaceCo()
    {
        float timer = 0f;
        float duration = 1f;
        _rb.isKinematic = true;
        Vector3 fromPos = _rb.position;
        Vector3 toPos = _originPos;
        _canDetect = false;

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(fromPos, toPos, timer / duration);
            yield return null;
        }

        _canDetect = true;
        transform.position = toPos;
        _isRetreating = false;
        _resetIntoPlaceCo = null;

        _calcVel = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (_isCharging)
        {
            _calcVel = Vector3.Lerp(_calcVel, transform.forward * MaxRammingSpeed, 1f - Mathf.Exp(-SpeedSharpness * Time.fixedDeltaTime));
            _rb.MovePosition(_rb.position + (_calcVel * Time.fixedDeltaTime));

            Vector3 rayOrigin = transform.position + (transform.up * 1f);
            if (Physics.Raycast(rayOrigin, transform.forward, out _crashHit, 1f + .2f, WallLayers, QueryTriggerInteraction.Ignore))
            {
                Crash();
            }
        }
        else if (_isRetreating && _resetIntoPlaceCo == null)
        {
            _calcVel = Vector3.Lerp(_calcVel, -transform.forward * MaxRetreatingSpeed, 1f - Mathf.Exp(-SpeedSharpness * Time.fixedDeltaTime));
            _rb.MovePosition(_rb.position + (_calcVel * Time.fixedDeltaTime));

            if (Vector3.Distance(_rb.position, _originPos) <= .5f)
            {
                _resetIntoPlaceCo = StartCoroutine(ResetIntoPlaceCo());
            }
        }

        Vector3 transmittedVel = ((transform.position - _cachedPos) / Time.fixedDeltaTime);
        TransmitVelocity(transmittedVel);
        _cachedPos = transform.position;
    }

    private void OnCharacterDetected(Sc_CharacterController character)
    {
        if (!_canDetect) return;
        Sc_CrushingHandler crushHandler = character.GetComponentInChildren<Sc_CrushingHandler>();
        if (crushHandler)
        {
            if (!crushHandler.Crushed)
            {
                _detectedCharacters.Add(crushHandler);
            }           
        }   
    }

    private void OnCharacterUndetected(Sc_CharacterController character)
    {
        Sc_CrushingHandler crushHandler = character.GetComponentInChildren<Sc_CrushingHandler>();
        if (crushHandler)
        {
            _detectedCharacters.Remove(crushHandler);
        }
    }

    private void TransmitVelocity(Vector3 toVel)
    {
        foreach (Sc_CharacterController controller in _parentedControllers)
        {
            controller.PushIntoDirection(toVel);
            //controller.InheritedVelocity += toVel;
        }

        foreach (Sc_Pushable pushable in _parentedPushables)
        {
            pushable.InheritedVelocity += toVel;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 from = transform.position + (transform.forward * 1f) + (transform.up * 1f);
        Vector3 to = from + (transform.forward * DetectionRange);
        Gizmos.DrawLine(from, to);
    }
}
