using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_IdleState : Sc_State
{
    [Header("IDLE PARAMETERS AND REFS")]
    public bool Erring = false;
    public float MaxErringDistance = 2f;
    public List<Transform> Waypoints = new List<Transform>();
    public bool PatrolLoop = false;
    [ReadOnly][SerializeField] private int _currentPatrolPoint = 0;
    [ReadOnly][SerializeField] private bool _headingBack = false;
    //Patrol ref
    //Err bool

    [Header("STATE REFS")]
    public Sc_InvestigateState InvestigateState;
    public Sc_PursueState PursueState;

    public override void OnStateEntered(Sc_AIBrain brain)
    {
        brain.Controller._maxGroundedMoveSpeed = brain.IdleMoveSpeed;
    }

    public override void OnStateExited(Sc_AIBrain brain)
    {
        
    }

    public override Sc_State Tick(Sc_AIBrain brain)
    {
        if (Waypoints.Count > 1)
        {
            Patrol(brain);
        }
        //If has patrol, follow patrol.
        //If not, check if errs.
        //If does, then err.

        //If sees or hears something, go to investigate state.
        return base.Tick(brain);
    }

    public override void OnHearSomething(Sc_AIBrain brain, Sc_SoundStimuli sstimuli)
    {
        Debug.Log(brain.name + " heard " + sstimuli.Source + "!");
        InvestigateState.InvestigationPriority = sstimuli.Priority;
        brain.GoToState(InvestigateState);
        InvestigateState.NoticedSound(brain, sstimuli);
    }

    public override void OnSawSomething(Sc_AIBrain brain, Sc_VisualStimuli vstimuli)
    {
        Debug.Log(brain.name + " heard " + vstimuli.name + "!");
        InvestigateState.InvestigationPriority = vstimuli.Priority;
        brain.GoToState(InvestigateState);
        InvestigateState.NoticedSight(brain, vstimuli);
    }

    public override void OnAwarenessThresholdReached(Sc_AIBrain brain, Sc_Character_Player breachingPlayer)
    {
        PursueState.CurrentPlayerTarget = breachingPlayer;
        PursueState.TargetLastKnownLocation = breachingPlayer.transform.position;
        brain.GoToState(PursueState);
    }

    #region Patrol
    private void Patrol(Sc_AIBrain brain)
    {
        if (brain.MoveTo(Waypoints[_currentPatrolPoint].position))
        {
            if (!_headingBack)
            {
                _currentPatrolPoint++;
                if (_currentPatrolPoint >= Waypoints.Count)
                {
                    if (PatrolLoop)
                    {
                        _currentPatrolPoint = 0;
                    }
                    else
                    {
                        _headingBack = true;
                        _currentPatrolPoint = Waypoints.Count - 2;
                    }
                }
            }
            else
            {
                _currentPatrolPoint--;
                if (_currentPatrolPoint < 0)
                {
                    if (PatrolLoop)
                    {
                        _currentPatrolPoint = Waypoints.Count - 1;
                    }
                    else
                    {
                        _headingBack = false;
                        _currentPatrolPoint = 1;
                    }
                }
            }
        }
    }
    #endregion

    #region GIZMOS
    private void OnDrawGizmosSelected()
    {
        if (Waypoints.Count > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < Waypoints.Count; i++)
            {
                Gizmos.DrawSphere(Waypoints[i].position + Vector3.up, .1f);
                if (i > 0)
                {
                    Gizmos.DrawLine(Waypoints[i].position + Vector3.up, Waypoints[i - 1].position + Vector3.up);
                }
                else
                {
                    if (PatrolLoop)
                    {
                        Gizmos.DrawLine(Waypoints[i].position + Vector3.up, Waypoints[Waypoints.Count - 1].position + Vector3.up);
                    }
                }
            }
        }
    }
    #endregion
}
