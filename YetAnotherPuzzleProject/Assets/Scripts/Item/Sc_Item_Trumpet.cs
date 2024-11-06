using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrumpetSoundType
{
    Low,
    MidLow,
    MidHigh,
    High
}

public class Sc_Item_Trumpet : Sc_Item
{
    [Header("Trumpet object refs")]
    public ParticleSystem Trumpet;

    [Header("Trumpet parameters")]
    public float TrumpetRange = 3f;
    public LayerMask TrumpetLayers;

    [Header("Trumpet sounds")]
    public AudioClip TrumpetLow;
    public AudioClip TrumpetMidLow;
    public AudioClip TrumpetMidHigh;
    public AudioClip TrumpetHigh;

    public override void UseItem()
    {
        if (_inInventory.IsUsingItem)
        {
            StopUsingTrumpet();
        }
        else
        {
            StartUsingTrumpet();
        }
    }

    public override void UseItemSpecial(int index)
    {
        if (index == 0)
        {
            Sc_GameManager.instance.SoundManager.PlaySFX(Source, TrumpetHigh);
            CheckForTrumpetListeners(TrumpetSoundType.High);
        }
        else if (index == 1)
        {
            Sc_GameManager.instance.SoundManager.PlaySFX(Source, TrumpetMidHigh);
            CheckForTrumpetListeners(TrumpetSoundType.MidHigh);
        }
        else if (index == 2)
        {
            Sc_GameManager.instance.SoundManager.PlaySFX(Source, TrumpetMidLow);
            CheckForTrumpetListeners(TrumpetSoundType.MidLow);
        }
        else if (index == 3)
        {
            Sc_GameManager.instance.SoundManager.PlaySFX(Source, TrumpetLow);
            CheckForTrumpetListeners(TrumpetSoundType.Low);
        }
        Trumpet.Play();
    }

    public override void StopUsingItem()
    {
        base.StopUsingItem();
        StopUsingTrumpet();
    }

    private void StartUsingTrumpet()
    {
        _inInventory.Character.Controller.CanMove = false;
        _inInventory.IsUsingItem = true;
        transform.localPosition = new Vector3(0f, -.85f, .85f);

        //Place trumpet forward character.
    }
    private void StopUsingTrumpet()
    {
        _inInventory.Character.Controller.CanMove = true;
        _inInventory.IsUsingItem = false;
        transform.localPosition = Vector3.zero;
    }

    private void CheckForTrumpetListeners(TrumpetSoundType trumpetSound)
    {
        Collider[] coll = Physics.OverlapSphere(transform.position, TrumpetRange);
        List<Sc_Trumpet_Listener> listeners = new List<Sc_Trumpet_Listener>();
        foreach(Collider collider in coll)
        {
            Sc_Trumpet_Listener listener = collider.GetComponent<Sc_Trumpet_Listener>();
            if (listener)
            {
                if (!listeners.Contains(listener))
                {
                    listeners.Add(listener);
                }
            }
        }

        foreach(Sc_Trumpet_Listener listener in listeners)
        {
            listener.OnHearTrumpet(trumpetSound);
        }
    }
}
