using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    None,
    MainMenu,
    Loading,
    Lobby,
    InGame,
}

public class Sc_GameManager : MonoBehaviour
{
    public static Sc_GameManager instance { get; private set; }

    [Header("GAMESTATE")]
    public GameState _startGameState = GameState.InGame;
    [ReadOnly][SerializeField] private GameState _currentGameState = GameState.None;
    public GameState CurrentGameState
    {
        get
        {
            return _currentGameState;
        }
        set
        {
            TransitionToGameState(value);
        }
    }

    public delegate void GameStateEvent(GameState enteredGameState);
    public event GameStateEvent GameStateChanged;

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

        CurrentGameState = _startGameState;
    }

    private void Start()
    {
        //CurrentGameState = _startGameState;
    }

    #region GAMESTATE
    public void TransitionToGameState(GameState toState)
    {
        if (_currentGameState == toState) return;
        GameState fromState = _currentGameState;

        OnStateExit(fromState);
        _currentGameState = toState;
        OnStateEnter(toState);
        GameStateChanged?.Invoke(toState);
    }

    public void OnStateEnter(GameState enteredState)
    {
        switch (enteredState)
        {
            case GameState.None:
                break;
            case GameState.MainMenu:
                break;
            case GameState.Loading:
                break;
            case GameState.Lobby:
                break;
            case GameState.InGame:
                break;
            default:
                break;
        }
    }

    public void OnStateExit(GameState exitedState)
    {
        switch (exitedState)
        {
            case GameState.None:
                break;
            case GameState.MainMenu:
                break;
            case GameState.Loading:
                break;
            case GameState.Lobby:
                break;
            case GameState.InGame:
                break;
            default:
                break;
        }
    }
    #endregion
}
