using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Lever : Sc_Activator
{
    [Header("OBJECT REFS")]
    public Transform _axlePivot;

    [Header("LEVER PARAMETERS")]
    private bool _leverLowered = false;

    [Header("SOUND REFERENCES")]
    public AudioSource _source;
    public AudioClip _leverLower;
    public AudioClip _leverLift;

    private Coroutine _delayedActivationCo = null;

    public void OnInteract()
    {
        ToggleLever();     
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
        if (!StopDelayedActivation())
        {
            ToggleActivate();
        }        
        _leverLowered = false;

        Sc_GameManager.instance.SoundManager.PlaySFX(_source, _leverLift, new Vector2(.95f, 1.05f));
    }

    private void LowerLever()
    {
        _axlePivot.localEulerAngles = new Vector3(40f, 0f, 0f);
        DelayActivation();
        _leverLowered = true;

        Sc_GameManager.instance.SoundManager.PlaySFX(_source, _leverLower, new Vector2(.95f, 1.05f));
    }
}
