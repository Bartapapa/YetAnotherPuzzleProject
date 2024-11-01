using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_ExitDoor : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Activateable _activateable;
    public bool IsAlreadyActivated = false;

    [Header("TO LEVEL")]
    public Loader.Scene _toLevel = Loader.Scene.SampleScene1;

    [Header("SOUND REFS")]
    public AudioClip _doorSound;
    public AudioSource _source;

    private BoxCollider _trigger;

    private void Awake()
    {
        _trigger = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        _trigger.isTrigger = true;
        if (_activateable)
        {
            _trigger.enabled = _activateable.StartActivated;
        }
        else
        {
            _trigger.enabled = IsAlreadyActivated;
        }
        
    }

    public void OnActivate(bool activate)
    {
        _trigger.enabled = activate;
        Sc_GameManager.instance.SoundManager.PlaySFX(_source, _doorSound);
    }

    public void LoadLevel()
    {
        Sc_GameManager.instance.SavePlayerCharacterData();
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
