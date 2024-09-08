using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_Activateable : MonoBehaviour
{
    [Header("UNITY EVENTS")]
    public UnityEvent<bool> OnActivate;
    public UnityEvent<bool> OnForceActivate;
    //public UnityEvent<bool> OnDeactivate;

    [Header("OBJECT REFS")]
    public Material _debugDeactivatedMat;
    public Material _debugActivatedMat;
    public List<Transform> _activationMeshes = new List<Transform>();
    private List<Renderer> _meshRenderers = new List<Renderer>();

    [Header("PARAMETERS")]
    public bool _startActivated = false;
    private bool _isActivated = false;
    public bool IsActivated { get { return _isActivated; } set { _isActivated = value; } }

    [Header("CONDITIONAL ACTIVATORS")]
    public List<Sc_Activateable> _conditions = new List<Sc_Activateable>();

    [Header("PERIODIC ACTIVATION")]
    public bool _periodicActivation = false;
    public float _activationDuration = 5f;
    private float _activationTimer = 0f;

    private void Start()
    {
        //_meshRenderers = _activationMeshes.GetComponent<Renderer>();
        foreach(Transform mesh in _activationMeshes)
        {
            Renderer renderer = mesh.GetComponent<Renderer>();
            if (renderer)
            {
                _meshRenderers.Add(renderer);
            }
        }

        //ForceActivate(_startActivated);
        Activate(_startActivated);
    }

    private void Update()
    {
        if (_periodicActivation)
        {
            _activationTimer += Time.deltaTime;
            if (_activationTimer >= _activationDuration)
            {
                ToggleActivation();
                _activationTimer = 0f;
            }
        }
    }

    public void ForceActivate(bool toggleOn)
    {
        _isActivated = toggleOn;
        ToggleMeshMaterial(toggleOn);

        OnForceActivate?.Invoke(toggleOn);
    }

    public void Activate(bool toggleOn)
    {
        if (toggleOn == _isActivated) return;
        if (toggleOn && !HasFilledConditionsForActivation())
        {
            Activate(false);
            return;
        }
        _isActivated = toggleOn;
        ToggleMeshMaterial(toggleOn);

        OnActivate?.Invoke(toggleOn);

        if (toggleOn)
        {
            Debug.Log(this.name + " has been activated!");
        }
        else
        {
            Debug.Log(this.name + " has been deactivated!");
        }
    }

    public void ToggleActivation()
    {
        Activate(!_isActivated);
    }

    private void ToggleMeshMaterial(bool toggleOn)
    {
        foreach(Renderer renderer in _meshRenderers)
        {
            renderer.material = toggleOn ? _debugActivatedMat : _debugDeactivatedMat;
        }
    }

    private bool HasFilledConditionsForActivation()
    {
        if (_conditions.Count == 0) return true;

        bool canActivate = true;

        foreach(Sc_Activateable condition in _conditions)
        {
            if (!condition.IsActivated)
            {
                canActivate = false;
            }
        }

        return canActivate;
    }
}
