using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_InvestigateState : Sc_State
{
    //[Header("INVESTIGATE PARAMETERS AND REFS")]
    //investigation point

    [Header("STATE REFS")]
    public Sc_IdleState IdleState;
    public Sc_PursueState PursueState;

    public override void OnStateEntered(Sc_AIBrain brain)
    {
        //NoticedSomething();
        brain.Controller._maxGroundedMoveSpeed = brain.CurrentDefaultMoveSpeed;
    }

    public override void OnStateExited(Sc_AIBrain brain)
    {
        
    }

    public override Sc_State Tick(Sc_AIBrain brain)
    {
        //Move to investigation point
        //If reached destination, investigate for a moment.
        //If investigation ended, return to idle.
        //If see/hear anything new at any point, NoticedSomething();
            //Constantly check against the current level's "Memory" to see if an object has been displaced or activated. If this happens, set new memory to current state.
        //If see/hear player, enter pursue state.

        return base.Tick(brain);
    }

    //NoticedSOmething(V3 point)
    //Stunned for a second, changes investigation point.
    //Increase alertness
    //if reached alertness threshold, the character stays alerted (has higher movespeed and detection range is increased, if any)
}
