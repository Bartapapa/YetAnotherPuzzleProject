using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_Interactible : MonoBehaviour
{
    [Header("UNITY EVENTS")]
    public UnityEvent<Sc_Character> OnInteractedWith;
    public UnityEvent OnThrownInteraction;

    [Header("OBJECT REFS")]
    public Material _debugSelectedMat;
    public Material _debugUnselectedMat;
    public Transform _mesh;
    private Renderer _meshRenderer;

    [Header("PARAMETERS")]
    public int _priority = 0;

    [Header("KEY")]
    public bool _usesKey = false;
    public List<int> _keyIDs = new List<int>();

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

    public void Interact(Sc_Character interactor, bool force = false)
    {
        if (_canBeInteractedWith)
        {          
            if (_usesKey)
            {
                Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
                if (player)
                {
                    player.Inventory._currentlyHeldItem.UseItem();
                    OnInteractedWith?.Invoke(interactor);
                }
            }
            else
            {
                OnInteractedWith?.Invoke(interactor);
            }
        }
        else
        {
            if (force)
            {
                OnInteractedWith?.Invoke(interactor);
            }
        }
    }

    public void InteractWithThrow()
    {
        if (_canBeInteractedWith)
        {
            OnThrownInteraction?.Invoke();
        }
    }

    public bool InteractorHasCorrectKey(int id)
    {
        return _keyIDs.Contains(id);
    }
}
