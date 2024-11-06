using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Spirit : MonoBehaviour
{
    [Header("INVISIBILITY")]
    public Transform Mesh;
    public Vector2 MinMaxInvisiblityDistance = new Vector2(1.5f, 4f);
    public Vector2 MinMaxInvisibilityAlpha = new Vector2(0f, .5f);
    private Renderer[] rend;
    private float _desiredAlpha = 0f;
    private float _currentAlpha = 0f;
    private float _alphaInterpolationSpeed = .25f;

    [Header("REVEALERS")]
    [ReadOnly] public List<Sc_SpiritRevealer> Revealers = new List<Sc_SpiritRevealer>();

    private void Start()
    {
        rend = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        HandleInvisibility();
    }
    private void HandleInvisibility()
    {
        //_desiredAlpha = Mathf.Abs(Mathf.Sin(Time.time));
        _desiredAlpha = GetDesiredInvisiblityAlpha();
        _currentAlpha = Mathf.MoveTowards(_currentAlpha, _desiredAlpha, _alphaInterpolationSpeed * Time.deltaTime);
        float toAlpha = Mathf.Lerp(MinMaxInvisibilityAlpha.x, MinMaxInvisibilityAlpha.y, _currentAlpha);

        foreach(Renderer r in rend)
        {
            Color newColor = new Color(r.material.color.r, r.material.color.g, r.material.color.b, toAlpha);
            r.material.color = newColor;
        }
    }

    private float GetDesiredInvisiblityAlpha()
    {
        float alpha = 0f;
        //alpha should be 0 if at MaxInvisibility distance or more, and 1 if at MinInvisibility distance or less.
        float closestDistance = float.MaxValue;
        foreach(Sc_SpiritRevealer revealer in Revealers)
        {
            if (revealer.Active)
            {
                float distance = Vector3.Distance(this.transform.position, revealer.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
            }
        }
        alpha = Mathf.InverseLerp(MinMaxInvisiblityDistance.y, MinMaxInvisiblityDistance.x, closestDistance);
        alpha = Mathf.Clamp01(alpha);

        return alpha;
    }

    private void OnRevealerStateChanged(Sc_SpiritRevealer revealer)
    {
        UnRegisterRevealer(revealer);
    }

    public void RegisterRevealer(Sc_SpiritRevealer revealer)
    {
        if (!Revealers.Contains(revealer))
        {
            Revealers.Add(revealer);
            revealer.OnStateChanged -= OnRevealerStateChanged;
            revealer.OnStateChanged += OnRevealerStateChanged;
        }
    }

    public void UnRegisterRevealer(Sc_SpiritRevealer revealer)
    {
        Revealers.Remove(revealer);
        revealer.OnStateChanged -= OnRevealerStateChanged;
    }
}
