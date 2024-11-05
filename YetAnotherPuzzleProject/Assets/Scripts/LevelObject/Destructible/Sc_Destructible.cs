using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_Destructible : MonoBehaviour
{
    public UnityEvent<Sc_Character> OnDestructibleDestroy;

    [Header("DESTRUCTIBLE REFERENCES")]
    public GameObject FullMesh;
    public GameObject DestroyedMesh;
    public Collider FullMeshCollider;
    public Collider DestroyedMeshCollider;

    [Header("STATE")]
    [ReadOnly] public bool Destroyed = false;

    [Header("PARAMETERS")]
    public bool DestroyedByPick = false;

    private void Start()
    {
        ToggleFullMesh(true);
        ToggleDestroyedMesh(false);       
    }

    public void DestructibleDestroy(Sc_Character destroyer)
    {
        if (destroyer != null)
        {
            OnDestructibleDestroy?.Invoke(destroyer);
        }
        OnDestroyEvent(destroyer);
    }

    protected virtual void OnDestroyEvent(Sc_Character character)
    {
        Destroyed = true;

        ToggleFullMesh(false);
        ToggleDestroyedMesh(true);
    }

    private void ToggleFullMesh(bool turnOn)
    {
        FullMesh.SetActive(turnOn);
        if (FullMeshCollider)
        {
            FullMeshCollider.enabled = turnOn;
        }       
    }

    private void ToggleDestroyedMesh(bool turnOn)
    {
        DestroyedMesh.SetActive(turnOn);
        if (DestroyedMeshCollider)
        {
            DestroyedMeshCollider.enabled = turnOn;
        }        
    }
}
