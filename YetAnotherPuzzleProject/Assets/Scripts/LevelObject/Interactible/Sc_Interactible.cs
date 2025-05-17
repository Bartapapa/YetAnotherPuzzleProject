using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_Interactible : MonoBehaviour
{
    [Header("UNITY EVENTS")]
    public UnityEvent<Sc_Character> OnInteractedWith;
    public UnityEvent<Sc_Character_Player> OnRoboArmInteraction;
    public UnityEvent<Sc_Character> OnEndInteraction;
    public UnityEvent OnThrownInteraction;

    [Header("OBJECT REFS")]
    public List<Transform> _interactibleHighlightMeshes = new List<Transform>();
    public bool useDebug = true;
    public Material _debugSelectedMat;
    public Material _debugUnselectedMat;
    public Transform _mesh;
    private Renderer _meshRenderer;
    private List<Material> _interactibleHighlightMats = new List<Material>();

    [Header("PARAMETERS")]
    public int _priority = 0;
    public bool CanBeInteractedWithOnThrow = false;
    [SerializeField] private bool _canBeInteractedWith = true;

    [Header("CONDITION")]
    public Condition _condition;
    public bool CanBeInteractedWith { get { return _canBeInteractedWith; } set { _canBeInteractedWith = value; } }

    private void Start()
    {
        if (useDebug)
        {
            _meshRenderer = _mesh.GetComponent<Renderer>();
        }
        else
        {
            foreach (Transform mesh in _interactibleHighlightMeshes)
            {
                Renderer rend = mesh.GetComponent<Renderer>();
                if (rend)
                {
                    Material mat = rend.material;
                    _interactibleHighlightMats.Add(mat);
                }
            }
        }
    }

    public virtual void Select()
    {
        if (useDebug)
        {
            _meshRenderer.material = _debugSelectedMat;
        }
        else
        {
            foreach(Material mat in _interactibleHighlightMats)
            {
                mat.SetFloat("_interactibleSelectLerp", 1f);
            }
        }       
    }

    public virtual void Deselect()
    {
        if (useDebug)
        {
            _meshRenderer.material = _debugUnselectedMat;
        }
        else
        {
            foreach (Material mat in _interactibleHighlightMats)
            {
                mat.SetFloat("_interactibleSelectLerp", 0f);
            }
        }       
    }

    public void Interact(Sc_Character interactor, bool force = false)
    {
        if (_canBeInteractedWith)
        {
            Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
            if (player)
            {
                player.Interactor._lastInteractedInteractible = this;

                if (_condition.CheckPlayerCondition(player))
                {
                    OnInteractedWith?.Invoke(interactor);
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

    public virtual void EndInteract(Sc_Character interactor)
    {
        Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
        if (player)
        {
            //Debug.Log("INTERACTION ENDED - ITEM");
            player.Interactor._lastInteractedInteractible = null;
        }

        OnEndInteraction?.Invoke(interactor);
        //Base EndInteract method
    }

    public void InteractWithThrow()
    {
        if (_canBeInteractedWith)
        {
            OnThrownInteraction?.Invoke();
        }
    }

    public bool InteractorMeetsConditions(Sc_Character_Player interactingCharacter)
    {
        return _condition.CheckPlayerCondition(interactingCharacter);
    }
}
