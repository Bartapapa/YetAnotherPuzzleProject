using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_KeyLock : Sc_Activator
{
    [Header("LOCK OPENED")]
    public UnityEvent OnLockOpened;

    [Header("OBJECT REFS")]
    public Sc_PlayerDetector PlayerDetector;
    public ParticleSystem Particles;
    public CinemachineImpulseSource Impulse;
    public AudioSource Source;

    [Header("OPEN")]
    public AudioClip Open;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        PlayerDetector.CharacterDetected -= OnCharacterDetected;
        PlayerDetector.CharacterUndetected -= OnCharacterUndetected;

        PlayerDetector.CharacterDetected += OnCharacterDetected;
        PlayerDetector.CharacterUndetected += OnCharacterUndetected;
    }

    public void OpenLock()
    {
        PlayerDetector.enabled = false;

        DelayActivation();
        OnLockOpened?.Invoke();

        _rb.isKinematic = false;
        _rb.useGravity = true;
        _rb.AddForce(transform.forward * 5f, ForceMode.Impulse);
        _rb.AddRelativeTorque(transform.right * 1f, ForceMode.Impulse);

        Particles.Play();
        if (Sc_CameraManager.instance != null)
        {
            Sc_CameraManager.instance.CameraShake(Impulse, .05f);
        }
        if (Sc_GameManager.instance != null)
        {
            Sc_GameManager.instance.SoundManager.PlaySFX(Source, Open);
        }
    }

    private void OnCharacterDetected(Sc_Character detectedCharacter)
    {
        Sc_Character_Player player = detectedCharacter.GetComponent<Sc_Character_Player>();
        if (player)
        {
            if (player.Inventory.CurrentItem == EquippedItem.Hauled)
            {
                if (player.Inventory.CurrentHaulableItem != null)
                {
                    if (player.Inventory.CurrentHaulableItem._itemData.ID == 8)
                    {
                        player.Inventory.UseHaulableItem();
                        OpenLock();
                    }
                }
            }
        }
    }

    private void OnCharacterUndetected(Sc_Character undetectedCharacter)
    {

    }
}
