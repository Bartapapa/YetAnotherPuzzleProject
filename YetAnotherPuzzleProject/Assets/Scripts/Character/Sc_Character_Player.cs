using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Character_Player : Sc_Character
{
    [Header("PLAYER CHARACTER OBJECT REFS")]
    public Sc_Interactor Interactor;
    public Sc_Inventory Inventory;
    public Sc_SoundHandler_PlayerCharacter SoundHandler;
    public Sc_RestartCircle RestartCircle;

    [Header("PLAYER CHARACTER KEYBOARD AIMING HANDLING")]
    public float AimingRotationSpeed = 2f;
    [ReadOnly] public float BaseRotationSpeed = 0f;

    [Header("PLAYER CHARACTER SKINS")]
    public List<GameObject> Skins = new List<GameObject>();
    [SerializeField][ReadOnly] private int _skinIndex = -1;
    public int SkinIndex { get { return _skinIndex; } }

    private void Start()
    {
        BaseRotationSpeed = Controller._rotationSharpness;

        ChooseRandomSkin();
        Skins.Clear();
    }

    public void ChooseRandomSkin()
    {
        int randomSkin = Random.Range(0, Skins.Count);
        ChooseSkin(randomSkin);
    }

    public void ChooseSkin(int skinIndex)
    {
        for (int i = 0; i < Skins.Count; i++)
        {
            if (i == skinIndex)
            {
                Skins[i].SetActive(true);
                _skinIndex = i;
            }
            else
            {
                Skins[i].SetActive(false);
            }
        }
    }
}
