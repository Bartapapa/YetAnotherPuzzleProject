using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item_SpiritSeed : Sc_Item
{
    public override void UseItem()
    {

    }

    public override bool UseItemAsKey(Sc_Interactible lockedInteractible)
    {
        if (_inInventory != null)
        {
            _inInventory.DropItem(this);
        }
        Destroy(this.gameObject);

        return true;
    }
}
