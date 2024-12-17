using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AnimationHandler : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    public Animator Anim;

    Action _animationEndAction = null;
    Action _waitEndAction = null;
    Coroutine _animationPlayCo = null;
    Coroutine _waitCo = null;

    protected void PlayAnimation(string animationName, int layer, Action onAnimationStart = null, Action onAnimationEnd = null)
    {
        StopPlayAnimation();
        Anim.Play(animationName);
        if (onAnimationStart != null)
        {
            onAnimationStart();
        }
        _animationEndAction = onAnimationEnd;
        _animationPlayCo = StartCoroutine(PlayAnimationCoroutine(layer));
    }

    protected void SetLayerWeight(int layer, float weight)
    {
        Anim.SetLayerWeight(layer, weight);
    }

    protected void Wait(float duration, Action onWaitStart = null, Action onWaitEnd = null)
    {
        StopWait();
        if (onWaitStart != null)
        {
            onWaitStart();
        }
        _waitEndAction = onWaitEnd;
        _waitCo = StartCoroutine(WaitCoroutine(duration));
    }

    private IEnumerator WaitCoroutine(float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            yield return null;
        }

        if(_waitEndAction != null)
        {
            _waitEndAction();
            _waitEndAction = null;
        }

        _waitCo = null;
    }

    private IEnumerator PlayAnimationCoroutine(int layer)
    {
        yield return new WaitForEndOfFrame();
        float duration = Anim.GetCurrentAnimatorStateInfo(layer).length;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            yield return null;
        }
        
        if (_animationEndAction != null)
        {
            _animationEndAction();
            _animationEndAction = null;
        }

        _animationPlayCo = null;
    }

    public void StopWait()
    {
        if (_waitCo != null)
        {
            StopCoroutine(_waitCo);
            _waitCo = null;
        }
    }

    public void StopPlayAnimation()
    {
        if (_animationPlayCo != null)
        {
            StopCoroutine(_animationPlayCo);
            _animationPlayCo = null;
        }
    }
}
