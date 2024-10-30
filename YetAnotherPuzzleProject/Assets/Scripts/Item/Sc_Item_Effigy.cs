using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item_Effigy : Sc_Item
{
    [Header("EFFIGY OBJECT REFERENCES")]
    public Sc_WeightedObject WeightedObject;
    public CinemachineImpulseSource ImpulseSource;
    public ParticleSystem Dust;

    [Header("GROUND MASK")]
    public LayerMask Ground;

    private void FixedUpdate()
    {
        if (IsBeingThrown)
        {
            if (HasLanded())
            {
                OnItemDrop();
                OnEffigyLand();
            }
        }
    }

    private bool HasLanded()
    {
        return Physics.Raycast(transform.position, Vector3.down, .1f, Ground, QueryTriggerInteraction.Ignore);
    }

    public override void UseItem()
    {
        WeightedObject.StateChange();
    }

    public override void OnItemPickup()
    {
        WeightedObject.StateChange();
    }

    public override void OnItemDrop()
    {
        WeightedObject.StateChange();
        GroundShake();
    }

    private void OnEffigyLand()
    {
        _interactible.CanBeInteractedWith = true;
        _rb.isKinematic = true;
        _rb.useGravity = true;
        _coll.isTrigger = true;

        IsBeingThrown = false;

        SpreadContact();
        GroundShake();
    }

    private void GroundShake()
    {
        if (Sc_CameraManager.instance != null)
        {
            Sc_CameraManager.instance.CameraShake(ImpulseSource, .05f);
        }
        if (Dust)
        {
            Dust.Play();
        }
    }

    public override void ThrowItem(Sc_Character throwingCharacter, Vector3 throwDirection)
    {
        //base.ThrowItem(throwingCharacter, throwDirection);
        _interactible.CanBeInteractedWith = false;
        _rb.isKinematic = false;
        _rb.useGravity = true;
        _coll.isTrigger = false;

        Vector3 throwDir = throwDirection * _itemData.ThrowForce;
        _rb.AddForce(throwDir, ForceMode.Impulse);

        IsBeingThrown = true;
        _thrownByCharacter = throwingCharacter;
    }

    protected override void OnCollisionEnter(Collision collision)
    {

    }
}
