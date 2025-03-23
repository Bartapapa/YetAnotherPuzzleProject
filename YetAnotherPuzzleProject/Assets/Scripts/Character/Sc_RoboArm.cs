using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_RoboArm : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_CharacterController Controller;

    [Header("STATE")]
    public bool HasRoboArm = false;

    [Header("LINKED ACTIVATEABLES")]
    public List<Sc_RoboArmInputListener> LinkedRoboArmListeners = new List<Sc_RoboArmInputListener>();

    private bool _isChanneling = false;
    public bool HasLinkedListeners { get { return LinkedRoboArmListeners.Count > 0; } }
    public bool IsChanneling { get { return _isChanneling; } }
    public bool CanChannel { get { return !HasRoboArm || !HasLinkedListeners ? false : true; } }

    public delegate void BoolEvent(bool value);
    public event BoolEvent OnChannel;

    public void StartChannel()
    {
        if (_isChanneling) return;

        _isChanneling = true;
        Controller.CanMove = false;
        Controller.CanRotate = false;

        OnChannel?.Invoke(true);
    }

    public void StopChannel()
    {
        if (!_isChanneling) return;

        _isChanneling = false;
        Controller.CanMove = true;
        Controller.CanRotate = true;

        OnChannel?.Invoke(false);
    }
}
