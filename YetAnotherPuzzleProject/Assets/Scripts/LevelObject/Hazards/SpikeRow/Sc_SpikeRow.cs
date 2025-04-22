using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_SpikeRow : MonoBehaviour
{
    [Header("SPIKE ROW OBJECT REFS")]
    public Sc_PlayerDetector PlayerDetector;
    public BoxCollider Damager;

    [Header("PLAYER DETECTION PARAMETERS")]
    public float PlayerDetectionTimeThreshold = .5f;
    private float _currentPlayerDetectionDuration = 0f;

    [Header("RESET PARAMETERS")]
    public float ResetTime = 3f;

    private bool _ejected = false;
    private Coroutine _postEjectWaitCo;
    private Coroutine _resetIntoPlaceCo;
    private Coroutine _ejectSpikesDamagerCo;
    private List<Sc_CharacterController> _detectedCharacters = new List<Sc_CharacterController>();

    public delegate void DefaultEvent();
    public event DefaultEvent Ejected;
    public event DefaultEvent Reset;

    private void Start()
    {
        InitializeSpikeRow();
    }

    private void InitializeSpikeRow()
    {
        PlayerDetector.CharacterDetected -= OnCharacterDetected;
        PlayerDetector.CharacterDetected += OnCharacterDetected;

        PlayerDetector.CharacterUndetected -= OnCharacterUndetected;
        PlayerDetector.CharacterUndetected += OnCharacterUndetected;

        Damager.enabled = false;
    }

    private void Update()
    {
        HandleDetectionThreshold();
    }

    private void HandleDetectionThreshold()
    {
        if (_ejected) return;
        if (_detectedCharacters.Count >= 1)
        {
            _currentPlayerDetectionDuration += Time.deltaTime;
            if (_currentPlayerDetectionDuration >= PlayerDetectionTimeThreshold)
            {
                EjectSpikes();
                _currentPlayerDetectionDuration = 0f;
            }
        }
        else
        {
            _currentPlayerDetectionDuration = 0f;
        }
    }

    private void EjectSpikes()
    {
        if (_resetIntoPlaceCo != null)
        {
            StopCoroutine(_resetIntoPlaceCo);
            _resetIntoPlaceCo = null;
        }

        _ejected = true;

        if (_ejectSpikesDamagerCo != null)
        {
            StopCoroutine(_ejectSpikesDamagerCo);
            _ejectSpikesDamagerCo = null;
        }
        _ejectSpikesDamagerCo = StartCoroutine(EjectSpikesDamagerCo());

        Ejected?.Invoke();

        _postEjectWaitCo = StartCoroutine(PostEjectWaitCo());
    }

    private void ResetSpikes()
    {
        if (_postEjectWaitCo != null)
        {
            StopCoroutine(_postEjectWaitCo);
            _postEjectWaitCo = null;
        }

        Reset?.Invoke();

        _resetIntoPlaceCo = StartCoroutine(ResetIntoPlaceCo());
    }

    private void OnCharacterDetected(Sc_CharacterController character)
    {
        _detectedCharacters.Add(character);
    }

    private void OnCharacterUndetected(Sc_CharacterController character)
    {
        _detectedCharacters.Remove(character);
    }

    private IEnumerator EjectSpikesDamagerCo()
    {
        Damager.enabled = true;
        float timer = 0f;
        float duration = .2f;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Damager.enabled = false;
        _ejectSpikesDamagerCo = null;
    }

    private IEnumerator PostEjectWaitCo()
    {
        float timer = 0f;
        float duration = ResetTime;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _postEjectWaitCo = null;
        ResetSpikes();
    }

    private IEnumerator ResetIntoPlaceCo()
    {
        float timer = 0f;
        float duration = 2f;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _resetIntoPlaceCo = null;
        _ejected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Sc_Health health = other.gameObject.GetComponent<Sc_Health>();
        if (health)
        {
            Debug.LogWarning(other.gameObject.name + " has been hit by spikes!");
        }
    }
}
