using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PursueState : Sc_State
{
    [Header("PURSUE REFS")]
    public Sc_Character_Player CurrentPlayerTarget;
    [ReadOnly] public Vector3 TargetLastKnownLocation = Vector3.zero;
    public float HearTargetSoundTrackingDuration = 1f;
    public float ChangeTargetDuration = 1.5f;
    public float AttackRange = 2f;
    private Coroutine _changeTargetCo;
    public bool ChangingTarget { get { return _changeTargetCo != null; } }

    [Header("STATE REFS")]
    public Sc_IdleState IdleState;
    public Sc_InvestigateState InvestigateState;

    [Header("DEBUG")]
    public GameObject PursuitLight;
    public GameObject VisionLight;

    private bool HeardTargetProduceSoundRecently { get { return _hearTargetSoundTrackingCo != null; } }
    private Coroutine _hearTargetSoundTrackingCo = null;

    public override void OnStateEntered(Sc_AIBrain brain)
    {
        brain.Controller._maxGroundedMoveSpeed = brain.PursueMoveSpeed;
        PursuitLight.SetActive(true);
        VisionLight.SetActive(false);
    }

    public override void OnStateExited(Sc_AIBrain brain)
    {
        CurrentPlayerTarget = null;
        TargetLastKnownLocation = Vector3.zero;

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
            TargetLastKnownLocation = CurrentPlayerTarget.transform.position;
        }

        if (brain.MoveTo(TargetLastKnownLocation))
        {
            //Attack state.
            InvestigateState.InvestigationPoint = TargetLastKnownLocation;
            return InvestigateState;
        }

        return base.Tick(brain);
    }

    public override void OnHearSomething(Sc_AIBrain brain, Sc_SoundStimuli sstimuli)
    {
        //Debug.Log("I heard something!");
        if (sstimuli.PlayerSource != null)
        {
            if (sstimuli.PlayerSource == CurrentPlayerTarget)
            {
                TargetLastKnownLocation = sstimuli.PlayerSource.transform.position;
                //Produce sound recently coroutine.
                if (_hearTargetSoundTrackingCo != null)
                {
                    StopCoroutine(_hearTargetSoundTrackingCo);
                }
                _hearTargetSoundTrackingCo = StartCoroutine(HearTargetSoundTrackingCoroutine(sstimuli));
            }
        }
    }

    public override void OnSawSomething(Sc_AIBrain brain, Sc_VisualStimuli vstimuli)
    {
        //Debug.Log("I saw something!");
        if (vstimuli.PlayerSource != null)
        {
            if (vstimuli.PlayerSource == CurrentPlayerTarget)
            {
                ChangeTarget(brain, vstimuli.PlayerSource);
            }
        }
    }

    public override void OnAwarenessThresholdReached(Sc_AIBrain brain, Sc_Character_Player breachingPlayer)
    {
        if (!CanLocateTarget(brain) && !ChangingTarget)
        {
            CurrentPlayerTarget = breachingPlayer;
            TargetLastKnownLocation = breachingPlayer.transform.position;
        }
        //Only change target if target isn't 'seen'.
    }

    #region Location and tracking

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

    private IEnumerator HearTargetSoundTrackingCoroutine(Sc_SoundStimuli sstimuli)
    {
        float timer = 0f;
        while (timer < HearTargetSoundTrackingDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _hearTargetSoundTrackingCo = null;
    }
    #endregion

    #region Change targets

    public void ChangeTarget(Sc_AIBrain brain, Sc_Character_Player toPlayer)
    {
        brain.Controller.CanMove = false;
        TargetLastKnownLocation = toPlayer.transform.position;

        if (_changeTargetCo != null)
        {
            StopCoroutine(_changeTargetCo);
        }
        _changeTargetCo = StartCoroutine(ChangeTargetCoroutine(brain));
    }

    private IEnumerator ChangeTargetCoroutine(Sc_AIBrain brain)
    {
        float timer = 0f;
        while (timer < ChangeTargetDuration)
        {
            TargetLastKnownLocation = CurrentPlayerTarget.transform.position;
            brain.Controller.LookAt(TargetLastKnownLocation);
            timer += Time.deltaTime;
            yield return null;
        }
        StopInvestigatingSomething(brain);
    }

    public void StopInvestigatingSomething(Sc_AIBrain brain)
    {
        brain.Controller.CanMove = true;
        brain.Controller.StopLookAt();
        if (_changeTargetCo != null)
        {
            StopCoroutine(_changeTargetCo);
            _changeTargetCo = null;
        }
    }

    #endregion

    #region Attacking

    private void Attack(Sc_AIBrain brain, Vector3 point)
    {

    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (TargetLastKnownLocation != Vector3.zero)
        {
            Gizmos.DrawWireSphere(TargetLastKnownLocation + Vector3.up, .4f);
        }
    }
}
