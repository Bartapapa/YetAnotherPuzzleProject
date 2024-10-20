using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Lifespan : MonoBehaviour
{
    public float Duration = 0f;
    private float _currentTime = float.MinValue;
    public float TimeLeft { get { return Duration - _currentTime; } }
    public float CurrentTime { get { return _currentTime; } }

    private bool _lifeSpanEnded = false;

    public delegate void DefaultEvent();
    public event DefaultEvent OnLifespanEnd;

    private void Start()
    {
        if (Duration > 0)
        {
            SetLifespan(Duration);
        }
    }

    private void Update()
    {
        if (_currentTime < Duration && !_lifeSpanEnded)
        {
            _currentTime += Time.deltaTime;
        }
        else
        {
            _lifeSpanEnded = true;
            OnLifespanEnd?.Invoke();
        }
    }

    public void SetLifespan(float duration)
    {
        Duration = duration;
        _currentTime = 0f;
        _lifeSpanEnded = false;
    }
}
