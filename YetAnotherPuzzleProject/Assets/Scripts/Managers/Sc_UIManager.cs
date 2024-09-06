using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_UIManager : MonoBehaviour
{
    public static Sc_UIManager instance { get; private set; }

    [Header("OBJECT REFS")]
    public Sc_Transition Transitioner;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

}
