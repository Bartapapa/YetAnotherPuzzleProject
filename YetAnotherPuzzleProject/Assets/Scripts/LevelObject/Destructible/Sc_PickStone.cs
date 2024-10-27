using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_PickStone : Sc_Destructible
{
    [Header("STONE OBJECT REFERENCES")]
    public Sc_Vase Vase;
    public NavMeshObstacle NMObstacle;

    protected override void OnDestroyEvent(Sc_Character character)
    {
        base.OnDestroyEvent(character);
        Vase.CheckVase(character);
        NMObstacle.enabled = false;

        if (Sc_GameManager.instance != null)
        {
            if (Sc_GameManager.instance.CurrentLevel != null)
            {
                Sc_GameManager.instance.CurrentLevel.RequestRebuildNavmesh();
            }
        }
    }
}
