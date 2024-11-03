using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_ExitDoor : Sc_Activateable
{
    [Header("GATE")]
    public Renderer GateWayMesh;
    public Material ClosedMat;
    public Material OpenMat;

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

    #region Activateable implementation

    public override bool Activate(bool toggleOn)
    {
        if (base.Activate(toggleOn))
        {
            OpenDoor(toggleOn);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void ForceActivate(bool toggleOn)
    {
        base.ForceActivate(toggleOn);
        ForceOpenDoor(toggleOn);
    }

    #endregion

    public void OpenDoor(bool open)
    {
        _trigger.enabled = open;
        Material toMat = open ? OpenMat : ClosedMat;
        GateWayMesh.material = toMat;
        Sc_GameManager.instance.SoundManager.PlaySFX(_source, _doorSound);
    }

    public void ForceOpenDoor(bool open)
    {
        _trigger.enabled = open;
        Material toMat = open ? OpenMat : ClosedMat;
        GateWayMesh.material = toMat;
        //Gate.ForceActivate(open);
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
