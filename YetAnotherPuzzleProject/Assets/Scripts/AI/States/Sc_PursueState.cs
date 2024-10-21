using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PursueState : Sc_State
{
    //[Header("PURSUE PARAMETERS AND REFS")]
    //canSeeHearPlayer bool
    //lastKnownPlayerPosition v3
    //goToPlayerPosition

    [Header("PURSUE REFS")]
    public Sc_Character_Player CurrentPlayerTarget;
    [ReadOnly] [SerializeField] private Vector3 _targetLastKnownLocation = Vector3.zero;

    [Header("STATE REFS")]
    public Sc_IdleState IdleState;
    public Sc_InvestigateState InvestigateState;

    [Header("DEBUG")]
    public GameObject PursuitLight;
    public GameObject VisionLight;

    private bool HeardTargetProduceSoundRecently { get { return _hearTargetSoundGrace != null; } }
    private Coroutine _hearTargetSoundGrace = null;

    public override void OnStateEntered(Sc_AIBrain brain)
    {
        brain.Controller._maxGroundedMoveSpeed = brain.PursueMoveSpeed;
        PursuitLight.SetActive(true);
        VisionLight.SetActive(false);
    }

    public override void OnStateExited(Sc_AIBrain brain)
    {
        CurrentPlayerTarget = null;
        _targetLastKnownLocation = Vector3.zero;

        brain.CurrentAwareness = brain.AwarenessResetValue;

        PursuitLight.SetActive(false);
        VisionLight.SetActive(true);
    }

    public override Sc_State Tick(Sc_AIBrain brain)
    {
        //pursue closest seen player
        //If see no player, will go to closest lastKnownPlayerPosition
        //Once reached, revert to InvestigateState, set InvestigationPoint to lastKnownPlayerPosition
        //If gets close enough to player and can attack, then go to AttackState

        if (CanLocateTarget(brain))
        {
            _targetLastKnownLocation = CurrentPlayerTarget.transform.position;
        }

        if (brain.MoveTo(_targetLastKnownLocation))
        {
            InvestigateState.InvestigationPoint = _targetLastKnownLocation;
            return InvestigateState;
        }

        return base.Tick(brain);
    }

    public override void OnHearSomething(Sc_AIBrain brain, Sc_SoundStimuli sstimuli)
    {
        //Debug.Log("I heard something!");
        if (sstimuli.Player != null)
        {
            if (sstimuli.Player == CurrentPlayerTarget)
            {
                _targetLastKnownLocation = sstimuli.Player.transform.position;
                //Produce sound recently coroutine.
            }
        }
    }

    public override void OnSawSomething(Sc_AIBrain brain, Sc_VisualStimuli vstimuli)
    {
        //Debug.Log("I saw something!");
        if (vstimuli.Player != null)
        {
            if (vstimuli.Player == CurrentPlayerTarget)
            {
                _targetLastKnownLocation = vstimuli.Player.transform.position;
            }
        }
    }

    public bool CanLocateTarget(Sc_AIBrain brain)
    {
        bool canLocateTarget = false;

        switch (brain.MainSense)
        {
            case AISenseType.Sight:
                if (brain.SeenCharacters.Contains(CurrentPlayerTarget))
                {
                    canLocateTarget = true;
                }
                break;
            case AISenseType.Hearing:
                if (HeardTargetProduceSoundRecently)
                {
                    canLocateTarget = true;
                }
                break;
            default:
                break;
        }

        return canLocateTarget;
    }

    public override void OnAwarenessThresholdReached(Sc_AIBrain brain, Sc_Character_Player breachingPlayer)
    {
        //Only change target if target isn't 'seen'.
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_targetLastKnownLocation != Vector3.zero)
        {
            Gizmos.DrawWireSphere(_targetLastKnownLocation + Vector3.up, .4f);
        }
    }
}
