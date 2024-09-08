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
    public Sc_Character_Player PlayerCharacter { get { return _playerCharacter; } }
    
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
        //if (Sc_LevelManager.instance == null)
        //{
        //    InitializePlayerCharacter(Vector3.zero, Quaternion.identity);
        //}
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

    public void InitializePlayerCharacter(Vector3 spawnPoint, Quaternion rot)
    {
        if (_playerCharacter == null)
        {
            Sc_Character_Player newCharacter = Instantiate<Sc_Character_Player>(_characterPrefab, spawnPoint, Quaternion.identity, this.transform);
            _playerCharacter = newCharacter;
            newCharacter.name = "PlayerCharacter" + Sc_GameManager.instance.PlayerManager.CurrentPlayers.Count;

            if (Sc_CameraManager.instance)
            {
                Sc_CameraManager.instance.AddFocus(Sc_CameraManager.instance._defaultCameraFocus, _playerCharacter.transform);
            }
        }
        else
        {
            ResetPlayerCharacter();
            _playerCharacter.Controller.RB.Move(spawnPoint, rot);

            if (Sc_CameraManager.instance)
            {
                if (!Sc_CameraManager.instance.DoesCameraFocusHaveFocus(Sc_CameraManager.instance._defaultCameraFocus, _playerCharacter.transform))
                {
                    Sc_CameraManager.instance.AddFocus(Sc_CameraManager.instance._defaultCameraFocus, _playerCharacter.transform);
                }
                else
                {
                    Debug.LogWarning(_playerCharacter.gameObject.transform + " is already a focus of the camera manager. Not adding.");
                }
            }
        }

        //if (Sc_LevelManager.instance != null)
        //{
        //    Sc_LevelManager.instance.CameraManager.RegisterPOI(_currentCharacter.transform);
        //}
    }

    public void ResetPlayerCharacter()
    {
        if (PlayerCharacter == null) return;

        PlayerCharacter.Interactor.ClearPotentialInteractibles();
        PlayerCharacter.Controller.StopAnchoringSequence();
        PlayerCharacter.Controller.ResetAnchor();
        PlayerCharacter.Controller.StopClimbing();
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
            if (_playerCharacter.Controller.IsAnchoredToValve)
            {
                _playerCharacter.Controller.CurrentValve.EndUsing();
            }

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
            _playerCharacter.SoundHandler.Quack();
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
