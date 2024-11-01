using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Valve_Floor : Sc_GaugeInputer
{
    [Header("OBJECT REFS")]
    public Sc_Interactible _interactible;

    [Header("ANCHOR")]
    public Transform _anchor;

    [Header("VALVE PARAMETERS")]
    public float RotationSpeed = 5f;
    public bool _onlyAllowPositiveInput = false;

    private Sc_Character _currentUser;
    private Vector3 _rectifiedInput = Vector3.zero;
    private bool _isBeingUsed = false;
    private float _deltaRotation = 0f;
    private float _lastRot = 0f;
    public bool IsPositiveInput { get { return _deltaRotation >= 0 ? true : false; } }

    private Vector3 _toDir = Vector3.zero;
    private Vector3 _seDir;
    private Vector3 _sDir;
    private Vector3 _swDir;
    private Vector3 _wDir;
    private Vector3 _nwDir;
    private Vector3 _nDir;
    private Vector3 _neDir;
    private Vector3 _eDir;

    private void Start()
    {
        SetCardinalDirs();
    }

    private void SetCardinalDirs()
    {
        _seDir = transform.forward;
        _sDir = Quaternion.Euler(0, 45, 0) * transform.forward;
        _swDir = Quaternion.Euler(0, 90, 0) * transform.forward;
        _wDir = Quaternion.Euler(0, 135, 0) * transform.forward;
        _nwDir = Quaternion.Euler(0, 180, 0) * transform.forward;
        _nDir = Quaternion.Euler(0, 225, 0) * transform.forward;
        _neDir = Quaternion.Euler(0, 270, 0) * transform.forward;
        _eDir = Quaternion.Euler(0, 315, 0) * transform.forward;
    }

    public void OnInteract(Sc_Character interactor)
    {
        if (_currentUser != null)
        {
            if (interactor == _currentUser)
            {
                EndUsing();
            }
            else
            {
                return;
            }
        }

        StartUsing(interactor);
    }

    private void Update()
    {
        HandleRotation();

        _deltaRotation = _lastRot - transform.eulerAngles.y;
        _lastRot = transform.eulerAngles.y;

        if (_deltaRotation > 50 || _deltaRotation < -50)
        {
            //Don't input, euler stuff going 360 degrees around itself.
        }
        else
        {
            if (_onlyAllowPositiveInput)
            {
                if (IsPositiveInput)
                {
                    ApplyInput();
                }
            }
            else
            {
                ApplyInput();
            }

        }
    }

    private void HandleRotation()
    {
        if (_toDir == Vector3.zero) return;

        Vector3 smoothedLookInputDirection = Vector3.MoveTowards(transform.forward, _toDir, RotationSpeed*Time.deltaTime).normalized;
        transform.forward = smoothedLookInputDirection;
    }

    public void StartUsing(Sc_Character user)
    {
        _interactible.CanBeInteractedWith = false;

        _currentUser = user;
        _isBeingUsed = true;
        _lastRot = transform.eulerAngles.y;

        user.Controller.ConnectToValve(this);
    }

    public void EndUsing()
    {
        _interactible.CanBeInteractedWith = true;

        _currentUser.Controller.DisconnectFromCurrentValve();

        _currentUser = null;
        _isBeingUsed = false;
    }

    public void SetInput(Vector3 moveInputVector)
    {
        if (moveInputVector.magnitude <= 0f) return;

        Vector3 reorientedInput = moveInputVector;
        reorientedInput = Vector3.ProjectOnPlane(reorientedInput, Vector3.up);
        reorientedInput = reorientedInput.normalized;
        _rectifiedInput = reorientedInput;
        _toDir = GetCardinalDirFromReorientedInput(reorientedInput);
    }

    protected override void ApplyInput()
    {
        float adjustedInput = _deltaRotation * _inputRate * Time.deltaTime;

        foreach (Sc_Gauge gauge in _gauges)
        {
            gauge.Input(adjustedInput);
        }
    }

    private Vector3 GetCardinalDirFromReorientedInput(Vector3 reorientedInput)
    {
        //float upDot = Vector3.Dot(reorientedInput, _nDir);
        //float rightDot = Vector3.Dot(reorientedInput, _eDir);

        Vector3 cardinalDir = Vector3.zero;
        float angleNorth = Vector3.Angle(reorientedInput, _nDir);
        float angleEast = Vector3.Angle(reorientedInput, _eDir);
        if (angleNorth <= 22.5f)
        {
            //North.
            cardinalDir = _nDir;
        }
        else if (angleNorth >= 157.5f)
        {
            //South.
            cardinalDir = _sDir;
        }
        else if (angleEast <= 22.5f)
        {
            //East.
            cardinalDir = _eDir;
        }
        else if (angleEast >= 157.5f)
        {
            //West.
            cardinalDir = _wDir;
        }
        else
        {
            if (angleNorth <= 90f && angleEast <= 90f)
            {
                //Northeast.
                cardinalDir = _neDir;
            }
            else if (angleNorth <= 90f && angleEast >= 90f)
            {
                //Northwest.
                cardinalDir = _nwDir;
            }
            else if (angleNorth >= 90f && angleEast <= 90f)
            {
                //Southeast.
                cardinalDir = _seDir;
            }
            else if (angleNorth >= 90f && angleEast >= 90f)
            {
                //Southwest.
                cardinalDir = _swDir;
            }
        }

        return cardinalDir;
    }

    private void OnDrawGizmos()
    {
        if (_seDir != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + _seDir);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + _sDir);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + _nDir);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + _eDir);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + _wDir);
        }
    }
}
