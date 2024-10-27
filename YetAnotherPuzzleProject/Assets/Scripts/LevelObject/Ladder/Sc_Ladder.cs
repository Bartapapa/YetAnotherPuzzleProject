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
    public Transform _ladderMesh;


    public void OnInteract(Sc_Character interactor)
    {
        Sc_CharacterController interactorController = interactor.Controller;
        if (interactorController)
        {
            interactorController.InitiateClimbSequence(this);
        }
    }

    public void CreateLadder(Vector3 basePoint, float height, Vector3 forwardDir)
    {
        //The ladder is always placed directly against the wall, its forward facing perpendicular outwards of the wall. This is the base point.
        transform.position = basePoint;
        transform.forward = forwardDir;

        //The start point is always .25 off of the ground in Y, and .5 to the ladder's forward
        _startPoint.localPosition = new Vector3(0f, basePoint.y + .25f, .5f);

        //The end point is always .5 from the ladder's maximum height, and .5 to the ladder's forward
        _endPoint.localPosition = new Vector3(0f, basePoint.y + height, .5f);

        //The footanchor is always on the ground, and .75 from the ladder's forward
        _footAnchor.localPosition = new Vector3(0f, basePoint.y, .75f);

        //the topanchor is always at the maximum height, and -.25 from the ladder's forward
        _topAnchor.localPosition = new Vector3(0f, basePoint.y + height, -.25f);

        //The mesh of the ladder is always the total height, and has a local offset in Y equal to /2 the height
        _ladderMesh.localScale = new Vector3(_ladderMesh.localScale.x, height, _ladderMesh.localScale.z);
        _ladderMesh.localPosition = new Vector3(0f, height * .5f, 0f);
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
