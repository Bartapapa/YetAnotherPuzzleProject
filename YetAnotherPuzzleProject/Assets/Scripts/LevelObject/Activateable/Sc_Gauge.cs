using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_Gauge : MonoBehaviour
{
    [Header("UNITYEVENTS")]
    public UnityEvent<float> OnCompletionChanged;

    [Header("PARAMETERS")]
    [SerializeField][ReadOnly] private float _completion = 0f;
    public float Completion { get { return _completion; } }
    public float _completionRate = 5f;
    public float _depletionRate = 0f;
    public float _maxCompletion = 1.2f;


    private float _toCompletion = 0f;

    private void Update()
    {
        HandleCompletionDrain();
        HandleCompletionInterpolation();
    }

    private void HandleCompletionDrain()
    {
        if (_depletionRate > 0f)
        {
            if (_toCompletion > 0f)
            {
                _toCompletion -= _depletionRate * Time.deltaTime;
                if (_toCompletion < 0f) _toCompletion = 0f;
            }
        }
    }

    private void HandleCompletionInterpolation()
    {
        if (_completion != _toCompletion)
        {
            if (_completion < _toCompletion)
            {
                _completion += _completionRate * Time.deltaTime;
                if (_completion > _toCompletion) _completion = _toCompletion;
            }
            else
            {
                _completion -= _completionRate * Time.deltaTime;
                if (_completion < _toCompletion) _completion = _toCompletion;
            }

            OnCompletionChanged?.Invoke(_completion);
        }
    }

    public void Input(float input)
    {
        _toCompletion += input;
        if (_toCompletion > _maxCompletion) _toCompletion = _maxCompletion;
        if (_toCompletion < 0f) _toCompletion = 0f;
    }
}
