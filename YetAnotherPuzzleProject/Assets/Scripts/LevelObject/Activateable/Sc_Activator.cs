using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Activator : MonoBehaviour
{
    [Header("ACTIVATEABLES")]
    public List<Sc_Activateable> Activateables = new List<Sc_Activateable>();

    [Header("CAN FAIL TO ACTIVATE")]
    public bool CanFailToActivate = false;

    [Header("ACTIVATION DELAY")]
    public float ActivationDelay = 1f;

    private Coroutine _delayedActivationCo = null;


    protected bool CanActivateAllActivateables(bool activate)
    {
        bool canActivate = true;
        if (!CanFailToActivate)
        {
            return canActivate;
        }

        foreach(Sc_Activateable activateable in Activateables)
        {
            if(activateable.IsActivated == activate)
            {
                canActivate = false;
                break;
            }

            if (activate)
            {
                if (!activateable.CanBeActivated)
                {
                    canActivate = false;
                    break;
                }
            }
            else
            {
                if (!activateable.CanBeDeactivated)
                {
                    canActivate = false;
                    break;
                }
            }

            if (activateable.Lock != null)
            {
                if (!activateable.Lock.IsActivated)
                {
                    canActivate = false;
                    break;
                }
            }
        }
        return canActivate;
    }

    protected bool CanToggleAllActivateables()
    {
        bool canActivate = true;
        if (!CanFailToActivate)
        {
            return canActivate;
        }

        foreach (Sc_Activateable activateable in Activateables)
        {
            if (activateable.IsActivated)
            {
                if (!activateable.CanBeDeactivated)
                {
                    canActivate = false;
                    break;
                }
            }
            else
            {
                if (!activateable.CanBeActivated)
                {
                    canActivate = false;
                    break;
                }
            }

            if (activateable.Lock != null)
            {
                if (!activateable.Lock.IsActivated)
                {
                    canActivate = false;
                    break;
                }
            }
        }
        return canActivate;
    }

    protected List<Sc_Lock> GetUnactivateableLocks()
    {
        List<Sc_Lock> unactivateableLocks = new List<Sc_Lock>();
        foreach(Sc_Activateable activateable in Activateables)
        {
            if (activateable.Lock != null)
            {
                if (!activateable.Lock.CanBeActivated)
                {
                    unactivateableLocks.Add(activateable.Lock);
                }
            }

            Sc_Lock locked = activateable.GetComponent<Sc_Lock>();
            if (locked)
            {
                if (!locked.CanBeActivated)
                {
                    unactivateableLocks.Add(locked);
                }
            }
        }
        return unactivateableLocks;
    }
    public bool Activate()
    {
        if (!CanActivateAllActivateables(true))
        {
            FailedToActivate();
            return false;
        }

        foreach(Sc_Activateable activateable in Activateables)
        {
            activateable.Activate(true);
        }
        return true;
    }

    public bool Deactivate()
    {
        if (!CanActivateAllActivateables(false))
        {
            FailedToActivate();
            return false;
        }

        foreach (Sc_Activateable activateable in Activateables)
        {
            activateable.Activate(false);
        }
        return true;
    }

    public bool ToggleActivate()
    {
        if (!CanToggleAllActivateables())
        {
            FailedToActivate();
            return false;
        }

        foreach (Sc_Activateable activateable in Activateables)
        {
            activateable.ToggleActivation();
        }
        return true;
    }

    protected virtual void FailedToActivate()
    {
        Debug.Log(this.gameObject.name + " failed to activate!");

        foreach (Sc_Lock locks in GetUnactivateableLocks())
        {
            locks.FailToEngage();
        }
    }

    protected bool DelayActivation()
    {
        if (!CanToggleAllActivateables())
        {
            FailedToActivate();
            return false;
        }

        _delayedActivationCo = StartCoroutine(DelayedActivationCoroutine());
        return true;
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
