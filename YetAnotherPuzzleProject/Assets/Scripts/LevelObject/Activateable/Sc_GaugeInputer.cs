using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_GaugeInputer : MonoBehaviour
{
    [Header("LINKED GAUGES")]
    public List<Sc_Gauge> _gauges = new List<Sc_Gauge>();

    [Header("PARAMETERS")]
    public float _inputRate = 1f;
    public bool _continuous = false;

    public void ApplyInput()
    {
        float adjustedInput = _continuous ? _inputRate * Time.deltaTime : _inputRate;

        foreach(Sc_Gauge gauge in _gauges)
        {
            gauge.Input(adjustedInput);
        }
    }
}
