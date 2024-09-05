using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public struct CharacterInput
{
    public float moveX;
    public float moveY;
    public Camera cameraRef;
}

public class Sc_Player : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private Sc_Character_Player _characterPrefab;
    [SerializeField] private Sc_Character_Player _playerCharacter;
    
    private Vector2 _movement;

    void Update()
    {
        if (_playerCharacter != null)
        {
            HandlePlayerInputs();
        }
    }


    #region INITIALIZATION

    private void OnEnable()
    {
        if (Sc_LevelManager.instance == null)
        {
            InitializePlayerCharacter(Vector3.zero);
        }
    }

    private void OnDisable()
    {
        
    }

    private void OnDestroy()
    {
        if (_playerCharacter != null)
        {
            Destroy(_playerCharacter.gameObject);
        }
    }

    public void InitializePlayerCharacter(Vector3 spawnPoint)
    {
        if (_playerCharacter == null)
        {
            Sc_Character_Player newCharacter = Instantiate<Sc_Character_Player>(_characterPrefab, spawnPoint, Quaternion.identity);
            _playerCharacter = newCharacter;
        }
        else
        {
            _playerCharacter.Controller.RB.MovePosition(spawnPoint);
        }

        //if (Sc_LevelManager.instance != null)
        //{
        //    Sc_LevelManager.instance.CameraManager.RegisterPOI(_currentCharacter.transform);
        //}
    }

    #endregion
    #region INPUTS
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
    #endregion

}
