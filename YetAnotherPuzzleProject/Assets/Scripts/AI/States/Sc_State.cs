using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sc_State : MonoBehaviour
{
    public virtual void OnStateEntered(Sc_AIBrain brain)
    {

    }

    public virtual void OnStateExited(Sc_AIBrain brain)
    {

    }

    public virtual Sc_State Tick(Sc_AIBrain brain)
    {
        return this;
    }
}
