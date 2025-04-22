using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_CrushingHandler : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Character_Player PlayerCharacter;
    public Sc_Health Health;

    [Header("PARAMETERS")]
    public float YOffset = 1f;
    public LayerMask WallLayers;

    [SerializeField][ReadOnly] private bool _crushed;
    public bool Crushed { get { return _crushed; } }

    private void OnCharacterCrushed(Vector3 crushingNormal)
    {
        if (_crushed) return;

        Vector3 playerPos = transform.position;
        float capsuleYOffset = PlayerCharacter.Controller.Capsule.center.y;
        Vector3 centerPos = playerPos + (Vector3.up * capsuleYOffset);
        RaycastHit crushingHit;
        Physics.Raycast(centerPos, new Vector3(PlayerCharacter.Controller.RB.velocity.x, 0f, PlayerCharacter.Controller.RB.velocity.z).normalized, out crushingHit, 5f, PlayerCharacter.Controller._groundLayers, QueryTriggerInteraction.Ignore);

        _crushed = true;
        PlayerCharacter.Controller.IgnoreInputs = true;
        PlayerCharacter.Controller.Capsule.enabled = false;
        Debug.LogError(PlayerCharacter.Controller.gameObject.name + " got crushed!");

        if (crushingNormal != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(-crushingNormal, Vector3.up);
            PlayerCharacter.Controller.RB.rotation = rot;

            RaycastHit crushHit;
            if (Physics.Raycast(transform.position + (Vector3.up*(YOffset*.5f)), crushingNormal, out crushHit, 5f, WallLayers, QueryTriggerInteraction.Ignore))
            {
                Sc_SquashedPlane newPlane = Instantiate<Sc_SquashedPlane>(PlayerCharacter.SquashedPlanePrefab, crushHit.point + (crushHit.normal*.05f), rot);
                PlayerCharacter.Mesh.gameObject.SetActive(false);
            }
        }

        Health.Death();
    }

    private void OnTriggerStay(Collider other)
    {
        if (_crushed) return;
        if (other.gameObject == this.gameObject) return;
        if (other.isTrigger) return;
        if (other.gameObject.layer == 0 || other.gameObject.layer == 8)
        {
            Sc_Crusher crusher = other.gameObject.GetComponent<Sc_Crusher>();
            if (crusher)
            {
                OnCharacterCrushed(crusher.transform.forward);
            }
            OnCharacterCrushed(Vector3.zero);
        }
    }
}
