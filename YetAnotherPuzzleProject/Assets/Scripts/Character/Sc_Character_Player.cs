using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Character_Player : Sc_Character
{
    [Header("PLAYER CHARACTER OBJECT REFS")]
    public Sc_Inventory Inventory;
    public Sc_Quacker Quacker;

    [Header("PLAYER CHARACTER SKINS")]
    public List<GameObject> Skins = new List<GameObject>();

    private void Start()
    {
        ChooseRandomSkin();
    }

    private void ChooseRandomSkin()
    {
        int randomSkin = Random.Range(0, Skins.Count);
        for (int i = 0; i < Skins.Count; i++)
        {
            if (i == randomSkin)
            {
                Skins[i].SetActive(true);
            }
            else
            {
                Skins[i].SetActive(false);
            }
        }
    }
}
