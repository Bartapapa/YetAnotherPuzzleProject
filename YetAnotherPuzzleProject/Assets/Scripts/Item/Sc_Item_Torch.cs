using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item_Torch : Sc_Item
{
    [Header("TORCH OBJECT REFERENCES")]
    public Sc_LightSource LightSource;
    public Light LightObj;

    public override void UseItem()
    {
        if (!IsEquipped) return;
        ToggleLight();
    }

    private void ToggleLight()
    {
        LightSource.enabled = !LightSource.enabled;
        LightObj.enabled = LightSource.enabled;
    }
}
