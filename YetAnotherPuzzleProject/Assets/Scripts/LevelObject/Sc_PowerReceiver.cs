using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PowerReceiver : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Activateable _activateable;

    private List<Sc_PowerGenerator> _registeredGenerators = new List<Sc_PowerGenerator>();

    private void RegisterGenerator(Sc_PowerGenerator generator)
    {
        _registeredGenerators.Add(generator);

        generator.GeneratingPower -= OnRegisteredGeneratorChangeGenerating;
        generator.GeneratingPower += OnRegisteredGeneratorChangeGenerating;

        _activateable.Activate(IsReceivingPower());
    }

    private void UnregisterGenerator(Sc_PowerGenerator generator)
    {
        if (!_registeredGenerators.Contains(generator)) return;

        _registeredGenerators.Remove(generator);

        generator.GeneratingPower -= OnRegisteredGeneratorChangeGenerating;

        _activateable.Activate(IsReceivingPower());
    }

    public void OnRegisteredGeneratorChangeGenerating(bool generating)
    {
        _activateable.Activate(IsReceivingPower());
    }

    public bool IsReceivingPower()
    {
        bool receivingPower = false;
        foreach (Sc_PowerGenerator generator in _registeredGenerators)
        {
            if (generator.GeneratesPower) receivingPower = true;
        }

        return receivingPower;
    }

    private void OnTriggerEnter(Collider other)
    {
        Sc_PowerGenerator generator = other.GetComponent<Sc_PowerGenerator>();
        if (generator) RegisterGenerator(generator);
    }

    private void OnTriggerExit(Collider other)
    {
        Sc_PowerGenerator generator = other.GetComponent<Sc_PowerGenerator>();
        if (generator) UnregisterGenerator(generator);
    }
}
