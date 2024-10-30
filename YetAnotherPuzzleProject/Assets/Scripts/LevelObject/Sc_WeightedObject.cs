using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_WeightedObject : MonoBehaviour
{
    [Header("PARAMETERS")]
    public int _weight = 10;

    public delegate void DefaultEvent();
    public event DefaultEvent OnStateChanged;

    public void StateChange()
    {
        OnStateChanged?.Invoke();
    }
}
