using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PressurePlate : Sc_Activator
{
    [Header("PARAMETERS")]
    public int WeightThreshold = 10;
    [ReadOnly] public int CurrentWeight = 0;
    [ReadOnly] public List<Sc_WeightedObject> RegisteredObjects = new List<Sc_WeightedObject>();
    private bool HasReachedWeightThreshold { get { return CurrentWeight >= WeightThreshold; } }

    private Coroutine _delayedActivationCo = null;
    private bool _pressed = false;
    private BoxCollider _coll;

    private void Start()
    {
        _coll = GetComponent<BoxCollider>();

        RecheckWeightedObjects();
    }

    private void OnWeightObjectStateChanged()
    {
        RecheckWeightedObjects();
    }

    private void RecheckWeightedObjects()
    {
        RegisteredObjects.Clear();

        Vector3 center = transform.position + _coll.center;
        Vector3 halfExtents = _coll.size * .5f;
        Collider[] coll = Physics.OverlapBox(center, halfExtents, Quaternion.identity);

        foreach(Collider collider in coll)
        {
            Sc_WeightedObject wObject = collider.GetComponent<Sc_WeightedObject>();
            if (wObject) RegisterObject(wObject);
        }

        CheckWeight();
    }

    private void CheckWeight()
    {
        CurrentWeight = 0;
        foreach(Sc_WeightedObject wObject in RegisteredObjects)
        {
            CurrentWeight += wObject._weight;
        }

        if (HasReachedWeightThreshold)
        {
            if (!_pressed)
            {
                if (!DelayActivation())
                {
                    return;
                }
                PressPlate(true);
            }
        }
        else
        {
            if (_pressed)
            {
                if (!StopDelayedActivation())
                {
                    if (!ToggleActivate())
                    {
                        return;
                    }
                }
                PressPlate(false);
            }
        }
    }

    private void PressPlate(bool press)
    {
        _pressed = press;
    }

    //protected override IEnumerator DelayedActivationCoroutine()
    //{
    //    float timer = 0f;
    //    while (timer < ActivationDelay)
    //    {
    //        timer += Time.deltaTime;
    //        yield return null;
    //    }

    //    Activate();

    //    _delayedActivationCo = null;
    //}

    private void RegisterObject(Sc_WeightedObject wObject)
    {
        if (RegisteredObjects.Contains(wObject)) return;

        RegisteredObjects.Add(wObject);
        wObject.OnStateChanged -= OnWeightObjectStateChanged;
        wObject.OnStateChanged += OnWeightObjectStateChanged;

        CheckWeight();
    }

    private void UnregisterObject(Sc_WeightedObject wObject)
    {
        if (!RegisteredObjects.Contains(wObject)) return;

        RegisteredObjects.Remove(wObject);
        wObject.OnStateChanged -= OnWeightObjectStateChanged;

        CheckWeight();
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
