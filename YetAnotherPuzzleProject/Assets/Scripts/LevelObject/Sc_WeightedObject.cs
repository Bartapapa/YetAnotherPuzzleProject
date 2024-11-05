using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_WeightedObject : MonoBehaviour
{
    [Header("PARAMETERS")]
    public int _weight = 10;

    private Vector3 _rbVelocity = Vector3.zero;
    public Vector3 RBVelocity { get { return _rbVelocity; } set { _rbVelocity = value; } }

    public delegate void DefaultEvent();
    public event DefaultEvent OnStateChanged;

    public void StateChange()
    {
        OnStateChanged?.Invoke();
    }
}
