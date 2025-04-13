using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum CodeDirections
{
    Up,
    Down,
    Right,
    Left
}

[System.Serializable]
public class Code
{
    public List<CodeDirections> CodeSequence = new List<CodeDirections>();
}
public class Sc_RoboArmInputListener : MonoBehaviour
{
    [Header("CODE PARAMETERS")]
    public UnityEvent<int> CodeComplete;
    public List<Code> Codes = new List<Code>();

    [ReadOnly][SerializeField] private int _codeIndex = -1;
    [ReadOnly][SerializeField] private int _codeSequenceStep = -1;

    private bool _inputCodeElement = false;

    public delegate void DefaultEvent();
    public event DefaultEvent CodeUp;
    public event DefaultEvent CodeDown;
    public event DefaultEvent CodeRight;
    public event DefaultEvent CodeLeft;

    public void OnRoboArmChannelInput(ref CharacterInput playerInput)
    {
        TranslateChannelInput(ref playerInput);
    }
    protected virtual void TranslateChannelInput(ref CharacterInput playerInput)
    {
        //Apply method when channeling
    }
    public void OnRoboArmCodeInput(ref CharacterInput playerInput)
    {
        TranslateCodeInput(ref playerInput);
    }
    private void TranslateCodeInput(ref CharacterInput playerInput)
    {
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(playerInput.moveX, 0f, playerInput.moveY), 1f);
        if (moveInputVector.sqrMagnitude <= .25f)
        {
            _inputCodeElement = false;
        }
        if (!_inputCodeElement)
        {
            if (moveInputVector.x >= .85f)
            {
                Debug.Log("CODE RIGHT");
                _inputCodeElement = true;
                CodeRight?.Invoke();
                CheckCode(CodeDirections.Right);
            }
            else if (moveInputVector.x <= -.85f)
            {
                Debug.Log("CODE LEFT");
                _inputCodeElement = true;
                CodeLeft?.Invoke();
                CheckCode(CodeDirections.Left);
            }
            else if (moveInputVector.z >= .85f)
            {
                Debug.Log("CODE UP");
                _inputCodeElement = true;
                CodeUp?.Invoke();
                CheckCode(CodeDirections.Up);
            }
            else if (moveInputVector.z <= -.85f)
            {
                Debug.Log("CODE DOWN");
                _inputCodeElement = true;
                CodeDown?.Invoke();
                CheckCode(CodeDirections.Down);
            }
        }
    }

    private bool InitializeCode(CodeDirections initialDirection)
    {
        bool codeInitialized = false;
        for (int i = 0; i < Codes.Count; i++)
        {
            if (initialDirection == Codes[i].CodeSequence[_codeSequenceStep])
            {
                codeInitialized = true;
                _codeIndex = i;
                break;
            }
        }
        return codeInitialized;
    }

    private void CheckCode(CodeDirections direction)
    {
        if (Codes.Count < 1) return;

        _codeSequenceStep++;
        if (_codeIndex <= -1)
        {
            if (InitializeCode(direction))
            {

            }
            else
            {
                ResetCode();
            }
        }
        else
        {
            if (direction == Codes[_codeIndex].CodeSequence[_codeSequenceStep])
            {
                if (_codeSequenceStep >= Codes[_codeIndex].CodeSequence.Count-1)
                {
                    Debug.Log("CODE " + _codeIndex + " COMPLETE!");
                    CodeComplete?.Invoke(_codeIndex);
                    ResetCode();
                }
            }
            else
            {
                ResetCode();
            }
        }       
    }

    private void ResetCode()
    {
        Debug.Log("CODE RESET");
        _codeIndex = -1;
        _codeSequenceStep = -1;
    }
}
