using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Pushable_Cylinder : Sc_Pushable
{
    [Header("Cylinder ELEMENTS")]
    public Transform _rightElementAnchor;
    public Transform _leftElementAnchor;
    public BlockElements _rightCylinderElement = BlockElements.None;
    public BlockElements _leftCylinderElement = BlockElements.None;
    public Sc_BatteryHole _batteryHolePrefab;

    protected override void InitializePushable()
    {
        base.InitializePushable();

        InitializeCylinderElements();
    }

    private void InitializeCylinderElements()
    {
        SpawnCylinderElement(BlockFace.Right, _rightCylinderElement);
        SpawnCylinderElement(BlockFace.Left, _leftCylinderElement);
    }

    private void SpawnCylinderElement(BlockFace face, BlockElements blockElement)
    {
        if (blockElement == BlockElements.None) return;

        Transform spawnTransform = null;

        switch (face)
        {
            case BlockFace.Right:
                spawnTransform = _rightElementAnchor;
                break;
            case BlockFace.Left:
                spawnTransform = _leftElementAnchor;
                break;
        }

        switch (blockElement)
        {
            case BlockElements.BatteryHole:
                if (_activateable == null) return;
                Sc_BatteryHole newBatteryHole = Instantiate<Sc_BatteryHole>(_batteryHolePrefab, spawnTransform.position, spawnTransform.rotation, spawnTransform);
                //newBatteryHole._activateable.OnActivate.AddListener(_activateable.Activate);
                break;
            case BlockElements.Handle:
                break;
        }
    }

    public override void Push(Vector3 direction)
    {
        if (CheckObstacle(direction) || _onSlope)
        {
            return;
        }
        float dot = Vector3.Dot(direction, transform.forward);
        if (Mathf.Abs(dot)<= .98f)
        {
            return;
        }
        _pushedDirection = direction;
    }
}
