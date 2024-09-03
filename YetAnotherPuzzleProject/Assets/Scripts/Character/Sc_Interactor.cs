using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Interactor : MonoBehaviour
{
    [Header("PARAMETERS")]
    public float _interactionRange = 1.5f;
    private SphereCollider _interactionSphere;

    private Sc_Interactible _currentSelectedInteractible;
    private List<Sc_Interactible> _potentialInteractibles = new List<Sc_Interactible>();
    private bool _canInteract = true;
    public bool CanInteract { get { return _canInteract; } }
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

    private Sc_Interactible SelectInteractible()
    {
        Sc_Interactible chosenInteractible = null;

        if (!_canInteract) return chosenInteractible;

        List<Sc_Interactible> localChosenInteractibles = new List<Sc_Interactible>();

        foreach(Sc_Interactible interactible in _potentialInteractibles)
        {
            if (interactible.CanBeInteractedWith)
            {
                localChosenInteractibles.Add(interactible);
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

    private void OnTriggerEnter(Collider other)
    {
        Sc_Interactible interactible = other.GetComponent<Sc_Interactible>();
        if (interactible)
        {
            _potentialInteractibles.Add(interactible);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Sc_Interactible interactible = other.GetComponent<Sc_Interactible>();
        if (interactible)
        {
            if (!_potentialInteractibles.Contains(interactible)) return;
            _potentialInteractibles.Remove(interactible);
        }
    }
}
