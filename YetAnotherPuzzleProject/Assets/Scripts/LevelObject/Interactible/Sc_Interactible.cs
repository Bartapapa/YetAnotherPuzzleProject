using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_Interactible : MonoBehaviour
{
    [Header("UNITY EVENTS")]
    public UnityEvent<Sc_Character> OnInteractedWith;
    public UnityEvent<Sc_Character_Player> OnRoboArmInteraction;
    public UnityEvent OnThrownInteraction;

    [Header("OBJECT REFS")]
    public Material _debugSelectedMat;
    public Material _debugUnselectedMat;
    public Transform _mesh;
    private Renderer _meshRenderer;

    [Header("PARAMETERS")]
    public int _priority = 0;
    public bool CanBeInteractedWithOnThrow = false;
    [SerializeField] private bool _canBeInteractedWith = true;

    [Header("KEY")]
    public Condition _condition;
    public bool _usesKey = false;
    public bool _usesRoboArm = false;
    public List<int> _keyIDs = new List<int>();
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
            Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
            if (player)
            {
                if (_usesRoboArm && _usesKey)
                {
                    if (player.Inventory.CurrentlyHeldItem != null)
                    {
                        if (_condition.CheckPlayerItemCondition(player.Inventory.CurrentlyHeldItem._itemData.ID))
                        {
                            if (player.Inventory.CurrentlyHeldItem.UseItemAsKey(this))
                            {
                                OnInteractedWith?.Invoke(interactor);
                                return;
                            }
                        }
                        else
                        {
                            if (player.HasRoboArm)
                            {
                                OnRoboArmInteraction?.Invoke(player);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (player.HasRoboArm)
                        {
                            OnRoboArmInteraction?.Invoke(player);
                            return;
                        }
                    }
                }
                else if (_usesKey)
                {
                    if (player.Inventory.CurrentlyHeldItem.UseItemAsKey(this))
                    {
                        OnInteractedWith?.Invoke(interactor);
                        return;
                    }
                }
                else if (_usesRoboArm)
                {
                    if (player.HasRoboArm)
                    {
                        OnRoboArmInteraction?.Invoke(player);
                        return;
                    }
                }
                else
                {
                    OnInteractedWith?.Invoke(interactor);
                    return;
                }
            }
            else
            {
                OnInteractedWith?.Invoke(interactor);
                return;
            }
        }
        else
        {
            if (force)
            {
                OnInteractedWith?.Invoke(interactor);
                return;
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

    public bool InteractorMeetsConditions(Sc_Character_Player interactingCharacter)
    {
        return _condition.CheckPlayerCondition(interactingCharacter);
    }
}
