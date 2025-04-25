using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_BatteryHole : Sc_Activator
{
    [Header("BATTERY HOLE OBJECT REFS")]
    public Sc_Interactible _interactible;
    public GameObject _batteryMesh;
    public Sc_ShedMesh _shedMesh;
    public List<Transform> _emissiveHoleMeshes = new List<Transform>();

    [ReadOnly][SerializeField] public List<Sc_RoboArmInputListener> RoboArmInputListeners = new List<Sc_RoboArmInputListener>();

    private bool _batteryPlaced = false;

    private void Start()
    {
        foreach(Sc_Activateable activateable in Activateables)
        {
            Sc_RoboArmInputListener[] roboArmListeners = activateable.GetComponents<Sc_RoboArmInputListener>();
            for (int i = 0; i < roboArmListeners.Length; i++)
            {
                RoboArmInputListeners.Add(roboArmListeners[i]);
            }
        }
    }

    public void OnInteract(Sc_Character interactor)
    {
        Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
        if (player)
        {
            if (!StopDelayedActivation())
            {
                if (!DelayActivation())
                {
                    return;
                }
            }
            player.RoboArm.SetRAILS(RoboArmInputListeners);
            RoboArmEnergize();
            player.RoboArm.StartInputingCode();
        }

        //if (!StopDelayedActivation())
        //{
        //    if (!DelayActivation())
        //    {
        //        return;
        //    }
        //}
        //PlaceBattery();

        //Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
        //if (player)
        //{
        //    _interactible.EndInteract(player);
        //}
    }

    public void OnRoboArmInteract(Sc_Character_Player interactor)
    {
        //Put here entire sequence for animation, turning around etc etc;


        //if (!StopDelayedActivation())
        //{
        //    if (!DelayActivation())
        //    {
        //        return;
        //    }
        //}
        //interactor.RoboArm.SetRAILS(RoboArmInputListeners);
        //RoboArmEnergize();
        //interactor.RoboArm.StartInputingCode();
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
