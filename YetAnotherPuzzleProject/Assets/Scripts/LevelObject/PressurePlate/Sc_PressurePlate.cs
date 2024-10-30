using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PressurePlate : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Activateable _activator;

    [Header("PARAMETERS")]
    public float ActivationDelay = 1f;
    public int _weightThreshold = 10;
    public int _currentWeight = 0;
    public List<Sc_WeightedObject> _registeredObjects = new List<Sc_WeightedObject>();
    private bool HasReachedWeightThreshold { get { return _currentWeight >= _weightThreshold; } }


    private Coroutine _delayedActivationCo = null;
    private bool _activated = false;
    private BoxCollider _coll;

    private void Start()
    {
        _coll = GetComponent<BoxCollider>();

        RecheckWeightedObjects();
    }
    private void RegisterObject(Sc_WeightedObject wObject)
    {
        if (_registeredObjects.Contains(wObject)) return;

        _registeredObjects.Add(wObject);
        wObject.OnStateChanged -= OnWeightObjectStateChanged;
        wObject.OnStateChanged += OnWeightObjectStateChanged;

        CheckWeight();
    }

    private void OnWeightObjectStateChanged()
    {
        RecheckWeightedObjects();
    }

    private void RecheckWeightedObjects()
    {
        _registeredObjects.Clear();

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
        _currentWeight = 0;
        foreach(Sc_WeightedObject wObject in _registeredObjects)
        {
            _currentWeight += wObject._weight;
        }

        if (HasReachedWeightThreshold)
        {
            if (!_activated)
            {
                DelayActivation();
            }
        }
        else
        {
            StopDelayedActivation();
            _activator.Activate(false);
            _activated = false;
        }
    }

    private void UnregisterObject(Sc_WeightedObject wObject)
    {
        if (!_registeredObjects.Contains(wObject)) return;

        _registeredObjects.Remove(wObject);
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

    private void DelayActivation()
    {
        if (_activated) return;
        _activated = true;
        _delayedActivationCo = StartCoroutine(DelayedActivationCoroutine());
    }

    private void StopDelayedActivation()
    {
        if (_delayedActivationCo != null)
        {
            StopCoroutine(_delayedActivationCo);
            _delayedActivationCo = null;
        }
    }

    private IEnumerator DelayedActivationCoroutine()
    {
        float timer = 0f;
        while (timer < ActivationDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _activator.Activate(true);
        _delayedActivationCo = null;
    }
}
