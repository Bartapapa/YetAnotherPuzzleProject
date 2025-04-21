using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_CrushingHandler : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_CharacterController CharacterController;

    [SerializeField][ReadOnly] private bool _crushed;
    public bool Crushed { get { return _crushed; } }

    private void Start()
    {
        //CharacterController.OnCrushed -= OnCharacterCrushed;
        //CharacterController.OnCrushed += OnCharacterCrushed;
    }

    private void OnCharacterCrushed()
    {
        if (_crushed) return;

        Vector3 playerPos = transform.position;
        float capsuleYOffset = CharacterController.Capsule.center.y;
        Vector3 centerPos = playerPos + (Vector3.up * capsuleYOffset);
        RaycastHit crushingHit;
        Physics.Raycast(centerPos, CharacterController.RB.velocity.normalized, out crushingHit, 5f, CharacterController._groundLayers, QueryTriggerInteraction.Ignore);

        _crushed = true;
        CharacterController.IgnoreInputs = true;
        CharacterController.Capsule.enabled = false;
        Debug.LogError(CharacterController.gameObject.name + " got crushed!");
    }

    private void OnTriggerStay(Collider other)
    {
        if (_crushed) return;
        if (other.gameObject == this.gameObject) return;
        if (other.isTrigger) return;
        if (other.gameObject.layer == 0 || other.gameObject.layer == 8)
        {
            OnCharacterCrushed();
        }
    }
}
