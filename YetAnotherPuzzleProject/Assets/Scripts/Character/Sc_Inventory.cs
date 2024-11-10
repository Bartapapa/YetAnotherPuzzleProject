using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Inventory : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Character Character;
    public Transform _itemHoldAnchor;
    public Transform _itemThrowPoint;

    [Header("ITEMS")]
    public Sc_Item[] _items = new Sc_Item[2];
    [ReadOnly] public Sc_Item CurrentlyHeldItem;
    private int _maximumItemCount = 3;
    public int MaximumItemCount { get { return _maximumItemCount; } }
    public bool IsCurrentlyHoldingItem { get { return CurrentlyHeldItem != null; } }
    public bool CanUseItem { get { return Character.Controller.IsClimbing || Character.Controller.IsAnchoring || Character.Controller.IsAnchoredToValve || !Character.Controller.IsGrounded || IsAiming || CurrentlyHeldItem == null ? false : true; } }
    [ReadOnly] public bool IsUsingItem = false;

    [Header("THROWING")]
    public LayerMask ThrownObjectCollisionLayers;
    public LineRenderer TrajectoryLine;
    public GameObject ImpactSphere;
    private Vector3 _actualAimDir;
    public bool CanAim { get { return IsUsingItem || Character.Controller.IsClimbing || Character.Controller.IsAnchoring || Character.Controller.IsAnchoredToValve || !Character.Controller.IsGrounded || CurrentlyHeldItem == null? false : true; } }
    [ReadOnly] public bool IsAiming = false;

    public delegate void ItemEvent(Sc_Item item);
    public ItemEvent ItemThrown;
    public ItemEvent ItemDropped;
    public ItemEvent ItemPickedUp;

    private void Update()
    {
        HandleAiming();
    }

    public void ResetInventory()
    {
        CurrentlyHeldItem = null;

        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] != null)
            {
                Destroy(_items[i].gameObject);
                _items[i] = null;
            }
        }
    }

    public void PopulateInventory(int[] inventoryItemIDs, int currentlyHeldItemIndex)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (inventoryItemIDs[i] >= 0)
            {
                Sc_Item newItem = Instantiate<Sc_Item>(
                    Sc_GameManager.instance.TreasureManager.ItemDatabase.GetItemFromID(inventoryItemIDs[i]),
                    _itemHoldAnchor);
                Store(newItem, i);
            }
        }
        if (currentlyHeldItemIndex >= 0)
        {
            if (currentlyHeldItemIndex == 3)
            {
                //CurrentlyHeldItem isn't kept in one's storage, such as a Blue Flame. Instantiate it, then equip it.
                Debug.LogWarning("Couldn't instantiate blue flame or other object not in inventory!");
            }
            else
            {
                Equip(_items[currentlyHeldItemIndex]);
            }          
        }
    }

    public int GetInventoryIndexOfCurrentlyHeldItem()
    {
        if (CurrentlyHeldItem != null)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (CurrentlyHeldItem == _items[i])
                {
                    return i;
                }
            }
            return 3;
        }
        return -1;
    }

    private void HandleAiming()
    {
        if (!IsAiming) return;
        DrawAimingTrajectory(_itemThrowPoint.position, _itemThrowPoint.forward * CurrentlyHeldItem._itemData.ThrowForce);
        //DrawTrajectory(_itemThrowPoint.position, _currentlyHeldItem._itemData.ThrowForce);
    }

    private void DrawAimingTrajectory(Vector3 initialPos, Vector3 initialVel)
    {
        Vector3 pos = initialPos;
        Vector3 vel = initialVel;
        Vector3 gravity = Physics.gravity;
        Vector3 aimDirection = initialVel;

        for (int i = 0; i < TrajectoryLine.positionCount; i++)
        {
            TrajectoryLine.SetPosition(i, pos);

            if (MakesContact(pos))
            {
                ImpactSphere.SetActive(true);
                ImpactSphere.transform.position = pos;

                for (int j = i; j < TrajectoryLine.positionCount; j++)
                {
                    TrajectoryLine.SetPosition(j, pos);
                }
                break;
            }
            else
            {
                ImpactSphere.SetActive(false);
            }

            vel = vel + gravity * Time.fixedDeltaTime;
            pos = pos + vel * Time.fixedDeltaTime;
        }
    }

    private bool MakesContact(Vector3 position)
    {
        return Physics.OverlapSphere(position, .3f, ThrownObjectCollisionLayers, QueryTriggerInteraction.Ignore).Length > 0;
    }

    private Sc_Interactible GetClosestContactedInteractible(Vector3 position, float contactAffectRange)
    {
        List<Sc_Interactible> contactedInteractibles = new List<Sc_Interactible>();
        Sc_Interactible chosenInteractible = null;

        Collider[] colls = Physics.OverlapSphere(position, contactAffectRange, ThrownObjectCollisionLayers);
        foreach(Collider coll in colls)
        {
            Sc_Interactible interactible = coll.GetComponent<Sc_Interactible>();
            if (interactible)
            {
                if (!contactedInteractibles.Contains(interactible))
                {
                    contactedInteractibles.Add(interactible);
                }
            }
        }

        float closestDistance = float.MaxValue;
        foreach(Sc_Interactible interactible in contactedInteractibles)
        {
            float distance = Vector3.Distance(interactible.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                chosenInteractible = interactible;
            }
        }

        return chosenInteractible;
    }

    private Sc_Character GetClosestContactedCharacter(Vector3 position, float contactAffectRange)
    {
        List<Sc_Character> contactedCharacters = new List<Sc_Character>();
        Sc_Character chosenCharacter = null;

        Collider[] colls = Physics.OverlapSphere(position, contactAffectRange, ThrownObjectCollisionLayers);
        foreach (Collider coll in colls)
        {
            Sc_Character character = coll.GetComponent<Sc_Character>();
            if (character)
            {
                if (!contactedCharacters.Contains(character))
                {
                    contactedCharacters.Add(character);
                }
            }
        }

        float closestDistance = float.MaxValue;
        foreach (Sc_Character character in contactedCharacters)
        {
            float distance = Vector3.Distance(character.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                chosenCharacter = character;
            }
        }

        return chosenCharacter;
    }

    public void PickUpItem(Sc_Item item)
    {
        if (Store(item))
        {
            ItemPickedUp?.Invoke(item);
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
        if (item == CurrentlyHeldItem)
        {
            DropCurrentItem();
            return;
        }

        item.IsEquipped = false;
        item.gameObject.SetActive(true);
        item._interactible.CanBeInteractedWith = true;
        if (Sc_Level.instance != null)
        {
            item.transform.parent = Sc_Level.instance.transform;
        }
        else
        {
            item.transform.parent = null;
        }      
        item.transform.position = this.transform.position;
        item.transform.rotation = this.transform.rotation;

        item.OnItemDrop();
        ItemDropped?.Invoke(item);

        if (GetIndexFromItem(item) >= 0)
        {
            _items[GetIndexFromItem(item)] = null;
        }
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

    private Sc_Item GetItemFromIndex(int index)
    {
        if (index < 0 || index > _items.Length)
        {
            return null;
        }
        else
        {
            return _items[index];
        }
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
        if (CurrentlyHeldItem == null) return;
        else
        {
            Sc_Item itemToDrop = CurrentlyHeldItem;
            itemToDrop.StopUsingItem();
            CurrentlyHeldItem = null;
            
            DropItem(itemToDrop);
        }
    }
    public bool Store(Sc_Item item, int forceIndex = -1)
    {
        bool canStore = false;
        if (IsCurrentlyStoredItem(item))
        {
            canStore = true;
            if (item == CurrentlyHeldItem)
            {
                CurrentlyHeldItem.StopUsingItem();
                CurrentlyHeldItem = null;               
            }
            if (!item.OnBeforeItemStore()) return false;
            item.OnItemStore();

            item._interactible.CanBeInteractedWith = false;
            item.IsEquipped = false;

            item.gameObject.SetActive(false);
        }
        else
        {
            canStore = CurrentItemCount()+1 <= _maximumItemCount;

            if (canStore)
            {
                if (!item.OnBeforeItemStore()) return false;
                item.OnItemStore();

                item._interactible.CanBeInteractedWith = false;
                item.IsEquipped = false;

                if (forceIndex >= 0 && forceIndex < _items.Length)
                {
                    _items[forceIndex] = item;
                }
                else
                {
                    _items[GetNextItemIndex()] = item;
                }

                item._inInventory = this;
                item.transform.parent = _itemHoldAnchor;
                item.transform.position = _itemHoldAnchor.position;
                item.transform.rotation = _itemHoldAnchor.rotation;
                item.gameObject.SetActive(false);
            }
        }
      
        return canStore;
    }

    private int CurrentItemCount()
    {
        int currentItemCount = 0;
        foreach(Sc_Item item in _items)
        {
            if (item != null)
            {
                currentItemCount++;
            }
        }
        return currentItemCount;
    }

    private bool IsCurrentlyStoredItem(Sc_Item item)
    {
        return GetIndexFromItem(item) != -1;
    }

    public void Equip(Sc_Item item)
    {
        CurrentlyHeldItem = item;

        item.gameObject.SetActive(true);
        item.IsEquipped = true;

        item.transform.parent = _itemHoldAnchor;
        item.transform.position = _itemHoldAnchor.position;
        item.transform.rotation = _itemHoldAnchor.rotation;

        item.OnItemEquip();
    }

    public void EquipFromInventory(int index)
    {
        Sc_Item itemToEquip = GetItemFromIndex(index);
        if (CurrentlyHeldItem != null)
        {
            if (itemToEquip == CurrentlyHeldItem)
            {
                return;
            }
            else
            {
                Store(CurrentlyHeldItem);
            }
        }

        if (itemToEquip != null)
        {
            Equip(itemToEquip);
        }
    }

    public void UseCurrentItem()
    {
        if (!CanUseItem) return;

        CurrentlyHeldItem.UseItem();
    }

    public void ThrowItem(Sc_Item item)
    {
        SetForThrow(item);
        item.ThrowItem(Character, _itemThrowPoint.forward);
        ItemThrown?.Invoke(item);

        CurrentlyHeldItem = null;

        //if (GetIndexFromItem(item) >= 0)
        //{

        //}
    }

    private void SetForThrow(Sc_Item item)
    {
        item.gameObject.SetActive(true);
        item._interactible.CanBeInteractedWith = false;
        item.IsEquipped = false;

        if (Sc_Level.instance != null)
        {
            item.transform.parent = Sc_Level.instance.transform;
        }
        else
        {
            item.transform.parent = null;
        }
        item.transform.position = _itemThrowPoint.position;
        item.transform.rotation = _itemThrowPoint.rotation;

        if (GetIndexFromItem(item) >= 0)
        {
            _items[GetIndexFromItem(item)] = null;
        }
        item._inInventory = null;
    }

    public void ThrowCurrentItem()
    {
        if (CurrentlyHeldItem == null) return;
        ThrowItem(CurrentlyHeldItem);
    }

    public void AimThrow()
    {
        if (!CanAim) return;
        IsAiming = true;
        TrajectoryLine.enabled = true;
        Character.Controller.CanMove = false;
    }

    public void StopAiming()
    {
        IsAiming = false;
        TrajectoryLine.enabled = false;
        ImpactSphere.SetActive(false);
        Character.Controller.CanMove = true;
    }

}
