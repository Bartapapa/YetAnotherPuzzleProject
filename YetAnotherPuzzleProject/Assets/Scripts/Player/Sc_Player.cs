using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct CharacterInput
{
    public float moveX;
    public float moveY;
    public Camera cameraRef;
}

public class Sc_Player : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private Sc_Character_Player _playerCharacter;
    
    private Vector2 _movement;

    void Update()
    {
        if (_playerCharacter != null)
        {
            HandlePlayerInputs();
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_playerCharacter.Controller.IsClimbing)
            {
                _playerCharacter.Controller.StopClimbing();
            }

            if (_playerCharacter.Interactor.CurrentSelectedInteractible != null)
            {
                _playerCharacter.Interactor.CurrentSelectedInteractible.Interact(_playerCharacter);
            }
        }
    }

    public void OnQuack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerCharacter.Quacker.Quack();
        }
    }

    private void HandlePlayerInputs()
    {
        CharacterInput playerInput = new CharacterInput();

        playerInput.moveX = _movement.x;
        playerInput.moveY = _movement.y;

        playerInput.cameraRef = Camera.main;

        _playerCharacter.Controller.SetInputs(ref playerInput);
    }
}
