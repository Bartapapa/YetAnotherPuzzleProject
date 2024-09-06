using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Level : MonoBehaviour
{
    [Header("LEVEL PARAMETERS")]
    public Vector4 _minMaxXZLevelDimensions = new Vector4(-10, 10, -10, 10);
    public float _ceilingHeight = 15f;
    public List<Transform> _spawnPoints = new List<Transform>();

    [Header("LEVEL OBJECT REFERENCES")]
    public ParticleSystem _fallingDust;

    private Coroutine _rotateWorldCO;
    private Coroutine _spawnFallingDustCO;

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

    #region UNIVERSALLEVELFUNCTIONS
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
                    //SPAWN DUST
                    Debug.Log("SPAWN DUST!");
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
