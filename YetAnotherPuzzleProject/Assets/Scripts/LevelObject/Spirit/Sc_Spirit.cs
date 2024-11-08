using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Spirit : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Trumpet_Listener TrumpetListener;
    [ReadOnly] public Sc_SpiritGuide CurrentGuide;

    [Header("FOLLOW")]
    public float FollowSpeed = 2f;
    private float _cachedFollowDistance = 0f;
    private float _cachedUpwardsAngle = 0f;
    private float _cachedOrbitSpeed = 0f;
    private Vector3 _guideOffset = Vector3.zero;

    [Header("INVISIBILITY")]
    public Transform Mesh;
    public Vector2 MinMaxInvisiblityDistance = new Vector2(1.5f, 4f);
    public Vector2 MinMaxInvisibilityAlpha = new Vector2(0f, .5f);
    private Renderer[] rend;
    private float _desiredAlpha = 0f;
    private float _currentAlpha = 0f;
    private float _alphaInterpolationSpeed = .5f;

    [Header("REVEALERS")]
    [ReadOnly] public List<Sc_SpiritRevealer> Revealers = new List<Sc_SpiritRevealer>();

    private void Start()
    {
        rend = GetComponentsInChildren<Renderer>();

        TrumpetListener.HearCharacterPlayTrumpet -= OnHearCharacterPlayTrumpet;
        TrumpetListener.HearCharacterPlayTrumpet += OnHearCharacterPlayTrumpet;
    }

    private void Update()
    {
        HandleInvisibility();
        HandleFollowGuide();
    }

    private void HandleFollowGuide()
    {
        if (CurrentGuide == null) return;
        _guideOffset = Quaternion.Euler(0f, _cachedOrbitSpeed * Time.deltaTime, 0f) * _guideOffset;
        transform.position = Vector3.MoveTowards(transform.position, CurrentGuide.transform.position + _guideOffset, FollowSpeed * Time.deltaTime);
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

    private void OnHearCharacterPlayTrumpet(Sc_Character character)
    {
        if (character.CanGuideSpirits)
        {
            FollowGuide(character.SpiritGuide);
        }
    }

    private void CacheFollowValues(Sc_SpiritGuide guide)
    {
        _cachedFollowDistance = UnityEngine.Random.Range(guide.MinMaxFollowDistance.x, guide.MinMaxFollowDistance.y);
        _cachedUpwardsAngle = UnityEngine.Random.Range(guide.MinMaxUpwardsAngle.x, guide.MinMaxUpwardsAngle.y);
        _cachedOrbitSpeed = UnityEngine.Random.Range(guide.MinMaxOrbitSpeed.x, guide.MinMaxOrbitSpeed.y);

        Vector3 guideForward = guide.transform.forward;
        float randomStartAngle = UnityEngine.Random.Range(0, 359f);
        guideForward = Quaternion.Euler(_cachedUpwardsAngle, randomStartAngle, 0f) * guideForward;
        _guideOffset = guideForward * _cachedFollowDistance;
    }

    public void FollowGuide(Sc_SpiritGuide guide)
    {
        StopFollowCurrentGuide();
        CurrentGuide = guide;
        CurrentGuide.AddSpirit(this);
        CacheFollowValues(guide);
    }

    public void StopFollowCurrentGuide()
    {
        if (CurrentGuide == null) return;
        CurrentGuide.RemoveSpirit(this);
        CurrentGuide = null;
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
