using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_Trumpet_Listener : MonoBehaviour
{
    public UnityEvent OnCodeComplete;
    public UnityEvent<int> OnCodeCorrect;
    public UnityEvent OnCodeFalse;

    [Header("LISTENER PARAMETERS")]
    public List<TrumpetSoundType> TrumpetCode = new List<TrumpetSoundType>();
    [ReadOnly] public List<TrumpetSoundType> CurrentCode = new List<TrumpetSoundType>();

    public delegate void DefaultEvent();
    public DefaultEvent HearTrumpet;

    public void OnHearTrumpet(TrumpetSoundType trumpetSound)
    {
        ParseCode(trumpetSound);
    }

    private void ParseCode(TrumpetSoundType trumpetSound)
    {
        if (TrumpetCode.Count == 0) return;
        else
        {
            CurrentCode.Add(trumpetSound);

            if (CurrentCode.Count == TrumpetCode.Count)
            {
                for (int i = 0; i < CurrentCode.Count; i++)
                {
                    if (CurrentCode[i] != TrumpetCode[i])
                    {
                        OnCodeFalse?.Invoke();
                        CurrentCode.Clear();
                        return;
                    }
                }
                OnCodeComplete?.Invoke();
                CurrentCode.Clear();
            }
            else
            {
                for (int i = 0; i < CurrentCode.Count; i++)
                {
                    if (CurrentCode[i] != TrumpetCode[i])
                    {
                        OnCodeFalse?.Invoke();
                        CurrentCode.Clear();
                        return;
                    }
                }
                OnCodeCorrect?.Invoke(CurrentCode.Count);
            }
        }
    }
}
