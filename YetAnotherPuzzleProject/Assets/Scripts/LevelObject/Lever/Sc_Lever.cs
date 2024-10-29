using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Lever : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public List<Sc_Activateable> _activateables;
    public Transform _axlePivot;

    [Header("LEVER PARAMETERS")]
    public bool _inverseLever = false;
    public float ActivationDelay = 1f;

    private bool _leverLowered = false;

    [Header("SOUND REFERENCES")]
    public AudioSource _source;
    public AudioClip _leverLower;
    public AudioClip _leverLift;

    private Coroutine _delayedActivationCo = null;

    public void OnInteract()
    {
        ToggleLever();

        //foreach(Sc_Activateable activateable in _activateables)
        //{
        //    activateable.ToggleActivation();
        //}      
    }

    private void ToggleLever()
    {
        if (_leverLowered)
        {
            LiftLever();
        }
        else
        {
            LowerLever();
        }
    }

    private void LiftLever()
    {
        _axlePivot.localEulerAngles = new Vector3(-40f, 0f, 0f);
        StopDelayedActivation();
        foreach (Sc_Activateable activateable in _activateables)
        {
            activateable.ToggleActivation();
        }
        _leverLowered = false;

        Sc_GameManager.instance.SoundManager.PlaySFX(_source, _leverLift, new Vector2(.95f, 1.05f));
    }

    private void LowerLever()
    {
        _axlePivot.localEulerAngles = new Vector3(40f, 0f, 0f);
        DelayActivation();

        Sc_GameManager.instance.SoundManager.PlaySFX(_source, _leverLower, new Vector2(.95f, 1.05f));
    }

    private void DelayActivation()
    {
        if (_leverLowered) return;
        _leverLowered = true;
        _delayedActivationCo = StartCoroutine(DelayedActivationCoroutine());
    }

    private void StopDelayedActivation()
    {
        if (_delayedActivationCo != null)
        {
            StopCoroutine(_delayedActivationCo);
            _delayedActivationCo = null;
        }
    }

    private IEnumerator DelayedActivationCoroutine()
    {
        float timer = 0f;
        while (timer < ActivationDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        foreach (Sc_Activateable activateable in _activateables)
        {
            activateable.ToggleActivation();
        }
        _delayedActivationCo = null;
    }
}
