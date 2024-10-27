using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item_Battery : Sc_Item
{
    public override void UseItem()
    {
        
    }

    public override bool UseItemAsKey()
    {
        if (_inInventory != null)
        {
            _inInventory.DropItem(this);
        }
        Destroy(this.gameObject);

        return true;
    }
}
