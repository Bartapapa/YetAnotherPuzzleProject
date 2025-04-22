using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        if (Camera.main == null) return;
        transform.forward = -Camera.main.transform.forward;
    }
}
