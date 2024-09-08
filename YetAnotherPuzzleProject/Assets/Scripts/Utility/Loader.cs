using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    private class LoadingMonoBehaviour : MonoBehaviour { }

    public enum Scene
    {
        Loading,
        Managers,
        UIScene,
        PlayerCharacterScene,
        SampleScene1,
        SampleScene2,
        Level_0_1,
        Level_0_2,
        Level_0_3,
        Level_0_4,
        Level_1_1,
        //Fill this with the Scene names
    }

    private static Action OnLoaderCallBack;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(Scene scene, bool additive = false)
    {
        OnLoaderCallBack = () => {
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene, additive));
            LoadSceneAsync(scene, additive);
        };

        SceneManager.LoadScene(Scene.Loading.ToString(), LoadSceneMode.Additive);
        //Sc_GameManager.instance.currentScene = scene;
    }

    public static void Unload(Scene scene)
    {
        SceneManager.UnloadSceneAsync(scene.ToString());
    }

    private static IEnumerator LoadSceneAsync(Scene scene, bool additive)
    {
        yield return null;

        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString(), additive ? LoadSceneMode.Additive : LoadSceneMode.Single);

        while (!loadingAsyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }

    public static void LoaderCallBack()
    {
        if (OnLoaderCallBack != null)
        {
            OnLoaderCallBack();
            OnLoaderCallBack = null;
        }
    }
}