using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item_Torch : Sc_Item
{
    [Header("TORCH OBJECT REFERENCES")]
    public Sc_LightSource LightSource;
    public Sc_SpiritRevealer Revealer;
    public Light DefaultLight;
    public Light SpiritLight;

    public override void UseItem()
    {
        if (!IsEquipped) return;
        ToggleSpiritReveal();
        DefaultLight.enabled = !DefaultLight.enabled;
    }

    public override void OnItemStore()
    {
        base.OnItemStore();
        Debug.Log(Revealer);
        Revealer.StateChange();
    }

    private void OnDestroy()
    {
        Debug.Log(Revealer);
        Revealer.StateChange();
    }

    private void ToggleLight()
    {
        LightSource.enabled = !LightSource.enabled;
        DefaultLight.enabled = LightSource.enabled;
    }

    private void ToggleSpiritReveal()
    {
        if (Revealer.Active)
        {
            DeactivateSpiritReveal();
        }
        else
        {
            ActivateSpiritReveal();
        }
    }

    private void ActivateSpiritReveal()
    {
        Revealer.Active = true;
        SpiritLight.enabled = true;
    }

    private void DeactivateSpiritReveal()
    {
        Revealer.Active = false;
        SpiritLight.enabled = false;
    }
}
