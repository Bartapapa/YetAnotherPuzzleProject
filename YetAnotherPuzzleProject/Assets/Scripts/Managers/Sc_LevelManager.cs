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
}
