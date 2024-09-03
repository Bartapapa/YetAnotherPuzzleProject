using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PressurePlate : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Activateable _activator;

    [Header("PARAMETERS")]
    public int _weightThreshold = 10;
    public int _currentWeight = 0;
    private bool HasReachedWeightThreshold { get { return _currentWeight >= _weightThreshold; } }

    private List<Sc_WeightedObject> _registeredObjects = new List<Sc_WeightedObject>();

    private void RegisterObject(Sc_WeightedObject wObject)
    {
        _registeredObjects.Add(wObject);
        _currentWeight += wObject._weight;

        if (HasReachedWeightThreshold) _activator.Activate(true);
    }

    private void UnregisterObject(Sc_WeightedObject wObject)
    {
        if (!_registeredObjects.Contains(wObject)) return;

        _registeredObjects.Remove(wObject);
        _currentWeight -= wObject._weight;

        if (!HasReachedWeightThreshold) _activator.Activate(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Sc_WeightedObject wObject = other.GetComponent<Sc_WeightedObject>();
        if (wObject) RegisterObject(wObject);
    }

    private void OnTriggerExit(Collider other)
    {
        Sc_WeightedObject wObject = other.GetComponent<Sc_WeightedObject>();
        if (wObject) UnregisterObject(wObject);
    }
}
