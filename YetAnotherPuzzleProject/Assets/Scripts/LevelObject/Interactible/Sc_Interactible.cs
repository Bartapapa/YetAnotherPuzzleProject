using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_Interactible : MonoBehaviour
{
    [Header("UNITY EVENTS")]
    public UnityEvent<Sc_Interactor> OnInteractedWith;

    [Header("OBJECT REFS")]
    public Material _debugSelectedMat;
    public Material _debugUnselectedMat;
    public Transform _mesh;
    private Renderer _meshRenderer;

    [Header("PARAMETERS")]
    public int _priority = 0;

    [Header("KEY")]
    public bool _usesKey = false;

    private bool _canBeInteractedWith = true;
    public bool CanBeInteractedWith { get { return _canBeInteractedWith; } set { _canBeInteractedWith = value; } }

    private void Start()
    {
        _meshRenderer = _mesh.GetComponent<Renderer>();
    }

    public void Select()
    {
        _meshRenderer.material = _debugSelectedMat;
    }

    public void Deselect()
    {
        _meshRenderer.material = _debugUnselectedMat;
    }

    public void Interact(Sc_Interactor interactor, bool force = false)
    {
        if (_canBeInteractedWith)
        {
            OnInteractedWith?.Invoke(interactor);
        }
        else
        {
            if (force)
            {
                OnInteractedWith?.Invoke(interactor);
            }
        }
    }
}
