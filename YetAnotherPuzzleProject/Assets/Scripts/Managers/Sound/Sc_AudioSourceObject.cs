using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AudioSourceObject : MonoBehaviour
{
    public Sc_Lifespan LifeSpan;
    public AudioSource Source;

    private void OnEnable()
    {
        LifeSpan.OnLifespanEnd -= OnLifeSpanEnded;
        LifeSpan.OnLifespanEnd += OnLifeSpanEnded;
    }

    private void OnLifeSpanEnded()
    {
        Destroy(this.gameObject);
    }
}
