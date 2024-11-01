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

    [Header("PERIODIC ACTIVATION")]
    public bool PeriodicActivation = false;
    public float PeriodicActivationDuration = 5f;
    private float _activationTimer = 0f;

    private void Start()
    {
        ForceActivate(StartActivated);
    }

    protected virtual void Update()
    {
        if (PeriodicActivation)
        {
            _activationTimer += Time.deltaTime;
            if (_activationTimer >= PeriodicActivationDuration)
            {
                ToggleActivation();
                _activationTimer = 0f;
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
}
