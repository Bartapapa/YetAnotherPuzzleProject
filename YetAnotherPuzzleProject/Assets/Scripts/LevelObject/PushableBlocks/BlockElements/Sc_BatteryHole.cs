using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_BatteryHole : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Interactible _interactible;
    public Sc_Activateable _activateable;
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
        _activateable.Activate(true);

        _batteryMesh.SetActive(true);
    }
}
