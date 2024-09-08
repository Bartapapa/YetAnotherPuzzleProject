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
    public float _rotationSharpness = 5f;
    public bool _onlyAllowPositiveInput = false;

    private Sc_Character _currentUser;
    private Vector3 _rectifiedInput = Vector3.zero;
    private bool _isBeingUsed = false;
    private float _deltaRotation = 0f;
    private float _lastRot = 0f;
    public bool IsPositiveInput { get { return _deltaRotation >= 0 ? true : false; } }

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
        if (_isBeingUsed)
        {
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
        Vector3 reorientedInput = moveInputVector;
        reorientedInput = Vector3.ProjectOnPlane(reorientedInput, Vector3.up);
        reorientedInput = reorientedInput.normalized;

        Vector3 smoothedLookInputDirection = Vector3.Lerp(transform.forward, reorientedInput, 1 - Mathf.Exp(-_rotationSharpness * Time.fixedDeltaTime)).normalized;
        transform.forward = smoothedLookInputDirection;
    }

    protected override void ApplyInput()
    {
        float adjustedInput = _deltaRotation * _inputRate * Time.deltaTime;

        foreach (Sc_Gauge gauge in _gauges)
        {
            gauge.Input(adjustedInput);
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //if (_rectifiedInput != Vector3.zero)
        //{
        //    Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + _rectifiedInput);
        //}
    }
}
