using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Interactible _interactible;
    [ReadOnly] public Sc_Inventory _inInventory;
    private Rigidbody _rb;
    private Collider _coll;

    [Header("ITEM DATA")]
    public SO_ItemData _itemData;

    public bool IsBeingThrown { get { return _thrownCo != null; } }
    private Coroutine _thrownCo;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
    }

    public virtual void OnInteractedWith(Sc_Character interactor)
    {
        Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
        if (player)
        {
            if (player.Inventory._currentlyHeldItem != null)
            {
                player.Inventory.PickUpItem(this);
            }
            else
            {
                player.Inventory.PickUpItemAndEquip(this);
            }          
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

    public virtual void ThrowItem()
    {
        _interactible.CanBeInteractedWith = false;
        _rb.isKinematic = false;
        _coll.isTrigger = false;
        //Take rigidbody, make is not kinematic no more.
        //Start throwing coroutine, wherein the object's velocity is set by an animation curve. It flies in a straight direction before starting to fall.
        //During this coroutine, it constantly checks in the direction of its trajectory with a spheretrace. If it hits anything, it breaks.
        //After a definite amount of time, it is self-destroyed anyhow to prevent it from actually going waaaay away.
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsBeingThrown)
        {
            Destroy(this.gameObject);
        }
    }


}
