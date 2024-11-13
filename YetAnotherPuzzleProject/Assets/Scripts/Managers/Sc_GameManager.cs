using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

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
    //public Sc_PlayerManager PlayerManagerPrefab;
    //public Sc_SoundManager SoundManagerPrefab;
    public Sc_PlayerManager PlayerManager;
    public Sc_SoundManager SoundManager;
    public Sc_TreasureManager TreasureManager;
    public Sc_Level CurrentLevel;

    [Header("SAVEDATA")]
    public SO_SaveData OverrideData;
    public SO_SaveData CurrentData;

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

    private bool _isLoading = false;

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

        SceneManager.sceneLoaded += OnSceneFinishedLoad;
        SceneManager.sceneUnloaded += OnSceneFinishedUnload;
    }

    private void Start()
    {        
        //Used if spawned in from directly within a level, for testing purposes.
        if (Sc_Level.instance != null)
        {
            CurrentLevel = Sc_Level.instance;
            if (!CurrentLevel.IsLobby)
            {
                PlayerManager.CreatePlayerIfNone();
            }
            PlayerManager.AssignEventsToLevelManager();
            CurrentLevel.SpawnAllPlayerCharacters();
        }

        
        SoundManager.InitializePresets();
    }

    #region GENERAL FUNCTIONS
    private void PurgeAllManagerCoroutines()
    {
        Debug.Log("STOP");
        SoundManager.StopCos();
    }
    #endregion

    #region LOADING
    private void OnSceneFinishedLoad(Scene scene, LoadSceneMode mode)
    {
        int sceneID = scene.buildIndex;
        Debug.Log("loaded scene id: " + sceneID);
        //Can switch from scene ID to scene ID, such as main menu and whatever.

        //CheckManagers();
    }

    private void OnSceneFinishedUnload(Scene scene)
    {
        int sceneID = scene.buildIndex;
        Debug.Log("unloaded scene id: " + sceneID);
        //Can switch from scene ID to scene ID, such as main menu and whatever. Use this to check that a level has been unloaded.
    }

    public void ReloadCurrentLevel()
    {
        if (CurrentLevel == null) return;
        if (!CurrentLevel.CanBeReloaded) return;
        Debug.LogWarning("RELOADING CURRENT LEVEL!");
        Load(CurrentLevel.CurrentScene);
    }

    public void Load(Loader.Scene scene)
    {
        if (_isLoading) return;
        Debug.Log("LOADING");
        OnBeforeLoadNewLevel();

        if (Sc_UIManager.instance != null)
        {
            _isLoading = true;

            Sc_UIManager.instance.Transitioner.Mask(
                () => ClearScene(CurrentLevel.CurrentScene,scene));
        }
        else
        {
            return;
        }
    }

    private void OnBeforeLoadNewLevel()
    {
        Debug.Log("PURGING");
        PurgeAllManagerCoroutines();
    }

    private void TwinUnloadLoad(Loader.Scene unloadScene, Loader.Scene loadScene)
    {
        SceneManager.UnloadSceneAsync(unloadScene.ToString());
        SceneManager.LoadSceneAsync(loadScene.ToString(), LoadSceneMode.Additive);
    }

    private void ClearScene(Loader.Scene unloadScene, Loader.Scene loadScene)
    {
        foreach(Sc_Player player in PlayerManager.GetPlayersFromPInputs())
        {
            player.ResetPlayerCharacter();
        }

        TwinUnloadLoad(unloadScene, loadScene);
        _isLoading = false;
    }

    #endregion

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
    private SO_SaveData CreateNewSaveData()
    {
        SO_SaveData newSaveData = ScriptableObject.CreateInstance<SO_SaveData>();
        if (OverrideData != null)
        {
            newSaveData = OverrideData;
        }
        return newSaveData;
    }

    private void CheckSaveData()
    {
        if (CurrentData == null)
        {
            CurrentData = CreateNewSaveData();
        }
    }

    private void CheckLevelSaveData()
    {
        if (CurrentData.GetLevelSaveProfileForLevel(CurrentLevel) == null)
        {
            Debug.LogWarning("No save data found for " + CurrentLevel.CurrentScene + "! Creating now.");
            SaveAllCurrentLevelData();
        }
    }

    public void SaveAllCurrentLevelData()
    {
        CheckSaveData();
        CurrentData.CreateLevelSaveProfile(CurrentLevel);
        SaveCurrentLevelVases();
        SaveCurrentLevelSpiritBowls();
    }

    public void SaveLevelData(Sc_Level level)
    {
        CheckSaveData();
        CurrentData.CreateLevelSaveProfile(level);
    }

    public void SaveCurrentLevelVases()
    {
        CheckSaveData();
        CheckLevelSaveData();
        CurrentData.GetLevelSaveProfileForLevel(CurrentLevel).SaveVases(CurrentLevel.TreasureVases);
    }

    public void SaveCurrentLevelSpiritBowls()
    {
        CheckSaveData();
        CheckLevelSaveData();
        CurrentData.GetLevelSaveProfileForLevel(CurrentLevel).SaveSeedBowls(CurrentLevel.SpiritSeedBowls);
    }

    public void LoadCurrentLevelData()
    {
        CheckSaveData();
        if (CurrentLevel == null) return;
        CheckLevelSaveData();
        CurrentLevel.LoadLevelData(CurrentData.GetLevelSaveProfileForLevel(CurrentLevel));
    }

    public void SavePlayerCharacterData()
    {
        if (CurrentData == null)
        {
            CurrentData = CreateNewSaveData();        
        }
        CurrentData.CreateCharacterSaveProfiles(PlayerManager.CurrentPlayers);
    }

    public void LoadPlayerCharacterData()
    {
        if (CurrentData == null) return;

        List<Sc_Player> players = PlayerManager.GetPlayersFromPInputs();
        for (int i = 0; i < players.Count; i++)
        {
            int[] savedInventory = CurrentData.CharacterSaveProfiles[i].PlayerCharacterInventoryIDs;
            int savedCurrentHeldItem = CurrentData.CharacterSaveProfiles[i].CurrentHeldItemIndex;
            players[i].PlayerCharacter.Inventory.PopulateInventory(savedInventory, savedCurrentHeldItem);
        }
    }


    //public void SaveData()
    //{
    //    if (_saveData == null)
    //    {
    //        SO_SaveData newSaveData = Instantiate<SO_SaveData>(BlankSaveData, this.transform);
    //        newSaveData.CreateSaveProfiles(Sc_PlayerManager.instance.CurrentPlayers);
    //        _saveData = newSaveData;
    //    }
    //}

    //public void CheckSaveDataDEBUG()
    //{
    //    if (_saveData == null)
    //    {
    //        Debug.LogWarning("No save data found!");
    //        return;
    //    }
    //    else
    //    {
    //        foreach(PlayerSaveProfile saveProfile in _saveData.SaveProfiles)
    //        {
    //            Debug.LogWarning("Here is the data for " + saveProfile.Player + ".");
    //            Debug.Log("SkinIndex: " + saveProfile.SkinIndex + ".");
    //            Debug.Log("Currently held item ID is: " + saveProfile.CurrentHeldItemID + ".");
    //        }
    //        Debug.LogWarning("Data verified.");
    //    }
    //}

    //public void LoadData()
    //{
    //    if (_saveData == null)
    //    {
    //        Debug.LogWarning("No save data found!");
    //        return;
    //    }
    //    else
    //    {
    //        for (int i = 0; i < Sc_PlayerManager.instance.CurrentPlayers.Count; i++)
    //        {
    //            Sc_Player player = Sc_PlayerManager.instance.GetPlayerFromPInput(Sc_PlayerManager.instance.CurrentPlayers[i]);
    //            if (player.PlayerCharacter != null)
    //            {
    //                PlayerSaveProfile correspondingSaveProfile = _saveData.SaveProfiles[i];
    //                player.PlayerCharacter.ChooseSkin(correspondingSaveProfile.SkinIndex);
    //            }
    //        }
    //    }
    //}

    //public void ClearSaveData()
    //{
    //    if (_saveData != null)
    //    {
    //        Destroy(_saveData.gameObject);
    //        _saveData = null;
    //    }
    //}

    #endregion
}
