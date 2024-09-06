using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_CameraRotator : MonoBehaviour
{
    public float rotateDegrees = 90f;
    public float overTime = 4f;

    public void RotateWorld()
    {
        if (Sc_LevelManager.instance != null)
        {
            Sc_LevelManager.instance._currentLevel.RotateWorldSequence(rotateDegrees, overTime);
        }
    }
}
