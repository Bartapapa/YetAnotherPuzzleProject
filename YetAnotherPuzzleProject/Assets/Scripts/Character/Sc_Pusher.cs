using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Pusher : MonoBehaviour
{
    [Header("PARAMETERS")]
    public float _pushCheckDistance = 1f;
    public float _characterHeight = 1.4f;
    public float _characterRadius = .5f;

    [Header("PUSHING")]
    public float _pushRequestTime = .5f;
    public LayerMask _pushableObjectLayers;
    private RaycastHit _pushableHit;
    private float _pushRequestTimer = 0f;
    private bool _pushRequested = false;
    private Sc_Pushable _currentPushable;

    public delegate void DefaultEvent();
    public event DefaultEvent PushStart;
    public event DefaultEvent PushEnd;

    private void FixedUpdate()
    {
        CheckRequestPush();
    }

    private void CheckRequestPush()
    {
        if (Physics.Raycast(transform.position+new Vector3(0, (_characterHeight*.5f), 0f), transform.forward, out _pushableHit, _pushCheckDistance, _pushableObjectLayers))
        {
            _currentPushable = _pushableHit.collider.GetComponent<Sc_Pushable>();
            if (_currentPushable)
            {
                float dot = Vector3.Dot(_pushableHit.normal, transform.forward);
                if (-dot > .8f)
                {
                    _pushRequested = true;
                }
                else
                {
                    _pushRequested = false;
                }
            }
        }
        else
        {
            _currentPushable = null;
        }
    }

    private void StartPush()
    {

    }

    private void EndPush()
    {

    }
}
