using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_RuneLockHandler : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Transform[] _runeLocks;
    [Header("PARAMETERS")]
    public Vector2 _minMaxRotationSpeed;
    private float[] _cachedRotationSpeeds;

    private void OnEnable()
    {
        InitRuneLocks();
    }

    private void Update()
    {
        HandleRuneLockRotation();
    }

    private void HandleRuneLockRotation()
    {
        //for (int i = 0; i < _runeLocks.Length; i++)
        //{
        //    _runeLocks[i].Rotate
        //}
    }

    private void InitRuneLocks()
    {
        _cachedRotationSpeeds = new float[_runeLocks.Length];
        SetRandomRuneLockRotationSpeeds();
    }

    private void SetRandomRuneLockRotationSpeeds()
    {

    }

    private void SetRuneLockRotationSpeed(int runelockIndex, float rotSpeed)
    {

    }
}
