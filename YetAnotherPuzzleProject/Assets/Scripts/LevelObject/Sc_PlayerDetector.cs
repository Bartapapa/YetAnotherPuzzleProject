using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderType
{
    None,
    Box,
    Sphere,
    Capsule,
    Mesh,
}

public class Sc_PlayerDetector : MonoBehaviour
{
    [Header("PARAMETERS")]
    public ColliderType ColliderType = ColliderType.Box;

    private BoxCollider _boxCollider;
    private SphereCollider _sphereCollider;
    private CapsuleCollider _capsuleCollider;
    private MeshCollider _meshCollider;

    public delegate void CharacterEvent(Sc_Character character);
    public event CharacterEvent CharacterDetected;
    public event CharacterEvent CharacterUndetected;

    private void Awake()
    {
        switch (ColliderType)
        {
            case ColliderType.None:
                break;
            case ColliderType.Box:
                _boxCollider = GetComponent<BoxCollider>();
                _boxCollider.isTrigger = true;
                break;
            case ColliderType.Sphere:
                _sphereCollider = GetComponent<SphereCollider>();
                _sphereCollider.isTrigger = true;
                break;
            case ColliderType.Capsule:
                break;
            case ColliderType.Mesh:
                break;
            default:
                break;
        }
    }
    public void InitializeDetector(Vector3 offset, Vector3 size)
    {
        switch (ColliderType)
        {
            case ColliderType.None:
                break;
            case ColliderType.Box:
                _boxCollider.center = offset;
                _boxCollider.size = size;
                break;
            case ColliderType.Sphere:
                _sphereCollider.center = offset;
                _sphereCollider.radius = size.x;
                break;
            case ColliderType.Capsule:
                break;
            case ColliderType.Mesh:
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Sc_Character character = other.GetComponent<Sc_Character>();
        if (character)
        {
            Debug.Log(this.name + " has detected: " + character.name + "!");
            CharacterDetected?.Invoke(character);
        }

        //Check if player behind obstacle
    }

    private void OnTriggerExit(Collider other)
    {
        Sc_Character character = other.GetComponent<Sc_Character>();
        if (character)
        {
            Debug.Log(character.name + " has left " + this.name + "'s detection.");
            CharacterUndetected?.Invoke(character);
        }
    }
}
