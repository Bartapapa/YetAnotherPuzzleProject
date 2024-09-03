using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Button : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Activateable _activateable;
    public Sc_Interactible _interactible;
    public Transform _buttonPivot;

    [Header("PARAMETERS")]
    public float _activationDuration = 1f;
    private float _activationTimer = 0f;

    private bool _buttonPushed = false;

    private void Update()
    {
        if (_buttonPushed)
        {
            _activationTimer += Time.deltaTime;
            if (_activationTimer >= _activationDuration)
            {
                LiftButton();
                _activationTimer = 0f;
            }
        }
    }
    public void OnInteract()
    {
        PushButton();
    }

    private void PushButton()
    {
        _buttonPushed = true;
        _interactible.CanBeInteractedWith = false;
        _activateable.ToggleActivation();

        _buttonPivot.localPosition = new Vector3(0f, 0f, -.1f);
    }

    private void LiftButton()
    {
        _buttonPushed = false;
        _interactible.CanBeInteractedWith = true;
        _activateable.ToggleActivation();

        _buttonPivot.localPosition = new Vector3(0f, 0f, 0f);
    }
}
