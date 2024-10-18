using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sc_State : MonoBehaviour
{
    public virtual void OnStateEntered()
    {

    }

    public virtual void OnStateExited()
    {

    }

    public virtual Sc_State Tick(Sc_AIBrain brain)
    {
        return this;
    }
}
