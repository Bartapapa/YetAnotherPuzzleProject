using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    None,
    MainMenu,
    Loading,
    InGame,
}

public class Sc_GameManager : MonoBehaviour
{
    public static Sc_GameManager instance { get; private set; }

    [Header("MANAGER REFS")]
    public Sc_PlayerManager PlayerManagerPrefab;
    public Sc_SoundManager SoundManagerPrefab;
    public SO_SaveData BlankSaveData;

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

    [Header("SAVEDATA")]
    [SerializeField] private SO_SaveData _saveData;

    private bool _managersChecked = false;

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

        SceneManager.sceneLoaded += OnLevelFinishedLoad;
    }

    private void OnLevelFinishedLoad(Scene scene, LoadSceneMode mode)
    {
        int sceneID = scene.buildIndex;
        Debug.Log("scene id: " + sceneID);
        //Can switch from scene ID to scene ID, such as main menu and whatever.

        CheckManagers();
    }

    public void CheckManagers()
    {
        if (_managersChecked)
        {
            Debug.Log("Managers already checked!");
            return;
        }

        if (Sc_PlayerManager.instance == null)
        {
            Sc_PlayerManager playerManager = Instantiate<Sc_PlayerManager>(PlayerManagerPrefab, this.transform.parent);
            playerManager.CreatePlayerIfNone();
        }

        if (Sc_SoundManager.instance == null)
        {
            Sc_SoundManager soundManager = Instantiate<Sc_SoundManager>(SoundManagerPrefab, this.transform.parent);
            soundManager.InitializePresets();
        }
        else
        {
            Sc_SoundManager.instance.InitializePresets();
        }

        switch (_currentGameState)
        {
            case GameState.None:
                break;
            case GameState.MainMenu:
                break;
            case GameState.Loading:
                break;
            case GameState.InGame:
                if (Sc_LevelManager.instance != null)
                {
                    Sc_PlayerManager.instance.AssignEventsToLevelManager();
                }
                else
                {
                    Debug.LogWarning("This level doesn't have a LevelManager!");
                    return;
                }
                if (Sc_CameraManager.instance == null)
                {
                    Debug.LogWarning("This level doesn't have a CameraManager!");
                    return;
                }
                if (Sc_UIManager.instance == null)
                {
                    Debug.LogWarning("This level doesn't have a UIManager!");
                }
                else
                {
                    Sc_UIManager.instance.Transitioner.Reveal();
                }
                Sc_LevelManager.instance.SpawnAllPlayerCharacters();
                LoadData();

                break;
            default:
                break;
        }

        _managersChecked = true;
    }

    public void Load(Loader.Scene scene)
    {
        _managersChecked = false;

        if (Sc_UIManager.instance != null)
        {
            Sc_UIManager.instance.Transitioner.Mask(
                () => Loader.Load(scene));
        }
        else
        {
            return;
        }
    }

    #region GAMESTATE
    public void TransitionToGameState(GameState toState)
    {
        if (_currentGameState == toState)
        {
            return;
        }

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
            case GameState.InGame:
                break;
            default:
                break;
        }
    }
    #endregion

    #region SAVEDATA
    public void SaveData()
    {
        if (_saveData == null)
        {
            SO_SaveData newSaveData = Instantiate<SO_SaveData>(BlankSaveData, this.transform);
            newSaveData.CreateSaveProfiles(Sc_PlayerManager.instance.CurrentPlayers);
            _saveData = newSaveData;
        }
    }

    public void CheckSaveDataDEBUG()
    {
        if (_saveData == null)
        {
            Debug.LogWarning("No save data found!");
            return;
        }
        else
        {
            foreach(PlayerSaveProfile saveProfile in _saveData.SaveProfiles)
            {
                Debug.LogWarning("Here is the data for " + saveProfile.Player + ".");
                Debug.Log("SkinIndex: " + saveProfile.SkinIndex + ".");
                Debug.Log("Currently held item ID is: " + saveProfile.CurrentHeldItemID + ".");
            }
            Debug.LogWarning("Data verified.");
        }
    }

    public void LoadData()
    {
        if (_saveData == null)
        {
            Debug.LogWarning("No save data found!");
            return;
        }
        else
        {
            for (int i = 0; i < Sc_PlayerManager.instance.CurrentPlayers.Count; i++)
            {
                Sc_Player player = Sc_PlayerManager.instance.GetPlayerFromPInput(Sc_PlayerManager.instance.CurrentPlayers[i]);
                if (player.PlayerCharacter != null)
                {
                    PlayerSaveProfile correspondingSaveProfile = _saveData.SaveProfiles[i];
                    player.PlayerCharacter.ChooseSkin(correspondingSaveProfile.SkinIndex);
                }
            }
        }
    }

    public void ClearSaveData()
    {
        if (_saveData != null)
        {
            Destroy(_saveData.gameObject);
            _saveData = null;
        }
    }

    #endregion
}
