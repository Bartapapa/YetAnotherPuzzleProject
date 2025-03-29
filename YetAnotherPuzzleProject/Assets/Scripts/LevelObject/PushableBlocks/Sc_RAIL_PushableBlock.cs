using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Sc_RAIL_PushableBlock : Sc_RoboArmInputListener
{
    [Header("PUSHABLE OBJECT REF")]
    public Sc_Pushable pushable;

    protected override void TranslateChannelInput(ref CharacterInput playerInput)
    {
        //base.TranslateInput(ref playerInput);
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(playerInput.moveX, 0f, playerInput.moveY), 1f);
        float cameraRotation = playerInput.cameraRef.transform.eulerAngles.y;
        Quaternion controlRotation = Quaternion.Euler(0, cameraRotation, 0);
        Vector3 desiredMoveInputVector = controlRotation * moveInputVector;

        float forwardDot = Vector3.Dot(desiredMoveInputVector, transform.forward);
        float rightwardDot = Vector3.Dot(desiredMoveInputVector, transform.right);
        Vector3 pushDirection = Vector3.zero;
        if (forwardDot >= .85f)
        {
            pushDirection = transform.forward;
        }
        else if (forwardDot <= -.85f)
        {
            pushDirection = -transform.forward;
        }
        else if (rightwardDot >= .85f)
        {
            pushDirection = transform.right;
        }
        else if (rightwardDot <= -.85f)
        {
            pushDirection = -transform.right;
        }

        if (pushDirection != Vector3.zero)
        {
            if (!pushable) return;
            pushable.Push(pushDirection);
        }
    }
}
