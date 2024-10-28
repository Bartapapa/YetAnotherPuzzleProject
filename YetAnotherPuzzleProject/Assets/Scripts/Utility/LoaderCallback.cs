using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool isFirstUpdate = true;

    private void Update()
    {
        if (isFirstUpdate)
        {
            if (Sc_GameManager.instance != null)
            {
                //Sc_GameManager.instance.CurrentGameState = GameState.Loading;
            }
            else
            {
                Debug.LogWarning("NO GAME MODE??? WTF");
            }

            isFirstUpdate = false;
            Loader.LoaderCallBack();
        }
    }
}
