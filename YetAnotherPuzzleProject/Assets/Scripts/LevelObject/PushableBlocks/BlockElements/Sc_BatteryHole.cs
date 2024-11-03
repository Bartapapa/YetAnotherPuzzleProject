using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_BatteryHole : Sc_Activateable
{
    public UnityEvent OnBatteryPlaced;

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
        if (Pushable) Pushable.Energize(true);

        OnBatteryPlaced?.Invoke();

        _batteryMesh.SetActive(true);
    }
}
