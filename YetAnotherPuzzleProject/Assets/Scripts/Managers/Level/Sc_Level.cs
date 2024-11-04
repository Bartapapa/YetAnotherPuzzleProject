using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sc_Level : MonoBehaviour
{
    public static Sc_Level instance { get; private set; }

    [Header("CURRENT SCENE")]
    public Loader.Scene CurrentScene = Loader.Scene.SampleScene1;
    public bool CanBeReloaded = true;
    public bool IsLobby = false;
    public bool IsDebugLevel = false;

    [Header("TREASURE VASES")]
    public List<Sc_Vase> TreasureVases = new List<Sc_Vase>();

    [Header("NAVMESH")]
    public NavMeshSurface NMSurface;

    [Header("LEVEL PARAMETERS")]
    public Vector4 _minMaxXZLevelDimensions = new Vector4(-10, 10, -10, 10);
    public float _ceilingHeight = 15f;
    public List<Transform> _spawnPoints = new List<Transform>();

    [Header("LEVEL OBJECT REFERENCES")]
    public ParticleSystem _fallingDust;

    private Coroutine _rotateWorldCO;
    private Coroutine _spawnFallingDustCO;

    private int _spawnedPlayers = 0;

    private bool _buildNavmeshRequested = true;

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

    private void Start()
    {
        if (Sc_GameManager.instance != null)
        {
            Sc_GameManager.instance.CurrentLevel = this;
            Sc_UIManager.instance.Transitioner.Reveal();

            Sc_GameManager.instance.PlayerManager.AssignEventsToLevelManager();


            //Load current level data
            Sc_GameManager.instance.LoadCurrentLevelData();
            //Save current level data
            Sc_GameManager.instance.SaveLevelData(this);

            SpawnAllPlayerCharacters();
            //Load all player character data
            if (!IsLobby)
            {
                Sc_GameManager.instance.LoadPlayerCharacterData();
            }           
            //Save all player character data
            Sc_GameManager.instance.SavePlayerCharacterData();
        }
    }

    private void Update()
    {
        if (_buildNavmeshRequested) BuildNavmesh();
    }
    #region LEVELDATA
    public void LoadLevelData(LevelSaveProfile saveProfile)
    {
        for (int i = 0; i < saveProfile.VasesChecked.Count; i++)
        {
            TreasureVases[i]._interactible.CanBeInteractedWith = !saveProfile.VasesChecked[i];
        }
    }
    #endregion

    #region PLAYERSPAWNING
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Sc_Player player = Sc_GameManager.instance.PlayerManager.GetPlayerFromPInput(playerInput);
       
        if (player != null)
        {
            //If we're in-game, otherwise skip this.
            Transform spawnPoint = GetSpawnPoint(_spawnedPlayers);

            SpawnPlayerCharacter(player, spawnPoint);
        }
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Sc_Player player = Sc_GameManager.instance.PlayerManager.GetPlayerFromPInput(playerInput);
        if (player != null)
        {
            if (player.PlayerCharacter != null)
            {
                Sc_CameraManager.instance.RemoveFocus(Sc_CameraManager.instance._defaultCameraFocus, player.PlayerCharacter.transform);
            }
            _spawnedPlayers--;
        }
    }

    public void SpawnAllPlayerCharacters()
    {
        ResetSpawnedPlayerCount();

        foreach (PlayerInput playerInput in Sc_GameManager.instance.PlayerManager.CurrentPlayers)
        {
            Sc_Player player = Sc_GameManager.instance.PlayerManager.GetPlayerFromPInput(playerInput);
            Transform spawnPoint = GetSpawnPoint(_spawnedPlayers);
            SpawnPlayerCharacter(player, spawnPoint);
        }
    }

    public void SpawnPlayerCharacter(Sc_Player player, Transform spawnPoint)
    {
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

    public void ResetSpawnedPlayerCount()
    {
        _spawnedPlayers = 0;
    }

    public Transform GetSpawnPoint(int spawnedPlayerIndex)
    {
        Transform spawnPoint = null;

        if (_spawnPoints.Count > 0)
        {
            spawnPoint = _spawnPoints[0];
        }

        if (spawnedPlayerIndex <= _spawnPoints.Count)
        {
            spawnPoint = _spawnPoints[spawnedPlayerIndex];
        }

        return spawnPoint;
    }
    #endregion

    #region UNIVERSALLEVELFUNCTIONS

    public void RequestRebuildNavmesh()
    {
        if (NMSurface == null) return;
        _buildNavmeshRequested = true;
    }

    private void BuildNavmesh()
    {
        if (NMSurface != null)
        {
            NMSurface.BuildNavMesh();
        }

        _buildNavmeshRequested = false; 
    }
    public void RotateWorldSequence(float degrees, float overTime)
    {
        if (_rotateWorldCO != null)
        {
            StopCoroutine(_rotateWorldCO);
        }

        _rotateWorldCO = StartCoroutine(RotateWorldCo(degrees, overTime));
    }

    private IEnumerator RotateWorldCo(float degrees, float overTime)
    {
        Sc_CameraManager manager = null;
        if (Sc_CameraManager.instance != null)
        {
            manager = Sc_CameraManager.instance;
        }
        else
        {
            StopCoroutine(_rotateWorldCO);
        }

        float blendTime = overTime * .25f;
        float rotateCameraTime = overTime - (blendTime * 2f);
        manager.ShakeDefaultCamera(1f, overTime, blendTime);

        if (_spawnFallingDustCO != null)
        {
            StopCoroutine(_spawnFallingDustCO);
        }
        _spawnFallingDustCO = StartCoroutine(SpawnFallingDustCo(new Vector2Int(4, 8), rotateCameraTime+blendTime));

        float time = 0f;
        while (time < blendTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0f;
        manager.RotateDefaultCamera(degrees, rotateCameraTime);
        while (time < rotateCameraTime+blendTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        _rotateWorldCO = null;
    }

    private IEnumerator SpawnFallingDustCo(Vector2Int minMaxDustToSpawn, float overTime)
    {
        Sc_CameraFocus focus = null;
        if (Sc_CameraManager.instance != null)
        {
            focus = Sc_CameraManager.instance._defaultCameraFocus;
        }
        else
        {
            StopCoroutine(_spawnFallingDustCO);
        }
        if (focus == null)
        {
            StopCoroutine(_spawnFallingDustCO);
        }

        int numberOfDustToSpawn = UnityEngine.Random.Range(minMaxDustToSpawn.x, minMaxDustToSpawn.y+1);
        float timeMarker = overTime / numberOfDustToSpawn;

        float[] timeStamps = new float[numberOfDustToSpawn];

        for (int i = 0; i < timeStamps.Length; i++)
        {
            timeStamps[i] = (timeMarker * i) + (UnityEngine.Random.Range(-timeMarker * .5f, timeMarker * .5f));
        }

        int spawnedDust = 0;
        float time = 0f;
        while (time < overTime)
        {
            if (spawnedDust < timeStamps.Length)
            {
                if (time >= timeStamps[spawnedDust])
                {
                    float randomX = UnityEngine.Random.Range(_minMaxXZLevelDimensions.x, _minMaxXZLevelDimensions.y);
                    float randomZ = UnityEngine.Random.Range(_minMaxXZLevelDimensions.z, _minMaxXZLevelDimensions.w);
                    Vector3 randomPosition = new Vector3(randomX, _ceilingHeight, randomZ);
                    ParticleSystem newDust = Instantiate<ParticleSystem>(_fallingDust, randomPosition, Quaternion.identity, Sc_CameraManager.instance._defaultCameraFocus.transform);
                    spawnedDust++;
                }
            }
            time += Time.deltaTime;
            yield return null;
        }
        _spawnFallingDustCO = null;
    }
    #endregion
}
