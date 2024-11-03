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
    public AudioClip _leverFailed;

    private Coroutine _delayedActivationCo = null;

    public void OnInteract()
    {
        ToggleLever();     
    }

    protected override void FailedToActivate()
    {
        base.FailedToActivate();
        Sc_GameManager.instance.SoundManager.PlaySFX(_source, _leverFailed, new Vector2(.95f, 1.05f));
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
        if (!StopDelayedActivation())
        {
            if (!ToggleActivate())
            {
                return;
            }
        }

        _axlePivot.localEulerAngles = new Vector3(-40f, 0f, 0f);
        _leverLowered = false;

        Sc_GameManager.instance.SoundManager.PlaySFX(_source, _leverLift, new Vector2(.95f, 1.05f));
    }

    private void LowerLever()
    {
        if (!DelayActivation())
        {
            return;
        }

        _axlePivot.localEulerAngles = new Vector3(40f, 0f, 0f);
        _leverLowered = true;

        Sc_GameManager.instance.SoundManager.PlaySFX(_source, _leverLower, new Vector2(.95f, 1.05f));
    }
}
