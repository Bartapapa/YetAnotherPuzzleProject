using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Lever : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Activateable _activateable;
    public Transform _axlePivot;

    private bool _leverLowered = false;

    public void OnInteract()
    {
        ToggleLever();
        _activateable.ToggleActivation();
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
        _leverLowered = false;
    }

    private void LowerLever()
    {
        _axlePivot.localEulerAngles = new Vector3(40f, 0f, 0f);
        _leverLowered = true;
    }
}
