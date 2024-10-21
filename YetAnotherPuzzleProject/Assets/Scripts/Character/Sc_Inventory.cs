using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Inventory : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Transform _itemHoldAnchor;

    [Header("ITEMS")]
    public Sc_Item[] _items = new Sc_Item[2];
    [ReadOnly] public Sc_Item _currentlyHeldItem;
    private int _maximumItemCount = 3;
    public int MaximumItemCount { get { return _maximumItemCount; } }

    public void PickUpItem(Sc_Item item)
    {
        if (Store(item))
        {

        }
        else
        {
            //Can't pick up item.
        }
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

        item.gameObject.SetActive(true);
        item._interactible.CanBeInteractedWith = true;
        item.transform.parent = null;
        item.transform.position = this.transform.position;
        item.transform.rotation = this.transform.rotation;

        _items[GetIndexFromItem(item)] = null;
        item._inInventory = null;
    }

    private int GetIndexFromItem(Sc_Item item)
    {
        int index = -1;
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == item)
            {
                index = i;
            }
        }
        return index;
    }

    private int GetNextItemIndex()
    {
        int index = -1;
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public void DropCurrentItem()
    {
        if (_currentlyHeldItem == null) return;
        else
        {
            Sc_Item itemToDrop = _currentlyHeldItem;
            _currentlyHeldItem = null;

            DropItem(itemToDrop);
        }
    }
    public bool Store(Sc_Item item)
    {
        bool canStore = false;
        if (IsCurrentlyStoredItem(item))
        {
            canStore = true;
        }
        else
        {
            canStore = _items.Length + 1 > _maximumItemCount;
        }

        if (canStore)
        {
            item._interactible.CanBeInteractedWith = false;

            _items[GetNextItemIndex()] = item;
            item._inInventory = this;
            item.gameObject.SetActive(false);
        }
        
        return canStore;
    }

    private bool IsCurrentlyStoredItem(Sc_Item item)
    {
        return GetIndexFromItem(item) != -1;
    }

    public void Equip(Sc_Item item)
    {
        _currentlyHeldItem = item;

        item.gameObject.SetActive(true);
        item.transform.parent = _itemHoldAnchor;
        item.transform.position = _itemHoldAnchor.position;
        item.transform.rotation = _itemHoldAnchor.rotation;
    }

    public void EquipFromInventory(int index)
    {
        if (index < 0 || index > _items.Length)
        {
            return;
        }
        else
        {

        }
    }

}
