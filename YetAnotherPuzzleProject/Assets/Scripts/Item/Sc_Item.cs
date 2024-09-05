using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Interactible _interactible;
    [ReadOnly] public Sc_Inventory _inInventory;

    [Header("ITEM DATA")]
    public SO_ItemData _itemData;

    public virtual void OnInteractedWith(Sc_Character interactor)
    {
        Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
        if (player)
        {
            player.Inventory.PickUpItemAndEquip(this);
        }
    }

    public virtual void UseItem()
    {
        if (_inInventory != null)
        {
            _inInventory.DropItem(this);
        }
        Destroy(this.gameObject);
    }
}
