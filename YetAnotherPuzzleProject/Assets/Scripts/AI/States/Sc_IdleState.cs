using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_IdleState : Sc_State
{
    //[Header("IDLE PARAMETERS AND REFS")]
    //Patrol ref
    //Err bool

    [Header("STATE REFS")]
    public Sc_InvestigateState InvestigateState;

    public override void OnStateEntered()
    {
        
    }

    public override void OnStateExited()
    {
        
    }

    public override Sc_State Tick(Sc_AIBrain brain)
    {
        //If has patrol, follow patrol.
        //If not, check if errs.
        //If does, then err.

        //If sees or hears something, go to investigate state.


        if (brain.Sight.SeesPlayers().Count >= 1)
        {
            Debug.Log("I see player!");
        }
        return base.Tick(brain);
    }
}
