using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Sc_InvestigateState : Sc_State
{
    [Header("INVESTIGATE POINT")]
    [ReadOnly] public Vector3 InvestigationPoint;
    [ReadOnly] public Sc_VisualStimuli InvestigationVisual;
    [ReadOnly] public Sc_SoundStimuli InvestigationSound;
    [ReadOnly][SerializeField] private int _investigationPriority = -1;
    [ReadOnly][SerializeField] private GameObject _cachedSoundSource = null;
    public int InvestigationPriority { get { return _investigationPriority; } set { _investigationPriority = value; } }
    private Vector3 _cachedTransformPos = Vector3.zero;

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
        brain.Controller._maxGroundedMoveSpeed = brain.InvestigateMoveSpeed;
        NoticedSmthGO.SetActive(false);
        InvestigatingGO.SetActive(false);
    }

    public override void OnStateExited(Sc_AIBrain brain)
    {
        _investigationPriority = -1;
        NoticedSmthGO.SetActive(false);
        InvestigatingGO.SetActive(false);

        StopNoticingSomething(brain);
        StopInvestigatingSomething(brain);
    }

    public override Sc_State Tick(Sc_AIBrain brain)
    {
        //Move to investigation point
        //If reached destination, investigate for a moment.
        //If investigation ended, return to idle.
        //If see/hear anything new at any point, NoticedSomething();
            //Constantly check against the current level's "Memory" to see if an object has been displaced or activated. If this happens, set new memory to current state.
        //If see/hear player, enter pursue state.

        if (brain.MoveTo(InvestigationPoint) && !IsInvestigating && !IsNoticing)
        {
            InvestigateSomething(brain, InvestigationPoint);
        }

        return base.Tick(brain);
    }

    public override void OnHearSomething(Sc_AIBrain brain, Sc_SoundStimuli sstimuli)
    {
        //Debug.Log("I heard something!");
        if (InvestigationVisual != null) return;
        if (CheckPriority(sstimuli.Priority) || _cachedSoundSource == sstimuli.Source)
        {
            _investigationPriority = sstimuli.Priority;
            NoticedSound(brain, sstimuli);
        }
    }

    public override void OnSawSomething(Sc_AIBrain brain, Sc_VisualStimuli vstimuli)
    {
        //Debug.Log("I saw something!");
        if (CheckPriority(vstimuli.Priority))
        {
            _investigationPriority = vstimuli.Priority;
            NoticedSight(brain, vstimuli);
        }
    }

    public override void OnAwarenessThresholdReached(Sc_AIBrain brain, Sc_Character_Player breachingPlayer)
    {
        PursueState.CurrentPlayerTarget = breachingPlayer;
        brain.GoToState(PursueState);
    }

    private bool CheckPriority(int stimuliPriority)
    {
        bool higherPrio = false;
        if (stimuliPriority > _investigationPriority) higherPrio = true;

        return higherPrio;
    }

    #region Noticing
    private IEnumerator NoticedSomethingCoroutine(Sc_AIBrain brain, bool useTransform = false)
    {
        float timer = 0f;
        while (timer < NoticingDuration)
        {
            if (useTransform)
            {
                if(InvestigationVisual != null && CanSeeVisualStimuli(brain, InvestigationVisual))
                {
                    InvestigationPoint = InvestigationVisual.transform.position;
                    brain.Controller.LookAt(InvestigationPoint);
                    if (InvestigationVisual.transform.position != _cachedTransformPos)
                    {
                        timer -= Time.deltaTime;
                    }
                }
                else if(_cachedSoundSource != null)
                {
                    InvestigationPoint = _cachedSoundSource.transform.position;
                    brain.Controller.LookAt(InvestigationPoint);
                }
            }
            else
            {
                brain.Controller.LookAt(InvestigationPoint);
            }
            
            timer += Time.deltaTime;
            if (useTransform && InvestigationVisual != null)
            {
                _cachedTransformPos = InvestigationVisual.transform.position;
            }
            yield return null;
        }
        _investigationPriority = -1;
        StopNoticingSomething(brain);
    }

    public void StopNoticingSomething(Sc_AIBrain brain)
    {
        brain.Controller.CanMove = true;
        brain.Controller.StopLookAt();
        NoticedSmthGO.SetActive(false);

        InvestigationVisual = null;
        InvestigationSound = null;
        _cachedSoundSource = null;

        if (_noticedSomethingCO != null)
        {
            StopCoroutine(_noticedSomethingCO);
            _noticedSomethingCO = null;
        }
    }

    public void NoticedSight(Sc_AIBrain brain, Sc_VisualStimuli vstimuli)
    {
        StopInvestigatingSomething(brain);

        brain.Controller.CanMove = false;
        NoticedSmthGO.SetActive(true);
        InvestigationVisual = vstimuli;
        _cachedTransformPos = vstimuli.transform.position;
        InvestigationPoint = vstimuli.transform.position;
        if (_noticedSomethingCO != null)
        {
            StopCoroutine(_noticedSomethingCO);
        }
        _noticedSomethingCO = StartCoroutine(NoticedSomethingCoroutine(brain, true));
    }

    public void NoticedSound(Sc_AIBrain brain, Sc_SoundStimuli sstimuli)
    {
        StopInvestigatingSomething(brain);

        brain.Controller.CanMove = false;
        NoticedSmthGO.SetActive(true);
        InvestigationPoint = sstimuli.transform.position;

        if (_cachedSoundSource == null)
        {
            InvestigationSound = sstimuli;
            _cachedSoundSource = sstimuli.Source;

            if (_noticedSomethingCO != null)
            {
                StopCoroutine(_noticedSomethingCO);
            }
            _noticedSomethingCO = StartCoroutine(NoticedSomethingCoroutine(brain, true));
        }
        else
        {
            if (sstimuli.Source == _cachedSoundSource)
            {

            }
            else
            {
                InvestigationSound = sstimuli;
                _cachedSoundSource = sstimuli.Source;

                if (_noticedSomethingCO != null)
                {
                    StopCoroutine(_noticedSomethingCO);
                }
                _noticedSomethingCO = StartCoroutine(NoticedSomethingCoroutine(brain));
            }
        }
    }

    private bool CanSeeVisualStimuli(Sc_AIBrain brain, Sc_VisualStimuli vstimuli)
    {
        if (vstimuli == null) return false;
        return brain.Sight.CanSee(vstimuli);
    }

    #endregion
    #region Investigation
    public void InvestigateSomething(Sc_AIBrain brain, Vector3 investigatePoint)
    {
        brain.Controller.CanMove = false;
        InvestigatingGO.SetActive(true);
        InvestigationPoint = investigatePoint;

        InvestigationVisual = null;
        InvestigationSound = null;
        _cachedSoundSource = null;

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
