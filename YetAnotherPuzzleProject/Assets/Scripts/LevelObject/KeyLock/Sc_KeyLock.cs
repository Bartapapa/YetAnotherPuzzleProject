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
    public Sc_Interactible Interactible;
    public ParticleSystem Particles;
    public CinemachineImpulseSource Impulse;
    public AudioSource Source;

    [Header("OPEN")]
    public AudioClip Open;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnInteractedWith(Sc_Character interactor)
    {
        OpenLock();
    }

    public void OpenLock()
    {
        Interactible.CanBeInteractedWith = false;
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
}
