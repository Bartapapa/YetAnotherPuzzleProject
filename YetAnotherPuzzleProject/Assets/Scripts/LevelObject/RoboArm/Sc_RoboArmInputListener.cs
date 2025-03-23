using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_RoboArmInputListener : MonoBehaviour
{
    public void OnRoboArmChannelInput(ref CharacterInput playerInput)
    {
        TranslateInput(ref playerInput);
    }
    protected virtual void TranslateInput(ref CharacterInput playerInput)
    {
        //Apply method when channeling
    }
}
