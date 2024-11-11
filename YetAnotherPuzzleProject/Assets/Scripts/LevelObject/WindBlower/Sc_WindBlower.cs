using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_WindBlower : Sc_Activateable
{
    [Header("WIND TURBINE PIVOT")]
    public Transform Pivot;
    public float SpinSpeed = 90f;

    [Header("WIND TURBINE PARAMETERS")]
    public float BlowDistance = 6f;
    public float StrongBlowDistance = 2f;
    public float BlowForce = 5f;
    public float StrongBlowForce = 20f;
    public LayerMask CharacterLayers;
    public LayerMask WindObstacleLayers;

    [Header("PARTICLES")]
    public ParticleSystem WindParticles;

    private BoxCollider _collider;
    private bool _blowingWind = false;
    private int _rayCastIterations = 10;
    private RaycastHit _characterHit;
    private RaycastHit _obstacleHit;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (_blowingWind)
        {
            Pivot.localEulerAngles += new Vector3(0f, 0f, SpinSpeed*Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (_blowingWind)
        {
            HandleCharacterPushing();
        }       
    }
    #region Activateable implementation
    public override bool Activate(bool toggleOn)
    {
        if (base.Activate(toggleOn))
        {
            BlowWind(toggleOn);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void ForceActivate(bool toggleOn)
    {
        base.ForceActivate(toggleOn);
        ForceBlowWind(toggleOn);
    }

    public override void OnLockEngaged(bool engage)
    {
        
    }

    public override void OnLockDestroyed()
    {
       
    }
    #endregion

    public void BlowWind(bool blow)
    {
        _blowingWind = blow;
        if (blow)
        {
            WindParticles.Play();
        }
        else
        {
            WindParticles.Stop();
        }
    }

    public void ForceBlowWind(bool blow)
    {
        BlowWind(blow);
    }

    private void HandleCharacterPushing()
    {
        List<Sc_CharacterController> pushedCharactersGentle = new List<Sc_CharacterController>();
        List<Sc_CharacterController> pushedCharactersStrong = new List<Sc_CharacterController>();
        Vector3 initialCheckOrigin = transform.position + Vector3.up - transform.right + (transform.right*.2f) + transform.forward;
        for (int i = 0; i <= _rayCastIterations; i++)
        {
            if (Physics.Raycast(initialCheckOrigin+(transform.right*((i/(float)_rayCastIterations)*1.6f)), transform.forward, out _characterHit, BlowDistance, CharacterLayers, QueryTriggerInteraction.Ignore))
            {
                //Found character in range
                if(!Physics.Raycast(initialCheckOrigin + (transform.right * ((i / (float)_rayCastIterations) * 1.6f)), transform.forward, BlowDistance, WindObstacleLayers, QueryTriggerInteraction.Ignore))
                {
                    //Character is unobstructed by obstacle
                    Sc_CharacterController character = _characterHit.collider.GetComponent<Sc_CharacterController>();
                    if (character)
                    {
                        float characterDistance = _characterHit.distance;
                        if (characterDistance <= StrongBlowDistance)
                        {
                            if (!pushedCharactersStrong.Contains(character))
                            {
                                pushedCharactersStrong.Add(character);
                                pushedCharactersGentle.Remove(character);
                            }
                        }
                        else
                        {
                            if (!pushedCharactersGentle.Contains(character) && !pushedCharactersStrong.Contains(character))
                            {
                                pushedCharactersGentle.Add(character);
                            }
                        }

                    }
                }
            }
        }

        foreach(Sc_CharacterController character in pushedCharactersStrong)
        {
            character.InheritedVelocity += transform.forward * StrongBlowForce * Time.fixedDeltaTime;
        }
        foreach(Sc_CharacterController character in pushedCharactersGentle)
        {
            character.InheritedVelocity += transform.forward * BlowForce * Time.fixedDeltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 initialRayOrigin = transform.position + Vector3.up - transform.right + (transform.right * .2f) + transform.forward;
        for (int i = 0; i <= _rayCastIterations; i++)
        {
            Gizmos.DrawLine(initialRayOrigin + (transform.right * ((i / (float)_rayCastIterations) * 1.6f)), initialRayOrigin + (transform.right * ((i / (float)_rayCastIterations) * 1.6f) + transform.forward * BlowDistance));
        }
    }
}
