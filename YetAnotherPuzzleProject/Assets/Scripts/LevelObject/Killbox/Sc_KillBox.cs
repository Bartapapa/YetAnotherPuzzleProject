using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_KillBox : MonoBehaviour
{
    [Header("RESPAWN")]
    public Transform _respawnPoint;

    [Header("DEBUG")]
    public bool _killPlayerCharacter = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_killPlayerCharacter)
        {
            Sc_Character_Player playerCharacter = other.GetComponent<Sc_Character_Player>();
            if (playerCharacter)
            {
                playerCharacter.Controller.RB.MovePosition(_respawnPoint.position);
                return;
            }
        }
    }
}
