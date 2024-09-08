using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Pillar : MonoBehaviour
{
    [Header("PARAMETERS")]
    public float _travelDistance = 2f;
    public float _overTime = 1f;
    public AnimationCurve _movementCurve;

    protected Rigidbody _rb;
    private Coroutine _movementCo;
    private Vector3 _bottomPos;
    private Vector3 _topPos;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogWarning(this.name + " doesn't have a Rigidbody!");
            return;
        }

        _bottomPos = transform.position;
        _topPos = _bottomPos + (transform.up * _travelDistance);
    }

    public void Move(bool activated)
    {
        if (_movementCo != null)
        {
            StopCoroutine(_movementCo);
        }
        _movementCo = StartCoroutine(Movement(activated));
    }

    public void GaugeMove(float gauge)
    {
        if (gauge > 1f) gauge = 1f;

        float alpha = _movementCurve.Evaluate(gauge / 1f);
        Vector3 newPos = Vector3.Lerp(_bottomPos, _topPos, alpha);
        _rb.Move(newPos, _rb.rotation);

        if (gauge >= 1f)
        {
            OnReachedTop();
        }
        else if (gauge <= 0f)
        {
            OnReachedBottom();
        }
    }

    public void ForceMove(bool activated)
    {
        if (_movementCo != null)
        {
            StopCoroutine(_movementCo);
            _movementCo = null;          
        }

        _rb.Move(activated ? _topPos : _bottomPos, _rb.rotation);

        if (activated)
        {
            OnReachedTop();
        }
        else
        {
            OnReachedBottom();
        }
    }

    private IEnumerator Movement(bool up)
    {
        Vector3 fromPos = transform.position;
        Vector3 toPos = up ? _topPos : _bottomPos;
        float time = 0f;
        while (time < _overTime)
        {
            float alpha = _movementCurve.Evaluate(time / _overTime);
            Vector3 newPos = Vector3.Lerp(fromPos, toPos, alpha);
            //transform.position = newPos;
            _rb.Move(newPos, _rb.rotation);
            time += Time.deltaTime;
            yield return null;
        }
        _rb.Move(toPos, _rb.rotation);
        //transform.position = toPos;
        if (up)
        {
            OnReachedTop();
        }
        else
        {
            OnReachedBottom();
        }
        _movementCo = null;
    }

    protected virtual void OnReachedTop()
    {

    }

    protected virtual void OnReachedBottom()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.up * _travelDistance));
        Gizmos.DrawSphere(transform.position + (transform.up * _travelDistance), .1f);
    }
}
