using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_LevelManager : MonoBehaviour
{
    public static Sc_LevelManager instance { get; private set; }

    [Header("LEVEL")]
    public Sc_Level _currentLevel;

    private int _spawnedPlayers = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    #region PLAYERSPAWNING
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Sc_Player player = Sc_PlayerManager.instance.GetPlayerFromPInput(playerInput);
        if (player != null)
        {
            //If we're in-game, otherwise skip this.
            Transform spawnPoint = _currentLevel.GetSpawnPoint(_spawnedPlayers);
            if (spawnPoint != null)
            {
                player.InitializePlayerCharacter(spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                player.InitializePlayerCharacter(Vector3.zero, Quaternion.identity);
            }
            _spawnedPlayers++;
        }
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Sc_Player player = Sc_PlayerManager.instance.GetPlayerFromPInput(playerInput);
        if (player != null)
        {
            if (player.PlayerCharacter != null)
            {
                Sc_CameraManager.instance.RemoveFocus(Sc_CameraManager.instance._defaultCameraFocus, player.PlayerCharacter.transform);
            }
            _spawnedPlayers--;
        }
    }

    public void ResetSpawnedPlayerCount()
    {
        _spawnedPlayers = 0;
    }
    #endregion
}
