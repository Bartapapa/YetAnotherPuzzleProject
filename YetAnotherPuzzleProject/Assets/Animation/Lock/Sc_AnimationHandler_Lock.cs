using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AnimationHandler_Lock : MonoBehaviour
{
    [Header("Object References")]
    public ParticleSystem Sparks;

    #region ANIMATIONEVENTS
    public void ProduceSparks()
    {
        Sparks.Play();
    }
    #endregion
}
