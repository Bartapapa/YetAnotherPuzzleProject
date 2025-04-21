using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Platform : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        Sc_CharacterController character = other.GetComponent<Sc_CharacterController>();
        if (character)
        {
            //character.ParentToObject(_headParent);
            //if (_parentedControllers.Contains(character))
            //{
            //    return;
            //}
            //_parentedControllers.Add(character);
            Debug.Log("Added Controller to ramblock: " + character.name);
            return;
        }

        Sc_Pushable pushable = other.GetComponent<Sc_Pushable>();
        if (pushable)
        {
            //if (_parentedPushables.Contains(pushable))
            //{
            //    return;
            //}
            //_parentedPushables.Add(pushable);
            Debug.Log("Added Pushable to ramblock: " + pushable.name);
            return;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        Sc_CharacterController character = other.GetComponent<Sc_CharacterController>();
        if (character)
        {
            //_parentedControllers.Remove(character);
            Debug.Log("Removed Controller from ramblock: " + character.name);
        }

        Sc_Pushable pushable = other.GetComponent<Sc_Pushable>();
        if (pushable)
        {
            //_parentedPushables.Remove(pushable);
            Debug.Log("Removed Pushable from ramblock: " + pushable.name);
            return;
        }
    }
}
