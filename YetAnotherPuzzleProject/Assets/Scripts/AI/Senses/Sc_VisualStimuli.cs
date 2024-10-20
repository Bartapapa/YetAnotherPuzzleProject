using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sc_VisualStimuli : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Character_Player Player;
    [ReadOnly][SerializeField] private List<Sc_LightSource> _lightSources = new List<Sc_LightSource>();

    [Header("PARAMETERS")]
    public int Priority = 10;
    public bool Active = true;
    [SerializeField] private LayerMask _obstacleLayers;
    [ReadOnly][SerializeField] private bool _isInLight = false;
    public bool IsInLight { get { return _isInLight; } }

    private void Update()
    {
        _isInLight = GetIsInLight();
    }

    private bool GetIsInLight()
    {
        bool isInLight = false;
        if (_lightSources.Count >= 1)
        {
            foreach (Sc_LightSource lightSource in _lightSources)
            {
                if (lightSource.GlobalLight)
                {
                    isInLight = true;
                    break;
                }

                Vector3 rayDir = (transform.position - lightSource.transform.position).normalized;
                float rayDist = Vector3.Distance(transform.position, lightSource.transform.position);
                if (!Physics.Raycast(lightSource.transform.position, rayDir, rayDist, _obstacleLayers))
                {
                    isInLight = true;
                    break;
                }
            }
        }
        return isInLight;
    }

    public void RegisterLightSource(Sc_LightSource source)
    {
        if (!_lightSources.Contains(source))
        {
            _lightSources.Add(source);
        }
    }

    public void UnregisterLightSource(Sc_LightSource source)
    {
        _lightSources.Remove(source);
    }
}
