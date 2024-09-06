using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoader : MonoBehaviour
{
    [Header("SCENES TO LOAD")]
    public List<Loader.Scene> Scenes = new List<Loader.Scene>();

    private void Start()
    {
        //if playing from editor, then spawn in the Game Manager.
        if (Sc_GameManager.instance == null)
        {
            LoadAllNecessaryScenes();
        }
    }

    private void LoadAllNecessaryScenes()
    {
        for (int i = 0; i < Scenes.Count; i++)
        {
            SceneManager.LoadScene(Scenes[i].ToString(), LoadSceneMode.Additive);
        }
    }
}
