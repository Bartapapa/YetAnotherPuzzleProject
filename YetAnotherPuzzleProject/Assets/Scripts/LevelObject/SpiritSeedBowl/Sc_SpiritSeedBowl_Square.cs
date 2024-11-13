using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_SpiritSeedBowl_Square : Sc_SpiritSeedBowl
{
    [Header("SQUARE OBJECT REFERENCES")]
    public GameObject Vine;

    public override void Bloom()
    {
        base.Bloom();
        SeedMesh.SetActive(false);
        Vine.SetActive(true);
    }
}
