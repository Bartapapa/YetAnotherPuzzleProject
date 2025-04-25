using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Sc_Item_Rock : Sc_Item
{
    public override void UseItem()
    {

    }

    public override void OnItemDestroyed()
    {
        if (Sc_GameManager.instance != null)
        {
            //Sc_GameManager.instance.SoundManager.PlaySFX(Source, Break);
            Sc_GameManager.instance.SoundManager.CreateAudioSourceObject(Break, transform.position, .5f);
        }

        StopAllCoroutines();

        ResetItem();
    }

    public override void ResetItem()
    {
        this.gameObject.transform.parent = _thrownByCharacter.Inventory.RockParent;
        this.gameObject.transform.position = _thrownByCharacter.Inventory.ItemHoldAnchor.position;
        this.gameObject.transform.rotation = _thrownByCharacter.Inventory.ItemHoldAnchor.rotation;

        _interactible.CanBeInteractedWith = false;
        _rb.isKinematic = true;
        _rb.useGravity = false;
        _coll.isTrigger = false;
        foreach (Renderer rend in _renderers)
        {
            foreach (Material mat in rend.materials)
            {
                mat.color = Color.white;
            }
        }

        IsBeingThrown = false;
        _thrownByCharacter = null;
        this.gameObject.SetActive(false);
    }
}
