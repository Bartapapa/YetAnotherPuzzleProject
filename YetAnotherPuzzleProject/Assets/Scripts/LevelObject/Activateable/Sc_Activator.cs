using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Activator : MonoBehaviour
{
    [Header("ACTIVATEABLES")]
    public List<Sc_Activateable> Activateables = new List<Sc_Activateable>();

    [Header("ACTIVATION DELAY")]
    public float ActivationDelay = 1f;

    private Coroutine _delayedActivationCo = null;

    public void Activate()
    {
        foreach(Sc_Activateable activateable in Activateables)
        {
            activateable.Activate(true);
        }
    }

    public void Deactivate()
    {
        foreach (Sc_Activateable activateable in Activateables)
        {
            activateable.Activate(false);
        }
    }

    public void ToggleActivate()
    {
        foreach (Sc_Activateable activateable in Activateables)
        {
            activateable.ToggleActivation();
        }
    }

    protected void DelayActivation()
    {
        _delayedActivationCo = StartCoroutine(DelayedActivationCoroutine());
    }

    protected bool StopDelayedActivation()
    {
        if (_delayedActivationCo != null)
        {
            StopCoroutine(_delayedActivationCo);
            _delayedActivationCo = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    protected virtual IEnumerator DelayedActivationCoroutine()
    {
        float timer = 0f;
        while (timer < ActivationDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        ToggleActivate();

        _delayedActivationCo = null;
    }
}
