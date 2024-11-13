using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_SpiritSeedBowl : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Interactible Interactible;
    public GameObject SeedMesh;

    [Header("PARAMETERS")]
    [ReadOnly] public bool HasBeenPlanted = false;

    private void Start()
    {
        SeedMesh.SetActive(false);
    }

    public void PlantSeed()
    {
        Interactible.CanBeInteractedWith = false;
        HasBeenPlanted = true;
        SeedMesh.SetActive(true);
    }

    public virtual void Bloom()
    {
        Interactible.CanBeInteractedWith = false;
        HasBeenPlanted = true;
    }
}
