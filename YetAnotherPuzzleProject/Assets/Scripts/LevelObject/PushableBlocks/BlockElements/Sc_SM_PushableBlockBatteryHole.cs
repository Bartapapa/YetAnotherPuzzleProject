using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_SM_PushableBlockBatteryHole : Sc_ShedMesh
{
    [Header("PUSHABLE BLOCK BATTERY HOLE OBJECT REFS")]
    public Sc_BatteryHole BatteryHole;
    public GameObject InteractiveBH;
    public GameObject ShedBH;

    public override void OnMeshShed()
    {
        InteractiveBH.SetActive(false);
        ShedBH.SetActive(true);
        base.OnMeshShed();
        BatteryHole._interactible.CanBeInteractedWith = false;
    }
}
