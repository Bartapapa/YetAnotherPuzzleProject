using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_PlayerManager : MonoBehaviour
{
    public static Sc_PlayerManager instance { get; private set; }

    [SerializeField] private InputAction _joinAction;
    [SerializeField] private InputAction _leaveAction;

    [Header("Players")]
    [SerializeField] private List<PlayerInput> _currentPlayers = new List<PlayerInput>();
    public List<PlayerInput> CurrentPlayers => _currentPlayers;

    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;

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

        _joinAction.Enable();
        _joinAction.performed += context => JoinAction(context);
        _leaveAction.Enable();
        _leaveAction.performed += context => LeaveAction(context);
    }

    private void Start()
    {
        if (Sc_LevelManager.instance != null)
        {
            PlayerInputManager.instance.playerJoinedEvent.AddListener(Sc_LevelManager.instance.OnPlayerJoined);
            PlayerInputManager.instance.playerLeftEvent.AddListener(Sc_LevelManager.instance.OnPlayerLeft);
        }

        PlayerInputManager.instance.JoinPlayer(0, -1, null);
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        _currentPlayers.Add(input);

        if (PlayerJoinedGame != null)
        {
            PlayerJoinedGame(input);
        }
    }

    public void OnPlayerLeft(PlayerInput input)
    {
        _currentPlayers.Remove(input);
        Destroy(input.gameObject);
    }

    void JoinAction(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }

    void LeaveAction(InputAction.CallbackContext context)
    {
        if (_currentPlayers.Count > 1)
        {
            foreach (var player in _currentPlayers)
            {
                foreach (var device in player.devices)
                {
                    if (device != null && context.control.device == device)
                    {
                        UnregisterPlayer(player);
                        return;
                    }
                }
            }
        }
    }

    void UnregisterPlayer(PlayerInput input)
    {
        _currentPlayers.Remove(input);

        if (PlayerLeftGame != null)
        {
            PlayerLeftGame(input);
        }

        Destroy(input.gameObject);
    }

    public List<Sc_Player> GetPlayersFromPInputs()
    {
        List<Sc_Player> players = new List<Sc_Player>();

        foreach (PlayerInput playerInput in _currentPlayers)
        {

            Sc_Player player = playerInput.GetComponent<Sc_Player>();
            if (player != null)
            {
                players.Add(player);
            }
        }

        return players;
    }

    public Sc_Player GetPlayerFromPInput(PlayerInput input)
    {
        Sc_Player player = input.GetComponent<Sc_Player>();
        return player;
    }
}
