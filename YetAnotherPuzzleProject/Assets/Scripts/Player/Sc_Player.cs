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

            Sc_GameManager.instance.SavePlayerCharacterData();
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
        PlayerCharacter.Inventory.ResetInventory();

        PlayerCharacter.Controller.ParentToObject(this.transform);
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
            else if (_playerCharacter.Controller.IsClimbing)
            {
                _playerCharacter.Controller.StopClimbing();
            }
            else if (_playerCharacter.Interactor.CurrentSelectedInteractible != null)
            {
                _playerCharacter.Interactor.CurrentSelectedInteractible.Interact(_playerCharacter);
            }
            else
            {
                _playerCharacter.SoundHandler.Quack();
            }
        }
    }

    public void OnQuack(InputAction.CallbackContext context)
    {
        //if (context.performed)
        //{
        //    _playerCharacter.SoundHandler.Quack();
        //}
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Should have aiming stance first, then on release have the throw. Draw a trajectory line during this period.
            
        }

        if (context.performed)
        {
            if (!_playerCharacter.Inventory.CanAim) return;
            _playerCharacter.Inventory.AimThrow();
        }
        if (context.canceled)
        {
            if (!_playerCharacter.Inventory.IsAiming) return;
            _playerCharacter.Inventory.StopAiming();
            _playerCharacter.Inventory.ThrowCurrentItem();
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerCharacter.Inventory.UseCurrentItem();
        }
    }

    public void OnItem1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerCharacter.Inventory.EquipFromInventory(0);
        }
    }

    public void OnItem2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerCharacter.Inventory.EquipFromInventory(1);
        }
    }

    public void OnItem3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerCharacter.Inventory.EquipFromInventory(2);
        }
    }

    public void OnDropItem(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerCharacter.Inventory.DropCurrentItem();
        }
    }

    public void OnRestartLevel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Sc_GameManager.instance.ReloadCurrentLevel();
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
