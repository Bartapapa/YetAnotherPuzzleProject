using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Parenter : MonoBehaviour
{
    [Header("HEAD PARENT TRANSFORM")]
    public Transform _headParent;

    public void ParentObject(GameObject obj)
    {
        obj.transform.parent = _headParent;
    }

    public void UnParentObject(GameObject obj)
    {
        obj.transform.parent = null;
    }
}
