using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item_Pick : Sc_Item
{
    [Header("PICK PARAMETERS")]
    public float AffectRadius = 1f;
    public float ForwardOffset = 1f;

    public override void UseItem()
    {
        Vector3 hitPoint = _inInventory.transform.position + (_inInventory.transform.forward * ForwardOffset);
        Collider[] coll = Physics.OverlapSphere(hitPoint, AffectRadius);
        foreach(Collider collider in coll)
        {
            Sc_Destructible destructible = collider.GetComponent<Sc_Destructible>();
            if (destructible)
            {
                if (!destructible.Destroyed)
                {
                    if (destructible.DestroyedByPick)
                    {
                        destructible.DestructibleDestroy(_inInventory.Character);
                    }
                }
            }
        }
    }
}
