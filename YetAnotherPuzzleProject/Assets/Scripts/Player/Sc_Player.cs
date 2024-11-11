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
    public float aimX;
    public float aimY;
    public Camera cameraRef;
}

public class Sc_Player : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private Sc_Character_Player _characterPrefab;
    [SerializeField] private Sc_Character_Player _playerCharacter;
    public Sc_Character_Player PlayerCharacter { get { return _playerCharacter; } }
    
    private Vector2 _movement;
    private Vector2 _cameraAim;

    private bool _restartRequested = false;
    private float _restartConfirmationDuration = .75f;
    private float _restartConfirmationTimer = 0f;

    void Update()
    {
        if (_playerCharacter != null)
        {
            HandlePlayerInputs();
        }

        HandleRestartRequest();
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

    public void OnCameraLook(InputAction.CallbackContext context)
    {
        _cameraAim = context.ReadValue<Vector2>();
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
    public void OnThrow(InputAction.CallbackContext context)
    {
        //Should have aiming stance first, then on release have the throw. Draw a trajectory line during this period.

        if (context.performed)
        {
            if (!_playerCharacter.Inventory.CanAim) return;

            if (context.action.activeControl.device.displayName == "Keyboard" || context.action.activeControl.device.displayName == "Mouse")
            {
                //Change character rotationSpeed
                _playerCharacter.Controller._rotationSharpness = _playerCharacter.AimingRotationSpeed;
            }

            _playerCharacter.Inventory.AimThrow();
        }
        if (context.canceled)
        {
            if (!_playerCharacter.Inventory.IsAiming) return;

            if (context.action.activeControl.device.displayName == "Keyboard" || context.action.activeControl.device.displayName == "Mouse")
            {
                //Change character rotationSpeed
                _playerCharacter.Controller._rotationSharpness = _playerCharacter.BaseRotationSpeed;
            }

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
            if (_playerCharacter.Inventory.IsUsingItem)
            {
                _playerCharacter.Inventory.CurrentlyHeldItem.UseItemSpecial(0);
            }
            else
            {
                _playerCharacter.Inventory.EquipFromInventory(0);
            }

        }
    }

    public void OnItem2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_playerCharacter.Inventory.IsUsingItem)
            {
                _playerCharacter.Inventory.CurrentlyHeldItem.UseItemSpecial(1);
            }
            else
            {
                _playerCharacter.Inventory.EquipFromInventory(1);
            }

        }
    }

    public void OnItem3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_playerCharacter.Inventory.IsUsingItem)
            {
                _playerCharacter.Inventory.CurrentlyHeldItem.UseItemSpecial(2);
            }
            else
            {
                _playerCharacter.Inventory.EquipFromInventory(2);
            }

        }
    }

    public void OnDropItem(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_playerCharacter.Inventory.IsUsingItem)
            {
                _playerCharacter.Inventory.CurrentlyHeldItem.UseItemSpecial(3);
            }
            else
            {
                _playerCharacter.Inventory.DropCurrentItem();

            }

        }
    }

    public void OnRestartLevel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _restartConfirmationTimer = 0f;
            _restartRequested = true;
            PlayerCharacter.RestartCircle.Anim.Play("FadeIn");
        }

        if (context.canceled)
        {
            _restartRequested = false;
            PlayerCharacter.RestartCircle.Anim.Play("FadeOut");
        }
    }

    private void HandleRestartRequest()
    {
        if (_restartRequested)
        {
            if (_restartConfirmationTimer < _restartConfirmationDuration)
            {
                _restartConfirmationTimer += Time.deltaTime;
                PlayerCharacter.RestartCircle.FillCircle(_restartConfirmationTimer / _restartConfirmationDuration);
            }
            else
            {
                _restartRequested = false;
                Sc_GameManager.instance.ReloadCurrentLevel();
                PlayerCharacter.RestartCircle.Anim.Play("FadeOut");
            }
        }
    }

    private void OnRestartLevelConfirmed()
    {

    }

    private void HandlePlayerInputs()
    {
        CharacterInput playerInput = new CharacterInput();

        playerInput.moveX = _movement.x;
        playerInput.moveY = _movement.y;

        playerInput.aimX = _cameraAim.x;
        playerInput.aimY = _cameraAim.y;

        playerInput.cameraRef = Camera.main;

        _playerCharacter.Controller.SetInputs(ref playerInput);

        if (Sc_CameraManager.instance != null)
        {
            Sc_CameraManager.instance.AimCamera(playerInput);
        }
    }
    #endregion

}
