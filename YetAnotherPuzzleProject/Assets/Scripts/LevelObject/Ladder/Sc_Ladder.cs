using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Ladder : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Interactible _interactible;

    [Header("LADDER")]
    public bool _ladderTop = false;

    public Transform _startPoint;
    public Transform _endPoint;
    public Transform _footAnchor;
    public Transform _topAnchor;


    public void OnInteract(Sc_Character interactor)
    {
        Sc_CharacterController interactorController = interactor.Controller;
        if (interactorController)
        {
            interactorController.InitiateClimbSequence(this);
        }
    }

    public void OnCharacterDetachFromLadder(Sc_Character character)
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_startPoint != null && _endPoint != null)
        {
            Gizmos.DrawLine(_startPoint.position, _endPoint.position);
            Gizmos.DrawSphere(_startPoint.position, .1f);
            Gizmos.DrawSphere(_endPoint.position, .1f);
        }

        Gizmos.color = Color.yellow;

        if (_footAnchor != null && _topAnchor != null)
        {
            Gizmos.DrawSphere(_footAnchor.position, .1f);
            Gizmos.DrawSphere(_topAnchor.position, .1f);
        }
    }
}
