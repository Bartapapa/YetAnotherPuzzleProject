using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_SpiritRevealer : MonoBehaviour
{
    [Header("PARAMETERS")]
    public float Range = 5f;
    public bool Active = true;

    private SphereCollider _coll;

    public delegate void RevealerEvent(Sc_SpiritRevealer revealer);
    public RevealerEvent OnStateChanged;

    private void Start()
    {
        _coll = GetComponent<SphereCollider>();
        _coll.isTrigger = true;
        _coll.radius = Range;
    }

    public void StateChange()
    {
        OnStateChanged?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Sc_Spirit spirit = other.GetComponent<Sc_Spirit>();
        if (spirit)
        {
            spirit.RegisterRevealer(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Sc_Spirit spirit = other.GetComponent<Sc_Spirit>();
        if (spirit)
        {
            spirit.UnRegisterRevealer(this);
        }
    }
}
