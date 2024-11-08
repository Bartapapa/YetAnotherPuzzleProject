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
    public float CodePunchTime = 5f;
    public List<TrumpetSoundType> TrumpetCode = new List<TrumpetSoundType>();
    [ReadOnly] public List<TrumpetSoundType> CurrentCode = new List<TrumpetSoundType>();
    private float _codePunchTimer = 0f;

    public delegate void DefaultEvent();
    public delegate void CharacterEvent(Sc_Character character);
    public DefaultEvent HearTrumpet;
    public CharacterEvent HearCharacterPlayTrumpet;

    private void Update()
    {
        if (CurrentCode.Count >= 1)
        {
            if (_codePunchTimer >= CodePunchTime)
            {
                OnCodeFalse?.Invoke();
                CurrentCode.Clear();
                _codePunchTimer = 0f;
            }
            _codePunchTimer += Time.deltaTime;
        }
        else
        {
            _codePunchTimer = 0f;
        }
    }

    public void OnHearTrumpet(TrumpetSoundType trumpetSound, Sc_Character character = null)
    {
        ParseCode(trumpetSound);
        if (character != null)
        {
            HearCharacterPlayTrumpet?.Invoke(character);
        }
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
                _codePunchTimer = 0f;
            }
        }
    }
}
