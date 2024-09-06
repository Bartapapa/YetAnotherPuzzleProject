using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_LevelLoader : MonoBehaviour
{
    [Header("LEVEL TO LOAD")]
    public Loader.Scene _levelToLoad = Loader.Scene.SampleScene;

    public void LoadLevel()
    {
        Sc_GameManager.instance.Load(_levelToLoad);
    }
}
