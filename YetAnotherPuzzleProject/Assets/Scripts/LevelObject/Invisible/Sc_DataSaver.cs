using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_DataSaver : MonoBehaviour
{
    public void OnActivate(bool activate)
    {
        if (activate)
        {
            SaveData();
        }
        else
        {
            DebugCheckData();
        }
    }

    public void SaveData()
    {
        Sc_GameManager.instance.SaveData();
    }

    public void DebugCheckData()
    {
        Sc_GameManager.instance.CheckSaveDataDEBUG();
    }
}
