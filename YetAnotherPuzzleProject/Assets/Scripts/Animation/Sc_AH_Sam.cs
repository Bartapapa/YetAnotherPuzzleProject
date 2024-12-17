using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AH_Sam : Sc_AnimationHandler
{
    [Header("SAM OBJECT REFERENCES")]
    public Sc_CharacterController Controller;
    public Sc_Inventory Inventory;

    private void Start()
    {
        Controller.OnLanded -= OnCharacterLanded;
        Controller.OnLanded += OnCharacterLanded;

        Controller.OnStartLadderClimb -= OnCharacterStartLadder;
        Controller.OnStartLadderClimb += OnCharacterStartLadder;
        Controller.OnLadderLeave -= OnCharacterLeaveLadder;
        Controller.OnLadderLeave += OnCharacterLeaveLadder;

        Inventory.TreasureFound -= OnTreasureFound;
        Inventory.TreasureFound += OnTreasureFound;
        Inventory.ItemFound -= OnItemFound;
        Inventory.ItemFound += OnItemFound;
    }

    private void Update()
    {
        HandleAnimatorValues();
    }

    private void HandleAnimatorValues()
    {
        bool isGrounded = Controller.IsGrounded;

        bool isClimbing = Controller.IsClimbing;
        if (isClimbing)
        {
            Anim.SetFloat("ClimbingAnimSpeed", Controller.ClimbInputVector.y);
        }

        bool isPushing = Controller.IsPushingBlock;
        float lateralMovement = Mathf.Abs(Vector3.Magnitude(new Vector3(Controller.RB.velocity.x, 0f, Controller.RB.velocity.z)));
        float verticalMovement = Mathf.Clamp(Controller.RB.velocity.y / 10f, -1f, 1f);

        Anim.SetFloat("LateralSpeed", lateralMovement);
        Anim.SetFloat("VerticalSpeed", verticalMovement);
        Anim.SetBool("isGrounded", isGrounded);
        Anim.SetBool("isPushing", isPushing);
        Anim.SetBool("isClimbing", isClimbing);
    }

    private void OnCharacterLanded(Rigidbody rb)
    {
        if (rb.velocity.y >= -4f) return;
        Anim.Play("Land");
    }

    private void OnCharacterStartLadder()
    {
        Anim.Play("Climb_Into");
    }

    private void OnCharacterLeaveLadder(bool top)
    {
        if (top)
        {
            Anim.Play("Climb_Top_Out");
        }
        else
        {
            Anim.Play("Climb_Bottom_Out");
        }  
    }

    private void OnTreasureFound(Sc_Item item)
    {
        SetLayerWeight(1, 1f);
        Anim.Play("FoundTreasure");
        float treasureAppearDuration = 0f;
        switch (item._itemData.Rarity)
        {
            case TreasureRarity.None:
                treasureAppearDuration = 1f;
                break;
            case TreasureRarity.Common:
                treasureAppearDuration = item.CommonTreasureLifetime;
                break;
            case TreasureRarity.Rare:
                treasureAppearDuration = item.RareTreasureLifetime;
                break;
            case TreasureRarity.VeryRare:
                treasureAppearDuration = item.VeryRareTreasureLifetime;
                break;
            default:
                break;
        }
        float waitDuration = .2f + treasureAppearDuration;
        Wait(waitDuration, null,
            () => PlayAnimation("StowItem", 1, null,
                () => SetLayerWeight(1, 0f)));
    }

    private void OnItemFound(Sc_Item item)
    {

    }
}
