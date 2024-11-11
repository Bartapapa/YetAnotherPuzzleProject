using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_Activateable : MonoBehaviour
{
    [Header("LOCK")]
    public Sc_Lock Lock;

    [Header("PARAMETERS")]
    public bool StartActivated = false;
    [ReadOnly][SerializeField] public bool IsActivated = false;
    [ReadOnly][SerializeField] public bool CanBeActivated = true;
    [ReadOnly][SerializeField] public bool CanBeDeactivated = true;

    [Header("PERIODIC ACTIVATION")]
    public bool PeriodicActivation = false;
    public float PeriodicActivationDuration = 5f;
    public float PeriodicActivationInitialDelay = 0f;
    protected float _activationTimer = 0f;
    protected float _periodicActivationInitialDelayTimer = 0f;

    protected virtual void Start()
    {
        if (Lock)
        {
            Lock.LockEngaged -= OnLockEngaged;
            Lock.LockEngaged += OnLockEngaged;

            Lock.LockDestroyed -= OnLockDestroyed;
            Lock.LockDestroyed += OnLockDestroyed;
        }

        ForceActivate(StartActivated);

        if (PeriodicActivation)
        {
            _activationTimer = PeriodicActivationDuration;
        }
    }

    protected virtual void Update()
    {
        if (PeriodicActivation)
        {
            if (_periodicActivationInitialDelayTimer < PeriodicActivationInitialDelay)
            {
                _periodicActivationInitialDelayTimer += Time.deltaTime;
            }
            else
            {
                _activationTimer += Time.deltaTime;
                if (_activationTimer >= PeriodicActivationDuration)
                {
                    ToggleActivation();
                    _activationTimer = 0f;
                }
            }
        }
    }

    public virtual void ForceActivate(bool toggleOn)
    {
        IsActivated = toggleOn;

        if (toggleOn)
        {
            Debug.Log(this.name + " has been forcefully activated!");
        }
        else
        {
            Debug.Log(this.name + " has been forcefully deactivated!");
        }
    }

    public virtual bool Activate(bool toggleOn)
    {
        if (toggleOn == IsActivated)
        {
            return false;
        }

        if (toggleOn)
        {
            if (!CanBeActivated)
            {
                return false;
            }
        }
        else
        {
            if (!CanBeDeactivated)
            {
                return false;
            }
        }

        if (Lock != null)
        {
            Lock.Spin(toggleOn);
            if (!Lock.IsActivated) return false;
        }

        IsActivated = toggleOn;
        
        if (toggleOn)
        {
            Debug.Log(this.name + " has been activated!");
        }
        else
        {
            Debug.Log(this.name + " has been deactivated!");
        }

        return true;
    }

    public bool ToggleActivation()
    {
        return Activate(!IsActivated);
    }

    public virtual void OnLockEngaged(bool engage)
    {

    }

    public virtual void OnLockDestroyed()
    {

    }
}
