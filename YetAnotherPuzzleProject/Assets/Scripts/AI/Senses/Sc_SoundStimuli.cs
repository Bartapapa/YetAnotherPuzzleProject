using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_SoundStimuli : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Lifespan Lifespan;
    public SphereCollider SoundCollider;
    public GameObject Source;
    public Sc_Character_Player Player;

    [Header("PARAMETERS")]
    public int Priority = 10;

    public delegate void SoundStimuliEvent(Sc_SoundStimuli stimuli);
    public event SoundStimuliEvent OnSoundEnded;


    public void InitSoundStimuli(GameObject source, float range = .5f, float duration = 1f, Sc_Character_Player player = null)
    {
        Source = source;

        if (duration > 0)
        {
            Lifespan.Duration = duration;
        }
        else
        {
            Lifespan.Duration = -1f;
        }

        if (range > .1f)
        {
            SoundCollider.radius = range;
        }
        else
        {
            SoundCollider.radius = .1f;
        }

        Player = player;
    }

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
                    //hearing[i].HearStimuli(this);
                }
            }
        }
    }
}
