using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_LevelLoader : MonoBehaviour
{
    [Header("LEVEL TO LOAD")]
    public Loader.Scene LevelToLoad = Loader.Scene.SampleScene1;

    public void LoadLevel()
    {
        Sc_GameManager.instance.Load(LevelToLoad);
    }
}
