using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Sc_Blink : MonoBehaviour
{
    public Transform _blinkers;
    public Vector2 _minMaxBlinkDelay = new Vector2(.5f, 5f);

    private float _blinkTime = .05f;
    private float _cachedBlinkDelay = 0f;
    private float _blinkDelayTimer = 0f;

    private void Start()
    {
        _cachedBlinkDelay = GetNewBlinkDelay();
    }
    private void Update()
    {
        if (_blinkDelayTimer < _cachedBlinkDelay)
        {
            _blinkDelayTimer += Time.deltaTime;
        }
        else
        {
            StartCoroutine(Blink());
            _blinkDelayTimer = 0f;
            _cachedBlinkDelay = GetNewBlinkDelay();
        }
    }
    private IEnumerator Blink()
    {
        float time = 0f;
        while (time < _blinkTime)
        {
            float toScale = Mathf.Lerp(1f, .05f, time / _blinkTime);
            _blinkers.localScale = new Vector3(1f, 1f, toScale);
            time += Time.deltaTime;
            yield return null;
        }
        time = 0f;
        while (time < _blinkTime)
        {
            float toScale = Mathf.Lerp(.05f, 1f, time / _blinkTime);
            _blinkers.localScale = new Vector3(1f, 1f, toScale);
            time += Time.deltaTime;
            yield return null;
        }
        _blinkers.localScale = new Vector3(1f, 1f, 1f);
    }

    private float GetNewBlinkDelay()
    {
        return Random.Range(_minMaxBlinkDelay.x, _minMaxBlinkDelay.y);
    }
}
