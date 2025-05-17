using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Health : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Character_Player PlayerCharacter;
    public ParticleSystem DeathParticles;

    [Header("STATE")]
    [ReadOnly] public bool Dead = false;

    private Coroutine _deathSequenceCo;

    public void Death(float deathSequenceTime = 3f)
    {
        DeathParticles.Play();

        PlayerCharacter.Inventory.DropCurrentlyHauledItem();

        PlayerCharacter.Controller.IgnoreInputs = true;
        PlayerCharacter.Controller.Capsule.enabled = false;
        Dead = true;

        _deathSequenceCo = StartCoroutine(DeathSequenceCo(deathSequenceTime));
    }

    private IEnumerator DeathSequenceCo(float deathSequenceTime)
    {
        float timer = 0f;
        float duration = deathSequenceTime;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _deathSequenceCo = null;

        //Change this for multiplayer purposes. Need to check if all playable characters are currently dead.
        Sc_GameManager.instance.ReloadCurrentLevel();
    }
}
