using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_SpiritSeedBowl_Circular : Sc_SpiritSeedBowl
{
    [Header("BLOOM REFS")]
    public GameObject BloomMesh;

    public override void Bloom()
    {
        base.Bloom();
        SeedMesh.SetActive(false);
        BloomMesh.SetActive(true);
    }
}
