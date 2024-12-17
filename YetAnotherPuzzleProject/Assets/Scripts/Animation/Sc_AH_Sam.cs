using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AH_Sam : Sc_AnimationHandler
{
    [Header("SAM OBJECT REFERENCES")]
    public Sc_CharacterController Controller;

    private void Start()
    {
        Controller.OnLanded -= OnCharacterLanded;
        Controller.OnLanded += OnCharacterLanded;
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
            Anim.SetFloat("ClimbingAnimSpeed", Mathf.Sign(Controller.ClimbInputVector.y));
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
}
