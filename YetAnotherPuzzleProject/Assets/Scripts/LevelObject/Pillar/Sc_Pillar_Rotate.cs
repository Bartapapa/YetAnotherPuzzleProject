using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Pillar_Rotate : Sc_Pillar
{
    [Header("ROTATE PARAMETERS")]
    public float _rotateAngle = 90f;
    public float _rotateOverTime = 1f;

    private Coroutine _rotateCo;

    private List<Sc_Pushable> _registeredPushables = new List<Sc_Pushable>();

    protected override void OnReachedTop()
    {
        if (_rotateCo != null)
        {
            StopCoroutine(_rotateCo);
        }
        _rotateCo = StartCoroutine(Rotate());
    }

    protected override void OnReachedBottom()
    {
        base.OnReachedBottom();
    }

    private IEnumerator Rotate()
    {
        float time = 0f;
        Quaternion originalRot = _rb.rotation;
        Quaternion destRot = Quaternion.Euler(_rb.rotation.eulerAngles + new Vector3(0, _rotateAngle, 0f));
        //Vector3 originalRot = transform.rotation.eulerAngles;
        //Vector3 toEulerRot = transform.rotation.eulerAngles + new Vector3(0f, transform.rotation.eulerAngles.y + _rotateAngle, 0f);
        while (time < _rotateOverTime)
        {
            float alpha = _movementCurve.Evaluate(time / _rotateOverTime);
            Quaternion toRot = Quaternion.Lerp(originalRot, destRot, alpha);
            //Vector3 toRot = Vector3.Slerp(originalRot, toEulerRot, alpha);
            //_rb.Move(_rb.position, toRot);
            _rb.MoveRotation(toRot);
            //transform.rotation = toRot;
            time += Time.deltaTime;
            yield return null;
        }
        //_rb.Move(_rb.position, destRot);
        //transform.rotation = destRot;
        _rb.MoveRotation(destRot);
        _rotateCo = null;

        Move(false);
    }
}
