using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_ExitDoor : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Activateable _activateable;

    [Header("TO LEVEL")]
    public Loader.Scene _toLevel = Loader.Scene.SampleScene1;

    private BoxCollider _trigger;

    private void Awake()
    {
        _trigger = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        _trigger.isTrigger = true;
        _trigger.enabled = _activateable._startActivated;
    }

    public void OnActivate(bool activate)
    {
        _trigger.enabled = activate;
    }

    public void LoadLevel()
    {
        Sc_GameManager.instance.Load(_toLevel);
    }

    private void OnTriggerEnter(Collider other)
    {
        Sc_Character_Player player = other.GetComponent<Sc_Character_Player>();
        if (player)
        {
            LoadLevel();
        }
    }
}
