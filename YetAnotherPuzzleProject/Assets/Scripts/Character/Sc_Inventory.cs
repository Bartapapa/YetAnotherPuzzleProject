using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Inventory : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Transform _itemHoldAnchor;

    [Header("ITEMS")]
    [ReadOnly] public List<Sc_Item> _items = new List<Sc_Item>();
    [ReadOnly] public Sc_Item _currentlyHeldItem;

    public void PickUpItem(Sc_Item item)
    {
        item._interactible.CanBeInteractedWith = false;

        _items.Add(item);
        item._inInventory = this;
    }

    public void PickUpItemAndEquip(Sc_Item item)
    {
        PickUpItem(item);
        Equip(item);
    }

    public void DropItem(Sc_Item item)
    {
        if (item == _currentlyHeldItem)
        {
            DropCurrentItem();
            return;
        }

        item._interactible.CanBeInteractedWith = true;
        item.transform.parent = null;
        item.transform.position = this.transform.position;
        item.transform.rotation = this.transform.rotation;

        _items.Remove(item);
        item._inInventory = null;
    }

    public void DropCurrentItem()
    {
        if (_currentlyHeldItem == null) return;
        else
        {
            _currentlyHeldItem = null;

            DropItem(_currentlyHeldItem);
        }
    }
    public void Store(Sc_Item item)
    {
        _currentlyHeldItem = null;

        item.gameObject.SetActive(false);
    }

    public void Equip(Sc_Item item)
    {
        _currentlyHeldItem = item;

        item.gameObject.SetActive(true);
        item.transform.parent = _itemHoldAnchor;
        item.transform.position = _itemHoldAnchor.position;
        item.transform.rotation = _itemHoldAnchor.rotation;
    }

}
