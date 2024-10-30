using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Lock : MonoBehaviour
{
    [Header("LOCK PARAMETERS")]
    public bool StartEngaged = true;
    [ReadOnly] public bool IsEngaged;

    [Header("SPIN PARAMETERS")]
    public float SpinDuration = 1f;
    public float SpinRotation = 360f;

    [Header("MESH REFS")]
    public Transform TopPivot;
    public Transform BottomPivot;

    private Coroutine _spinCo;
    private float _input = 0f;

    private void Start()
    {
        ToggleEngage(StartEngaged);
    }

    public void ToggleEngage(bool engage)
    {
        if (engage)
        {
            Engage();
        }
        else
        {
            Disengage();
        }
    }

    public void Engage()
    {
        if (IsEngaged) return;
        IsEngaged = true;

        TopPivot.localPosition = new Vector3(0f, 0f, 0f);
        TopPivot.eulerAngles = Vector3.zero;
        BottomPivot.eulerAngles = Vector3.zero;
    }

    public void Disengage()
    {
        if (!IsEngaged) return;
        IsEngaged = false;

        TopPivot.localPosition = new Vector3(0f, .75f, 0f);
        TopPivot.eulerAngles = Vector3.zero;
        BottomPivot.eulerAngles = Vector3.zero;
    }

    public void Spin(bool activated)
    {
        StopSpin();
        _spinCo = StartCoroutine(SpinCoroutine(activated));
    }

    public void GaugeSpin(float gauge, bool input = false)
    {
        float alpha = 0f;
 
        if (input)
        {
            _input += gauge;
            alpha = _input;
        }
        else
        {
            alpha = gauge;
            
        }

        Vector3 euler = new Vector3(0f, SpinRotation * alpha, 0f);
        if (IsEngaged)
        {
            TopPivot.eulerAngles = euler;
            BottomPivot.eulerAngles = euler;
        }
        else
        {
            TopPivot.eulerAngles = Vector3.zero;
            BottomPivot.eulerAngles = euler;
        }
    }

    private void StopSpin()
    {
        if (_spinCo != null)
        {
            StopCoroutine(_spinCo);
            _spinCo = null;
        }

        TopPivot.eulerAngles = Vector3.zero;
        BottomPivot.eulerAngles = Vector3.zero;
    }

    private IEnumerator SpinCoroutine(bool activated)
    {
        float timer = 0f;
        TopPivot.eulerAngles = Vector3.zero;
        BottomPivot.eulerAngles = Vector3.zero;

        while (timer < SpinDuration)
        {
            float rotateValue = (SpinRotation / SpinDuration) * Time.deltaTime;
            float multiplier = activated ? 1f : -1f;
            rotateValue = rotateValue * multiplier;
            if (IsEngaged)
            {
                TopPivot.Rotate(0f, rotateValue, 0f);
                BottomPivot.Rotate(0f, rotateValue, 0f);
            }
            else
            {
                BottomPivot.Rotate(0f, rotateValue, 0f);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        TopPivot.eulerAngles = Vector3.zero;
        BottomPivot.eulerAngles = Vector3.zero;
    }
}
