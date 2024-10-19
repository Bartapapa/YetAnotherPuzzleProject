using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_SoundStimuli : MonoBehaviour
{
    public Sc_Lifespan Lifespan;
    public SphereCollider _soundCollider;

    public delegate void SoundStimuliEvent(Sc_SoundStimuli stimuli);
    public event SoundStimuliEvent OnSoundEnded;


    private void OnEnable()
    {
        if (Lifespan)
        {
            Lifespan.OnLifespanEnd -= OnLifespanEnded;
            Lifespan.OnLifespanEnd += OnLifespanEnded;
        }
    }

    private void OnDisable()
    {
        if (Lifespan)
        {
            Lifespan.OnLifespanEnd -= OnLifespanEnded;
        }
    }

    private void OnDestroy()
    {
        OnSoundEnded?.Invoke(this);
    }

    private void OnLifespanEnded()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        SC_AISense[] hearing = other.GetComponentsInChildren<SC_AISense>();
        if (hearing.Length >= 1)
        {
            for (int i = 0; i < hearing.Length; i++)
            {
                if (hearing[i].Sense == AISenseType.Hearing)
                {
                    hearing[i].HearStimuli(this);
                }
            }
        }
    }
}
