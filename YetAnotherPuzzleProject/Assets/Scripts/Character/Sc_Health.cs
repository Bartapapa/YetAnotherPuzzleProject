using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Health : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public ParticleSystem DeathParticles;

    [Header("STATE")]
    [ReadOnly] public bool Dead = false;
    public void Death()
    {
        DeathParticles.Play();
        Dead = true;
    }
}
