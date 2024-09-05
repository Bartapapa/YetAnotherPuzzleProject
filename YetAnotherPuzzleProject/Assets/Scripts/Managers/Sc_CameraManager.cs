using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_CameraManager : MonoBehaviour
{
    public static Sc_CameraManager instance { get; private set; }

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
