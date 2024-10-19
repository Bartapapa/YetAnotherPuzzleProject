using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PursueState : Sc_State
{
    //[Header("PURSUE PARAMETERS AND REFS")]
    //canSeeHearPlayer bool
    //lastKnownPlayerPosition v3
    //goToPlayerPosition

    [Header("STATE REFS")]
    public Sc_IdleState IdleState;
    public Sc_InvestigateState InvestigateState;

    public override void OnStateEntered(Sc_AIBrain brain)
    {
        brain.Controller._maxGroundedMoveSpeed = brain.PursueMoveSpeed;
    }

    public override void OnStateExited(Sc_AIBrain brain)
    {
        
    }

    public override Sc_State Tick(Sc_AIBrain brain)
    {
        //pursue closest seen player
        //If see no player, will go to closest lastKnownPlayerPosition
        //Once reached, revert to InvestigateState, set InvestigationPoint to lastKnownPlayerPosition
        //If gets close enough to player and can attack, then go to AttackState
        return base.Tick(brain);
    }
}
