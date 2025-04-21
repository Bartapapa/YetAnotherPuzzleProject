using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Platform_RamBlock : Sc_Platform
{
    [Header("RAMBLOCK OBJECT REF")]
    public Sc_RamBlock RamBlock;

    protected override void OnTriggerEnter(Collider other)
    {
        Sc_CharacterController character = other.GetComponent<Sc_CharacterController>();
        if (character)
        {
            if (RamBlock._parentedControllers.Contains(character))
            {
                return;
            }
            RamBlock._parentedControllers.Add(character);
            Debug.Log("Added Controller to ramblock: " + character.name);
            return;
        }

        Sc_Pushable pushable = other.GetComponent<Sc_Pushable>();
        if (pushable)
        {
            if (RamBlock._parentedPushables.Contains(pushable))
            {
                return;
            }
            RamBlock._parentedPushables.Add(pushable);
            Debug.Log("Added Pushable to ramblock: " + pushable.name);
            return;
        }
    }
    protected override void OnTriggerExit(Collider other)
    {
        Sc_CharacterController character = other.GetComponent<Sc_CharacterController>();
        if (character)
        {
            RamBlock._parentedControllers.Remove(character);
            Debug.Log("Removed Controller from ramblock: " + character.name);
        }

        Sc_Pushable pushable = other.GetComponent<Sc_Pushable>();
        if (pushable)
        {
            RamBlock._parentedPushables.Remove(pushable);
            Debug.Log("Removed Pushable from ramblock: " + pushable.name);
            return;
        }
    }
}
