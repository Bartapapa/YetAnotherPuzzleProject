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
    public Sc_RoboArm RoboArm;

    [Header("PLAYER CHARACTER STATE")]
    public bool HasRoboArm = false;

    [Header("PLAYER CHARACTER KEYBOARD AIMING HANDLING")]
    public float AimingRotationSpeed = 2f;
    [ReadOnly] public float BaseRotationSpeed = 0f;

    [Header("PLAYER CHARACTER MESH")]
    public Renderer[] BodyRenderers;
    public Renderer[] CoatRenderers;
    public Renderer[] BillRenderers;
    public Renderer[] LegRenderers;
    public GameObject[] NormalMeshes;
    public GameObject[] RoboMeshes;

    private void Start()
    {
        BaseRotationSpeed = Controller._rotationSharpness;

        if (Sc_PlayerManager.instance != null)
        {
            Sc_PlayerManager.instance.ApplyRandomSkin(BodyRenderers, CoatRenderers, BillRenderers, LegRenderers);
        }

        ApplyRoboArm(RoboArm.HasRoboArm);
    }

    public void ApplyRoboArm(bool apply)
    {
        if (apply)
        {
            for (int i = 0; i < NormalMeshes.Length; i++)
            {
                NormalMeshes[i].SetActive(false);
            }
            for (int i = 0; i < RoboMeshes.Length; i++)
            {
                RoboMeshes[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < NormalMeshes.Length; i++)
            {
                NormalMeshes[i].SetActive(true);
            }
            for (int i = 0; i < RoboMeshes.Length; i++)
            {
                RoboMeshes[i].SetActive(false);
            }
        }
    }
}
