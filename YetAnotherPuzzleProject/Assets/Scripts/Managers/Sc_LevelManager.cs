using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_LevelManager : MonoBehaviour
{
    public static Sc_LevelManager instance { get; private set; }

    [Header("LEVEL")]
    public Sc_Level _currentLevel;

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

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Sc_Player player = Sc_PlayerManager.instance.GetPlayerFromPInput(playerInput);
        if (player != null)
        {
            player.InitializePlayerCharacter(_currentLevel._spawnPoints[Sc_PlayerManager.instance.CurrentPlayers.Count - 1].position);
        }
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        //Sc_Player player = Sc_PlayerManager.instance.GetPlayerFromPInput(playerInput);
        //if (player != null)
        //{
        //    CameraManager.UnregisterPOI(player.CurrentCharacter.transform);
        //}
    }
}
