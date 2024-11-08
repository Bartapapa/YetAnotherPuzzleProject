using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Character : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    public Sc_CharacterController Controller;
    public Sc_SpiritGuide SpiritGuide;
    public bool CanGuideSpirits { get { return SpiritGuide != null; } }

    [Header("CHARACTER PARAMETERS")]
    public bool CanBeHurt = true;
    public ParticleSystem HurtFX;
    public Transform HeadPoint;
    public float HurtDuration = 2f;
    public bool IsHurting { get { return _hurtCo != null; } }
    private Coroutine _hurtCo = null;

    private IEnumerator HurtCoroutine()
    {
        Controller.CanRotate = false;
        Controller.CanMove = false;
        float timer = 0f;
        while (timer < HurtDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        StopHurting();
    }

    public virtual void Hurt(Sc_Character attacker, Vector3 directionOfAttack, Vector3 pointOfContact)
    {
        if (HurtFX)
        {
            ParticleSystem hurtFX = Instantiate<ParticleSystem>(HurtFX, HeadPoint.position, Quaternion.identity);
        }
        _hurtCo = StartCoroutine(HurtCoroutine());
    }

    public void StopHurting()
    {
        if (_hurtCo != null)
        {
            StopCoroutine(_hurtCo);
            Controller.CanMove = true;
            Controller.CanRotate = true;
        }
    }
}
