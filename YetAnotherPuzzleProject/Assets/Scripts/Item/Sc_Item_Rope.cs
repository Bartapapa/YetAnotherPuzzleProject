using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item_Rope : Sc_Item
{
    [Header("ROPE OBJECT REFS")]
    public Sc_Ladder RopePrefab;
    public LineRenderer RopeLine;

    [Header("ROPE PARAMETERS")]
    public float ForwardCheckDistance = 1f;
    public float MaximumRopeHeight = 4f;
    public float MinimumRopeHeight = 1f;
    public LayerMask RopeLayers;

    private bool _foundAnchorPoint = false;
    private RaycastHit _wallHit;
    private RaycastHit _floorHit;
    private RaycastHit _topWallHit;
    private Vector3 _floorPoint = Vector3.zero;
    private float _calculatedHeight = 0f;

    private void Update()
    {
        if (IsEquipped)
        {
            if (!_inInventory.IsAiming)
            {
                //Check in front of user. If find wall, then check for foot of the wall. Once foot is found, check total height of wall.
                _foundAnchorPoint = CheckForRopeAnchor();
            }
        }
        else
        {
            RopeLine.enabled = false;
        }
    }

    public override void UseItem()
    {
        if (_foundAnchorPoint)
        {
            Sc_Ladder newRope = Instantiate<Sc_Ladder>(RopePrefab, Sc_Level.instance.transform);
            newRope.CreateLadder(_floorHit.point, _calculatedHeight, _wallHit.normal);
            Destroy(newRope);

            if (_inInventory != null)
            {
                _inInventory.DropItem(this);
            }
            Destroy(this.gameObject);
        }
    }

    private bool CheckForRopeAnchor()
    {
        //Check forward player.
        Vector3 fromPos = _inInventory.Character.transform.position + Vector3.up;
        Vector3 forward = _inInventory.Character.transform.forward;
        if (Physics.Raycast(fromPos, forward, out _wallHit, ForwardCheckDistance, RopeLayers, QueryTriggerInteraction.Ignore))
        {
            //Find floor.
            fromPos = _wallHit.point + (_wallHit.normal * .1f);
            if (Physics.Raycast(fromPos, Vector3.down, out _floorHit, MaximumRopeHeight, RopeLayers, QueryTriggerInteraction.Ignore))
            {
                //Check if nothing above floor
                fromPos = _floorHit.point + (Vector3.up*.1f);
                if (!Physics.Raycast(fromPos, Vector3.up, MaximumRopeHeight, RopeLayers, QueryTriggerInteraction.Ignore))
                {
                    //Check wall height
                    fromPos = _wallHit.point - (_wallHit.normal * .1f) + (Vector3.up * MaximumRopeHeight);
                    if (Physics.Raycast(fromPos, Vector3.down, out _topWallHit, MaximumRopeHeight, RopeLayers, QueryTriggerInteraction.Ignore))
                    {
                        //Check if top of wall is flat
                        if (_topWallHit.normal == Vector3.up)
                        {
                            Vector3 floorHitY = new Vector3(0f, _floorHit.point.y, 0f);
                            Vector3 topWallHitY = new Vector3(0f, _topWallHit.point.y, 0f);
                            _calculatedHeight = Vector3.Distance(floorHitY, topWallHitY);
                            if (_calculatedHeight <= MaximumRopeHeight && _calculatedHeight >= MinimumRopeHeight)
                            {
                                //Verify if exit point is not in wall
                                fromPos = _topWallHit.point + (Vector3.up * .1f);
                                if (!Physics.Raycast(fromPos, Vector3.up, 2f, RopeLayers, QueryTriggerInteraction.Ignore))
                                {
                                    fromPos = _topWallHit.point + (Vector3.up * 2.1f);
                                    if (!Physics.Raycast(fromPos, Vector3.down, 2f, RopeLayers, QueryTriggerInteraction.Ignore))
                                    {
                                        RopeLine.enabled = true;
                                        RopeLine.SetPosition(0, _floorHit.point);
                                        RopeLine.SetPosition(1, _floorHit.point + (Vector3.up * _calculatedHeight));
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        RopeLine.enabled = false;
        return false;
    }
}
