using Cinemachine;
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

    [Header("FOLLOW PARAMETERS")]
    public bool UseFocus = true;
    public float _cameraFollowSpeed = 1f;
    public bool _focusOriginalPosition = false;
    public int _selfWeight = 1;

    [Header("AIM PARAMETERS")]
    public bool UseAim = false;
    public Transform AimParent;
    public float AimSpeed = 30f;
    public float CameraDistance = 20f;
    public Vector2 MinMaxXAim = new Vector2(10f, 80f);

    private CinemachineVirtualCamera _defaultCamera;
    private Vector2 _input = Vector2.zero;
    private Vector3 _originalPosition;
    private Vector2 _desiredInput = Vector2.zero;

    private void Start()
    {
        _defaultCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _originalPosition = transform.position;

        if (UseAim)
        {
            AimParent.forward = _defaultCamera.transform.forward;
            this.transform.parent = AimParent;
            transform.position = AimParent.position - (AimParent.forward * CameraDistance);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void LateUpdate()
    {
        if (UseFocus)
        {
            HandleFocus();
        }
        if (UseAim)
        {
            HandleAim();
        }
    }

    private void HandleAim()
    {
        _input = Vector2.MoveTowards(_input, _desiredInput, 50f * Time.deltaTime);

        AimParent.localEulerAngles += new Vector3(-_input.y * AimSpeed * Time.deltaTime, _input.x * AimSpeed * Time.deltaTime, 0f);
        AimParent.localEulerAngles = new Vector3(Mathf.Clamp(AimParent.localEulerAngles.x, MinMaxXAim.x, MinMaxXAim.y), AimParent.localEulerAngles.y, AimParent.localEulerAngles.z);
        //AimParent.Rotate(_in, _input.x * Time.deltaTime, 0f);

        _desiredInput = Vector2.zero;
    }

    public void AddAimInput(Vector2 aimInput)
    {
        _desiredInput += aimInput;
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
