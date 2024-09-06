using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sc_Transition : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private Image _mask;

    [Header("Parameters")]
    [SerializeField] private float _maskTime;

    private Coroutine _transitionCO = null;
    private Action _onTransitionEnd;

    private void Start()
    {
        //_mask.fillAmount = 0f;
    }

    public void Mask(Action onTransitionEnd = null, Action onTransitionStart = null)
    {
        //Use when a level is finished, so that the LevelManager can start loading the next level assets seamlessly.
        _transitionCO = StartCoroutine(Transition(true, onTransitionEnd, onTransitionStart));
    }

    public void Reveal(Action onTransitionEnd = null, Action onTransitionStart = null)
    {
        //Use when a level is done loading the assets.
        _transitionCO = StartCoroutine(Transition(false, onTransitionEnd, onTransitionStart));
    }

    IEnumerator Transition(bool isMasking, Action onTransitionEnd = null, Action onTransitionStart = null)
    {
        _mask.fillClockwise = isMasking;
        float time = 0f;
        float duration = _maskTime;
        float startPoint = _mask.fillAmount;
        _onTransitionEnd = onTransitionEnd;

        if (onTransitionStart != null)
        {
            onTransitionStart();
        }

        while (time < duration)
        {
            time += Time.deltaTime;

            if (isMasking)
            {
                _mask.fillAmount = Mathf.Lerp(0, 1f, time / duration);
            }
            else
            {
                _mask.fillAmount = 1 - Mathf.Lerp(0, 1f, time / duration);

            }

            yield return null;
        }

        if (_onTransitionEnd != null)
        {
            _onTransitionEnd();
            _onTransitionEnd = null;
        }
        _transitionCO = null;
    }
}
