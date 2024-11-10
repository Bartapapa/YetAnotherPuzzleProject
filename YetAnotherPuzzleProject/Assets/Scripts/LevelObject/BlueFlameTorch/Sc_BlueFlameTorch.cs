using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_BlueFlameTorch : Sc_Activator
{
    [Header("BLUE FLAME TORCH OBJECT REFS")]
    public Sc_Interactible Interactible;

    [Header("BLUE FLAME ROOT")]
    public Transform BlueFlameRoot;

    public void OnInteractedWith(Sc_Character interactor)
    {
        Interactible.CanBeInteractedWith = false;
        DelayActivation();
    }
}
