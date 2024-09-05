using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_CameraRotator : MonoBehaviour
{
    public float rotateDegrees = 90f;

    public void RotateCamera()
    {
        if (Sc_CameraManager.instance != null)
        {
            Sc_CameraManager.instance.RotateDefaultCamera(rotateDegrees);
        }
    }
}
