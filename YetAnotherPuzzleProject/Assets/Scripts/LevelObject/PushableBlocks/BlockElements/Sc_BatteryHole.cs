using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_BatteryHole : Sc_Activator
{
    [Header("BATTERY HOLE OBJECT REFS")]
    public Sc_Interactible _interactible;
    public GameObject _batteryMesh;

    [ReadOnly][SerializeField] public List<Sc_RoboArmInputListener> RoboArmInputListeners = new List<Sc_RoboArmInputListener>();

    private bool _batteryPlaced = false;

    private void Start()
    {
        foreach(Sc_Activateable activateable in Activateables)
        {
            Sc_RoboArmInputListener roboArmListener = activateable.GetComponent<Sc_RoboArmInputListener>();
            if (roboArmListener) RoboArmInputListeners.Add(roboArmListener);
        }
    }

    public void OnInteract(Sc_Character interactor)
    {
        if (!StopDelayedActivation())
        {
            //if (!ToggleActivate())
            //{
            //    return;
            //}
            if (!DelayActivation())
            {
                return;
            }
        }
        PlaceBattery();
    }

    public void OnRoboArmInteract(Sc_Character_Player interactor)
    {
        if (!StopDelayedActivation())
        {
            //if (!ToggleActivate())
            //{
            //    return;
            //}
            if (!DelayActivation())
            {
                return;
            }
        }
        interactor.RoboArm.LinkedRoboArmListeners = RoboArmInputListeners;
        RoboArmEnergize();
    }

    private void PlaceBattery()
    {
        _batteryPlaced = true;
        _interactible.CanBeInteractedWith = false;
        //if (Pushable) Pushable.Energize(true);

        _batteryMesh.SetActive(true);
    }

    private void RoboArmEnergize()
    {
        _interactible.CanBeInteractedWith = false;
        //if (Pushable) Pushable.RoboArmEnergize(true);
    }
}
