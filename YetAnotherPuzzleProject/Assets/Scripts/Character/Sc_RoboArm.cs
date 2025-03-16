using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_RoboArm : MonoBehaviour
{
    [Header("STATE")]
    public bool HasRoboArm = false;

    [Header("LINKED PUSHABLE")]
    public Sc_Pushable LinkedPushable;

    public bool HasLinkedRoboArm { get { return LinkedPushable != null; } }

    public void StartChannel()
    {

    }

    public void StopChannel()
    {

    }
}
