using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Sc_Item_BlueFlame : Sc_Item
{
    [Header("BOWL")]
    public Transform BowlAnchor;

    [Header("RETURN")]
    public AnimationCurve HeightCurve;
    public float HeightApex = 2f;
    public float ReturnDuration = 3f;

    [Header("TORCH")]
    public AnimationCurve TorchHeightCurve;
    public float TorchHeightApex = 1f;
    public float MoveToDuration = 1f;
    [ReadOnly] public Sc_BlueFlameTorch BlueFlameTorch;

    private Coroutine _returnToBowlCo = null;
    private Coroutine _moveToTorchCo = null;

    public override void OnInteractedWith(Sc_Character interactor)
    {
        //Don't store and equip as per usual
        //If character has already equipped object, then store it
        //Otherwise, just equip this item without storing

        Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
        if (player)
        {
            if (player.Inventory.IsCurrentlyHoldingItem)
            {
                player.Inventory.Store(player.Inventory.CurrentlyHeldItem);
            }
            _inInventory = player.Inventory;
            player.Inventory.Equip(this);
            _interactible.CanBeInteractedWith = false;
        }
    }

    public override void UseItem()
    {
        //Do nothing
    }

    public override bool UseItemAsKey(Sc_Interactible lockedInteractible)
    {
        //Move flame to position, make uninteractible.
        Sc_BlueFlameTorch blueFlameTorch = lockedInteractible.GetComponent<Sc_BlueFlameTorch>();
        if (!blueFlameTorch) return false;
        _interactible.CanBeInteractedWith = false;
        MoveToBlueFlameTorch(blueFlameTorch);
        return true;
    }

    public override void ThrowItem(Sc_Character throwingCharacter, Vector3 throwDirection)
    {
        //Flame goes back to origin spot.
        ReturnToBowl();
    }

    public override void OnItemDrop()
    {
        base.OnItemDrop();
        ReturnToBowl();
        //Flame goes back to origin spot.
    }

    public override bool OnBeforeItemStore()
    {
        ReturnToBowl();
        return false;
        //Flame goes back to origin spot. Flame needs to be gameobject active for this to work, and set gameobject to inactive happens after this call. Find a workaround.
    }

    private void ReturnToBowl()
    {
        if (_returnToBowlCo != null)
        {
            StopCoroutine(_returnToBowlCo);
            _returnToBowlCo = null;
        }
        _returnToBowlCo = StartCoroutine(ReturnToBowlCo());
        if (_inInventory)
        {
            if (_inInventory.CurrentlyHeldItem == this)
            {
                _inInventory.CurrentlyHeldItem = null;
            }
        }
        _inInventory = null;
        IsEquipped = false;
    }

    private void MoveToBlueFlameTorch(Sc_BlueFlameTorch torch)
    {
        if (torch == null) return;
        BlueFlameTorch = torch;
        if (_moveToTorchCo != null)
        {
            StopCoroutine(_moveToTorchCo);
            _moveToTorchCo = null;
        }
        _moveToTorchCo = StartCoroutine(MoveToBlueFlameTorchCo());
        if (_inInventory)
        {
            if (_inInventory.CurrentlyHeldItem == this)
            {
                _inInventory.CurrentlyHeldItem = null;
            }
        }
        _inInventory = null;
        IsEquipped = false;
    }

    private IEnumerator ReturnToBowlCo()
    {
        _interactible.CanBeInteractedWith = false;

        if (Sc_Level.instance != null)
        {
            transform.parent = Sc_Level.instance.transform;
        }
        else
        {
            transform.parent = null;
        }

        float timer = 0f;
        float fromHeight = transform.position.y;
        float destinationHeight = BowlAnchor.position.y;
        Vector3 fromPos = transform.position;
        Vector3 destinationPos = BowlAnchor.position;
        Quaternion fromRot = transform.rotation;
        Quaternion destinationRot = BowlAnchor.rotation;
        Vector3 toPos = Vector3.zero;
        Quaternion toRot = Quaternion.identity;
        float heightAdjust = 0f;
        while (timer <= ReturnDuration)
        {
            heightAdjust = HeightCurve.Evaluate(timer / ReturnDuration) * HeightApex;
            toPos = Vector3.Lerp(fromPos, destinationPos, timer / ReturnDuration);
            toPos = toPos + new Vector3(0f, heightAdjust, 0f);
            toRot = Quaternion.Slerp(fromRot, destinationRot, timer / ReturnDuration);
            transform.position = toPos;
            transform.rotation = toRot;
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = destinationPos;
        transform.rotation = destinationRot;

        _interactible.CanBeInteractedWith = true;
        _returnToBowlCo = null;
    }

    private IEnumerator MoveToBlueFlameTorchCo()
    {
        _interactible.CanBeInteractedWith = false;

        if (BlueFlameTorch != null)
        {
            transform.parent = BlueFlameTorch.BlueFlameRoot;
        }
        else if (Sc_Level.instance != null)
        {
            transform.parent = Sc_Level.instance.transform;
        }
        else
        {
            transform.parent = null;
        }

        float timer = 0f;
        float fromHeight = transform.position.y;
        float destinationHeight = BlueFlameTorch.BlueFlameRoot.position.y;
        Vector3 fromPos = transform.position;
        Vector3 destinationPos = BlueFlameTorch.BlueFlameRoot.position;
        Quaternion fromRot = transform.rotation;
        Quaternion destinationRot = BlueFlameTorch.BlueFlameRoot.rotation;
        Vector3 toPos = Vector3.zero;
        Quaternion toRot = Quaternion.identity;
        float heightAdjust = 0f;
        while (timer <= MoveToDuration)
        {
            heightAdjust = TorchHeightCurve.Evaluate(timer / MoveToDuration) * TorchHeightApex;
            toPos = Vector3.Lerp(fromPos, destinationPos, timer / MoveToDuration);
            toPos = toPos + new Vector3(0f, heightAdjust, 0f);
            toRot = Quaternion.Slerp(fromRot, destinationRot, timer / MoveToDuration);
            transform.position = toPos;
            transform.rotation = toRot;
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = destinationPos;
        transform.rotation = destinationRot;

        _moveToTorchCo = null;
    }
}
