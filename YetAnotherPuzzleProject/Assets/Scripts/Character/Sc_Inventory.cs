using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class Sc_Inventory : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Character Character;
    public Transform _itemHoldAnchor;
    public Transform _itemThrowPoint;

    [Header("ITEMS")]
    public Sc_Item[] _items = new Sc_Item[2];
    [ReadOnly] public Sc_Item _currentlyHeldItem;
    private int _maximumItemCount = 3;
    public int MaximumItemCount { get { return _maximumItemCount; } }
    public bool IsCurrentlyHoldingItem { get { return _currentlyHeldItem != null; } }

    [Header("THROWING")]
    public LayerMask ThrownObjectCollisionLayers;
    public LineRenderer TrajectoryLine;
    public GameObject ImpactSphere;
    private Vector3 _actualAimDir;
    public bool CanAim { get { return Character.Controller.IsClimbing || Character.Controller.IsAnchoring || Character.Controller.IsAnchoredToValve || !Character.Controller.IsGrounded || _currentlyHeldItem == null? false : true; } }
    [ReadOnly] public bool IsAiming = false;

    private void Update()
    {
        HandleAiming();
    }

    private void HandleAiming()
    {
        if (!IsAiming) return;
        DrawAimingTrajectory(_itemThrowPoint.position, _itemThrowPoint.forward * _currentlyHeldItem._itemData.ThrowForce);
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

    private Sc_Item GetItemFromIndex(int index)
    {
        Sc_Item item = null;
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
            if (item == _currentlyHeldItem)
            {
                _currentlyHeldItem = null;
            }

            item._interactible.CanBeInteractedWith = false;
            item.IsEquipped = false;

            item.gameObject.SetActive(false);
        }
        else
        {
            canStore = CurrentItemCount()+1 <= _maximumItemCount;

            if (canStore)
            {
                item._interactible.CanBeInteractedWith = false;
                item.IsEquipped = false;

                _items[GetNextItemIndex()] = item;
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
        _currentlyHeldItem = item;

        item.gameObject.SetActive(true);
        item.IsEquipped = true;

        item.transform.parent = _itemHoldAnchor;
        item.transform.position = _itemHoldAnchor.position;
        item.transform.rotation = _itemHoldAnchor.rotation;
    }

    public void EquipFromInventory(int index)
    {
        Sc_Item itemToEquip = GetItemFromIndex(index);
        if (_currentlyHeldItem != null)
        {
            if (itemToEquip == _currentlyHeldItem)
            {
                return;
            }
            else
            {
                Store(_currentlyHeldItem);
            }
        }

        if (itemToEquip != null)
        {
            Equip(itemToEquip);
        }
    }

    public void UseCurrentItem()
    {
        if (_currentlyHeldItem == null) return;

        _currentlyHeldItem.UseItem();
    }

    public void ThrowItem(Sc_Item item)
    {
        if (GetIndexFromItem(item) >= 0)
        {
            _currentlyHeldItem = null;
            SetForThrow(item);
            item.ThrowItem(Character, _itemThrowPoint.forward);
        }
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

        _items[GetIndexFromItem(item)] = null;
        item._inInventory = null;
    }

    public void ThrowCurrentItem()
    {
        if (_currentlyHeldItem == null) return;
        ThrowItem(_currentlyHeldItem);
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
