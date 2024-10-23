using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Character : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    public Sc_CharacterController Controller;

    [Header("CHARACTER PARAMETERS")]
    public bool CanBeHurt = true;
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

    public virtual void Hurt(Sc_Character attacker, Vector3 directionOfAttack)
    {
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
