using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FocusObject
{
    public Transform Focus;
    public int Weight;

    public FocusObject(Transform focus, int weight)
    {
        Focus = focus;
        Weight = weight;
    }
}

public class Sc_CameraFocus : MonoBehaviour
{
    [Header("POIs")]
    [ReadOnly][SerializeField] private List<FocusObject> _focusObjects = new List<FocusObject>();
    public List<FocusObject> FocusObjects { get { return _focusObjects; } }

    [Header("PARAMETERS")]
    public float _cameraFollowSpeed = 1f;
    public bool _focusOriginalPosition = false;
    public int _selfWeight = 1;

    private Vector3 _originalPosition;

    private void Start()
    {
        _originalPosition = transform.position;
    }

    private void LateUpdate()
    {
        HandleFocus();
    }

    private void HandleFocus()
    {
        transform.position = Vector3.Lerp(transform.position, FindAveragePosition(), _cameraFollowSpeed * Time.deltaTime);
    }

    private Vector3 FindAveragePosition()
    {
        Vector3 averagePosition = Vector3.zero;
        int totalWeights = 0;

        if (_focusOriginalPosition)
        {
            for (int i = 0; i < _selfWeight; i++)
            {
                averagePosition += _originalPosition;
                totalWeights++;
            }
        }

        foreach(FocusObject focusObject in _focusObjects)
        {
            for (int i = 0; i < focusObject.Weight; i++)
            {
                averagePosition += focusObject.Focus.position;
                totalWeights++;
            }
        }

        if (totalWeights > 0)
        {
            averagePosition = averagePosition / totalWeights;
        }

        return averagePosition;
    }
}
