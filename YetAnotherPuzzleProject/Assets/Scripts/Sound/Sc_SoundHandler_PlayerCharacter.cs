using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Sc_SoundHandler_PlayerCharacter : Sc_SoundHandler
{
    [Header("OBJECT REFS")]
    public AudioSource MouthSource;
    public AudioSource FootstepSource;
    public Sc_Character_Player PlayerCharacter;

    [Header("GRUNTS")]
    public List<AudioClip> _grunts = new List<AudioClip>();
    public Vector2 _minMaxGruntPitch = new Vector2(.8f, 1.1f);

    [Header("QUACK")]
    public AudioClip _quack;
    public ParticleSystem _duckParticles;
    public Vector2 _minMaxQuackPitch = new Vector2(1f, 1f);

    [Header("FOOTSTEPS")]
    public float _footstepDelay = .33f;
    public List<AudioClip> _footsteps = new List<AudioClip>();
    public Vector2 _minMaxFootstepPitch = new Vector2(.8f, 1.1f);
    private bool _currentlyWalking = false;

    [Header("LANDING")]
    public float _landingThreshold = 5f;
    public List<AudioClip> _landing = new List<AudioClip>();

    private Coroutine _footstepsCO;

    private void OnEnable()
    {
        PlayerCharacter.Controller.OnGroundedMovement -= OnGroundedMovement;
        PlayerCharacter.Controller.OnAerialMovement -= OnAerialMovement;
        PlayerCharacter.Controller.OnLanded -= OnLanded;

        PlayerCharacter.Controller.OnGroundedMovement += OnGroundedMovement;
        PlayerCharacter.Controller.OnAerialMovement += OnAerialMovement;
        PlayerCharacter.Controller.OnLanded += OnLanded;
    }

    private void OnDisable()
    {
        PlayerCharacter.Controller.OnGroundedMovement -= OnGroundedMovement;
        PlayerCharacter.Controller.OnAerialMovement -= OnAerialMovement;
        PlayerCharacter.Controller.OnLanded -= OnLanded;
    }

    public void Grunt()
    {
        Sc_GameManager.instance.SoundManager.PlayRandomSFX(MouthSource, _grunts, _minMaxGruntPitch);
    }

    public void Quack()
    {
        Sc_GameManager.instance.SoundManager.PlaySFX(MouthSource, _quack, _minMaxQuackPitch);
        _duckParticles.Play();

        GenerateSoundObject(transform.position, 1f, 1f);
    }

    public void StartFootsteps()
    {
        StopFootsteps();
        _footstepsCO = StartCoroutine(Footsteps());
    }

    public void StopFootsteps()
    {
        if (_footstepsCO != null)
        {
            StopCoroutine(_footstepsCO);
        }
    }

    private IEnumerator Footsteps()
    {
        float time = 0f;
        while (true)
        {
            time += Time.deltaTime;
            if (time >= _footstepDelay)
            {
                Sc_GameManager.instance.SoundManager.PlayRandomSFX(FootstepSource, _footsteps, _minMaxFootstepPitch);
                time = 0f;
            }
            yield return null;
        }
    }

    #region LISTENTOEVENTS

    private void OnGroundedMovement(Rigidbody rb)
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (Mathf.Abs(horizontalVelocity.magnitude) >= 2f && PlayerCharacter.Controller.IsGrounded && !PlayerCharacter.Controller.IsPushingBlock)
        {
            if (!_currentlyWalking)
            {
                _currentlyWalking = true;
                StartFootsteps();
            }
        }
        else
        {          
            if (_currentlyWalking)
            {
                _currentlyWalking = false;
                StopFootsteps();
            }
        }
    }

    private void OnAerialMovement(Rigidbody rb)
    {

    }

    private void OnLanded(Rigidbody rb)
    {
        if (rb.velocity.y >= _landingThreshold) return;
        Sc_GameManager.instance.SoundManager.PlayRandomSFX(MouthSource, _landing, new Vector2(.8f, 1.1f));
        Grunt();
    }

    #endregion
}
