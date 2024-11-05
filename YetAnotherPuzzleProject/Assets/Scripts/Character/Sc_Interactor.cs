using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Interactor : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Character Character;
    public Sc_Inventory _inventory;

    [Header("PARAMETERS")]
    public float _interactionRange = 1.5f;
    private SphereCollider _interactionSphere;

    [ReadOnly][SerializeField] private Sc_Interactible _currentSelectedInteractible;
    [ReadOnly][SerializeField] private List<Sc_Interactible> _potentialInteractibles = new List<Sc_Interactible>();
    public bool CanInteract { get { return _inventory.IsUsingItem || Character.Controller.IsClimbing || Character.Controller.IsAnchoring || Character.Controller.IsAnchoredToValve || !Character.Controller.IsGrounded ? false : true; } }
    public Sc_Interactible CurrentSelectedInteractible { get { return _currentSelectedInteractible; } }

    private void Start()
    {
        _interactionSphere = GetComponent<SphereCollider>();
        if (_interactionSphere == null)
        {
            Debug.LogWarning(this.name + " doesn't have an Interaction Sphere!");
            return;
        }
        _interactionSphere.isTrigger = true;
        _interactionSphere.radius = _interactionRange;
    }

    private void Update()
    {
        RemoveNullInteractibles();
        Sc_Interactible chosenInteractible = SelectInteractible();
        if (_currentSelectedInteractible == null)
        {
            _currentSelectedInteractible = chosenInteractible;
            if (_currentSelectedInteractible != null)
            {
                _currentSelectedInteractible.Select();
            }           
        }
        else
        {
            if (_currentSelectedInteractible == chosenInteractible)
            {
                //Do nothing.
            }
            else
            {
                _currentSelectedInteractible.Deselect();
                _currentSelectedInteractible = chosenInteractible;
                if (_currentSelectedInteractible != null)
                {
                    _currentSelectedInteractible.Select();
                }

            }
        }
    }

    private void RemoveNullInteractibles()
    {
        foreach(Sc_Interactible interactible in _potentialInteractibles)
        {
            if (interactible == null)
            {
                _potentialInteractibles.Remove(interactible);
            }
        }
    }

    private Sc_Interactible SelectInteractible()
    {
        Sc_Interactible chosenInteractible = null;

        if (!CanInteract) return chosenInteractible;

        List<Sc_Interactible> localChosenInteractibles = new List<Sc_Interactible>();

        foreach(Sc_Interactible interactible in _potentialInteractibles)
        {
            if (interactible.CanBeInteractedWith)
            {
                if (interactible._usesKey)
                {
                    if (_inventory == null)
                    {
                        //Do nothing
                    }
                    else
                    {
                        if (!_inventory.IsCurrentlyHoldingItem)
                        {
                            //Do nothing
                        }
                        else
                        {
                            int currentHeldItemKey = _inventory.CurrentlyHeldItem._itemData.ID;
                            if (interactible.InteractorHasCorrectKey(currentHeldItemKey))
                            {
                                localChosenInteractibles.Add(interactible);
                            }
                            else
                            {
                                //Do nothing
                            }
                        }
                    }
                }
                else
                {
                    localChosenInteractibles.Add(interactible);
                }            
            }
        }

        //Break here if only 1 or 0 found.
        if (localChosenInteractibles.Count == 1)
        {
            return localChosenInteractibles[0];
        }
        else if (localChosenInteractibles.Count == 0)
        {
            return null;
        }

        int highestPriority = 0;
        foreach(Sc_Interactible interactible in localChosenInteractibles)
        {
            if (interactible._priority > highestPriority)
            {
                highestPriority = interactible._priority;
            }
        }
        foreach (Sc_Interactible interactible in localChosenInteractibles)
        {
            if (interactible._priority < highestPriority)
            {
                localChosenInteractibles.Remove(interactible);
            }
        }

        //Break here if only 1 or 0 found.
        if (localChosenInteractibles.Count == 1)
        {
            return localChosenInteractibles[0];
        }
        else if (localChosenInteractibles.Count == 0)
        {
            return null;
        }

        float closestDistance = float.MaxValue;
        foreach (Sc_Interactible interactible in localChosenInteractibles)
        {
            float distance = Vector3.Distance(interactible.transform.position, this.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                chosenInteractible = interactible;
            }
        }

        return chosenInteractible;
    }

    public void ClearPotentialInteractibles()
    {
        _potentialInteractibles.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        Sc_Interactible interactible = other.GetComponent<Sc_Interactible>();
        if (interactible)
        {
            if (!_potentialInteractibles.Contains(interactible))
            {
                if (interactible.CanBeInteractedWith)
                {
                    _potentialInteractibles.Add(interactible);
                }              
            }
            else
            {
                if (!interactible.CanBeInteractedWith)
                {
                    _potentialInteractibles.Remove(interactible);
                }
            }           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Sc_Interactible interactible = other.GetComponent<Sc_Interactible>();
        if (interactible)
        {
            _potentialInteractibles.Remove(interactible);
        }
    }
}
