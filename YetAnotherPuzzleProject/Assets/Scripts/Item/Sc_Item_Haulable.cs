using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item_Haulable : Sc_Item
{
    [Header("GROUND")]
    public LayerMask Ground;
    protected bool _grounded = true;

    private void FixedUpdate()
    {
        if (RB.isKinematic) return;

        if (!_grounded && IsGrounded())
        {
            OnLanded();
        }

        _grounded = IsGrounded();
    }

    private bool IsGrounded()
    {
        bool isGrounded = Physics.Raycast(transform.position + (Vector3.up*.1f), Vector3.down, .4f, Ground, QueryTriggerInteraction.Ignore);
        return isGrounded;
    }

    protected virtual void OnLanded()
    {
        //Base landing logic;
    }

    public override void OnInteractedWith(Sc_Character interactor)
    {
        Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
        if (player)
        {
            RB.isKinematic = true;
            RB.useGravity = false;
            RB.interpolation = RigidbodyInterpolation.None;
            RB.collisionDetectionMode = CollisionDetectionMode.Discrete;
            player.Inventory.HaulItem(this);
        }
    }

    public override void OnEndInteraction(Sc_Character interactor)
    {
        Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
        if (player)
        {
            RB.isKinematic = false;
            RB.useGravity = true;
            RB.interpolation = RigidbodyInterpolation.Interpolate;
            RB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            _grounded = false;
            player.Inventory.DropCurrentlyHauledItem();
        }
    }
}
