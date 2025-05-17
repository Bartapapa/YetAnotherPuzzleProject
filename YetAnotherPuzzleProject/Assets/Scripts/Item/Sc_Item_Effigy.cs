using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item_Effigy : Sc_Item_Haulable
{
    [Header("EFFIGY OBJECT REFERENCES")]
    public Sc_WeightedObject WeightedObject;
    public CinemachineImpulseSource ImpulseSource;
    public ParticleSystem Dust;

    public override void UseItem()
    {
        //WeightedObject.StateChange();
    }

    protected override void OnLanded()
    {
        OnEffigyLand();
    }

    public override void OnItemDrop()
    {
        //base.OnItemDrop();
    }

    private void OnEffigyLand()
    {
        _interactible.CanBeInteractedWith = true;
        _grounded = true;
        WeightedObject.StateChange();

        if (Sc_GameManager.instance != null)
        {
            Sc_GameManager.instance.SoundManager.PlaySFX(Source, Drop, new Vector2(.9f, 1f));
        }

        //SpreadContact();
        GroundShake();
    }

    private void GroundShake()
    {
        if (Sc_CameraManager.instance != null)
        {
            Sc_CameraManager.instance.CameraShake(ImpulseSource, .05f);
        }
        if (Dust)
        {
            Dust.Play();
        }
    }
}
