using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Quacker : MonoBehaviour
{
    public AudioSource _source;
    public AudioClip _quack;
    public ParticleSystem _duckParticles;

    public void Quack()
    {
        Sc_GameManager.instance.SoundManager.PlaySFX(_source, _quack);
        _duckParticles.Play();
    }
}
