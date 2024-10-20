using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Sc_InvestigateState : Sc_State
{
    [Header("INVESTIGATE POINT")]
    [ReadOnly] public Vector3 InvestigationPoint;

    [Header("NOTICING/INVESTIGATING PARAMETERS")]
    public float NoticingDuration = 1f;
    public float InvestigationDuration = 2f;
    private Coroutine _noticedSomethingCO = null;
    private Coroutine _investigationCO = null;
    private bool IsInvestigating { get { return _investigationCO != null; } }
    private bool IsNoticing { get { return _noticedSomethingCO != null; } }

    [Header("STATE REFS")]
    public Sc_IdleState IdleState;
    public Sc_PursueState PursueState;

    [Header("DEBUG")]
    public GameObject NoticedSmthGO;
    public GameObject InvestigatingGO;

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

        if (brain.MoveTo(InvestigationPoint) && !IsInvestigating)
        {
            InvestigateSomething(brain, InvestigationPoint);
        }

        return base.Tick(brain);
    }

    public override void OnHearSomething(Sc_AIBrain brain, Sc_SoundStimuli sstimuli)
    {
        Debug.Log("I heard something!");
        NoticedSomething(brain, sstimuli.transform.position);
        //change InvestigationPoint
    }

    public override void OnSawSomething(Sc_AIBrain brain, Sc_VisualStimuli vstimuli)
    {
        Debug.Log("I saw something!");
        NoticedSomething(brain, vstimuli.transform.position);
        //change InvestigationPoint
    }

    #region Noticing
    private IEnumerator NoticedSomethingCoroutine(Sc_AIBrain brain)
    {
        float timer = 0f;
        while (timer < NoticingDuration)
        {
            brain.Controller.LookAt(InvestigationPoint);
            timer += Time.deltaTime;
            yield return null;
        }
        StopNoticingSomething(brain);
    }

    public void StopNoticingSomething(Sc_AIBrain brain)
    {
        brain.Controller.CanMove = true;
        brain.Controller.StopLookAt();
        NoticedSmthGO.SetActive(false);
        if (_noticedSomethingCO != null)
        {
            StopCoroutine(_noticedSomethingCO);
            _noticedSomethingCO = null;
        }
    }

    public void NoticedSomething(Sc_AIBrain brain, Vector3 stimuliPoint)
    {
        StopInvestigatingSomething(brain);

        brain.Controller.CanMove = false;
        NoticedSmthGO.SetActive(true);
        InvestigationPoint = stimuliPoint;
        if (_noticedSomethingCO != null)
        {
            StopCoroutine(_noticedSomethingCO);
        }
        _noticedSomethingCO = StartCoroutine(NoticedSomethingCoroutine(brain));
    }

    #endregion
    #region Investigation
    public void InvestigateSomething(Sc_AIBrain brain, Vector3 investigatePoint)
    {
        brain.Controller.CanMove = false;
        InvestigatingGO.SetActive(true);
        InvestigationPoint = investigatePoint;
        if (_investigationCO != null)
        {
            StopCoroutine(_investigationCO);
        }
        _investigationCO = StartCoroutine(InvestigateCoroutine(brain));
    }

    private IEnumerator InvestigateCoroutine(Sc_AIBrain brain)
    {
        float timer = 0f;
        while (timer < InvestigationDuration)
        {
            brain.Controller.LookAt(InvestigationPoint);
            timer += Time.deltaTime;
            yield return null;
        }
        OnInvestigationEnd(brain);
        StopInvestigatingSomething(brain);
    }

    public void StopInvestigatingSomething(Sc_AIBrain brain)
    {
        brain.Controller.CanMove = true;
        brain.Controller.StopLookAt();
        InvestigatingGO.SetActive(false);
        if (_investigationCO != null)
        {
            StopCoroutine(_investigationCO);
            _investigationCO = null;
        }
    }

    private void OnInvestigationEnd(Sc_AIBrain brain)
    {
        brain.GoToState(IdleState);
    }
    #endregion



    //NoticedSOmething(V3 point)
    //Stunned for a second, changes investigation point.
    //Increase alertness
    //if reached alertness threshold, the character stays alerted (has higher movespeed and detection range is increased, if any)
}
