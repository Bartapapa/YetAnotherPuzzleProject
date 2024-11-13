using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockFace
{
    Front,
    Back,
    Right,
    Left,
}
public enum BlockElements
{
    None,
    BatteryHole,
    Handle,
}
public class Sc_Pushable_Block : Sc_Pushable
{
    [Header("BLOCK ELEMENTS")]
    public Transform _frontElementAnchor;
    public Transform _backElementAnchor;
    public Transform _rightElementAnchor;
    public Transform _leftElementAnchor;
    public BlockElements _frontBlockElement = BlockElements.None;
    public BlockElements _backBlockElement = BlockElements.None;
    public BlockElements _rightBlockElement = BlockElements.None;
    public BlockElements _leftBlockElement = BlockElements.None;
    public Sc_BatteryHole _batteryHolePrefab;

    protected override void InitializePushable()
    {
        base.InitializePushable();

        InitializeBlockElements();
    }

    private void InitializeBlockElements()
    {
        SpawnBlockElement(BlockFace.Front, _frontBlockElement);
        SpawnBlockElement(BlockFace.Back, _backBlockElement);
        SpawnBlockElement(BlockFace.Right, _rightBlockElement);
        SpawnBlockElement(BlockFace.Left, _leftBlockElement);
    }

    private void SpawnBlockElement(BlockFace face, BlockElements blockElement)
    {
        if (blockElement == BlockElements.None) return;

        Transform spawnTransform = null;

        switch (face)
        {
            case BlockFace.Front:
                spawnTransform = _frontElementAnchor;
                break;
            case BlockFace.Back:
                spawnTransform = _backElementAnchor;
                break;
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
                Sc_BatteryHole newBatteryHole = Instantiate<Sc_BatteryHole>(_batteryHolePrefab, spawnTransform.position, spawnTransform.rotation, spawnTransform);
                newBatteryHole.Pushable = this;
                break;
            case BlockElements.Handle:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (_frontBlockElement != BlockElements.None)
        {
            Gizmos.DrawSphere(_frontElementAnchor.position, .15f);
        }
        if (_rightBlockElement != BlockElements.None)
        {
            Gizmos.DrawSphere(_rightElementAnchor.position, .15f);
        }
        if (_leftBlockElement != BlockElements.None)
        {
            Gizmos.DrawSphere(_leftElementAnchor.position, .15f);
        }
        if (_backBlockElement != BlockElements.None)
        {
            Gizmos.DrawSphere(_backElementAnchor.position, .15f);
        }
    }
}
