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

    private bool _leverLowered = false;

    public void OnInteract()
    {
        ToggleLever();

        foreach(Sc_Activateable activateable in _activateables)
        {
            activateable.ToggleActivation();
        }      
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
