using System;
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
public class Sc_Pushable : MonoBehaviour
{
    //[Header("OBJECT REFS")]
    //public Sc_WeightedObject weightedObject;
    //private List<Sc_WeightedObject> _registeredObjects = new List<Sc_WeightedObject>();

    [Header("OBJECT REFS")]
    public Sc_Activateable _activateable;

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

    [Header("MOVEMENT")]
    public float _maxSpeed = 1f;
    public float _speedSharpness = 15f;

    [Header("GROUND")]
    public LayerMask _groundLayers;
    private RaycastHit _groundHit;

    private Rigidbody _rb;

    private void Start()
    {
        InitializePushable();
    }

    private void InitializePushable()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogWarning(this.name + " doesn't have a Rigidbody!");
            return;
        }

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
                if (_activateable == null) return;
                Sc_BatteryHole newBatteryHole = Instantiate<Sc_BatteryHole>(_batteryHolePrefab, spawnTransform.position, spawnTransform.rotation, spawnTransform);
                newBatteryHole._activateable.OnActivate.AddListener(_activateable.Activate);
                break;
            case BlockElements.Handle:
                break;
        }
    }

    public void Push(Vector3 direction)
    {
        Vector3 targetMovementVelocity = direction * _maxSpeed;
        _rb.velocity = Vector3.Lerp(_rb.velocity, targetMovementVelocity, 1f - Mathf.Exp(-_speedSharpness * Time.fixedDeltaTime));
    }

    private bool CheckObstacle()
    {
        Physics.BoxCast(transform.position + Vector3.up, new Vector3(1.1f, 1.1f, 1.1f), Vector3.down, out _groundHit, transform.rotation, _groundLayers);
        float angle = Vector3.Angle(Vector3.up, _groundHit.normal);
        return (angle != 0 && angle <= 1) ? true : false;
    }

    //private void RegisterObject(Sc_WeightedObject wObject)
    //{
    //    _registeredObjects.Add(wObject);
    //    weightedObject._weight += wObject._weight;
    //}

    //private void UnregisterObject(Sc_WeightedObject wObject)
    //{
    //    if (!_registeredObjects.Contains(wObject)) return;

    //    _registeredObjects.Remove(wObject);
    //    weightedObject._weight -= wObject._weight;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    Sc_WeightedObject wObject = other.GetComponent<Sc_WeightedObject>();
    //    if (wObject) RegisterObject(wObject);
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    Sc_WeightedObject wObject = other.GetComponent<Sc_WeightedObject>();
    //    if (wObject) UnregisterObject(wObject);
    //}
}