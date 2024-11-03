using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_BatteryHole : Sc_Activateable
{
    [Header("OBJECT REFS")]
    public Sc_Interactible _interactible;
    public Sc_Pushable Pushable;
    public GameObject _batteryMesh;

    private bool _batteryPlaced = false;

    public void OnInteract()
    {
        PlaceBattery();
    }

    private void PlaceBattery()
    {
        _batteryPlaced = true;
        _interactible.CanBeInteractedWith = false;
        Pushable.Energize(true);

        _batteryMesh.SetActive(true);
    }
}
