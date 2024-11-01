using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class Sc_Pillar_Rotate : Sc_Pillar
{
    [Header("ROTATE PARAMETERS")]
    public float _rotateAngle = 90f;
    public float _rotateOverTime = 1f;

    private Coroutine _rotateCo;

    private List<Sc_Pushable> _registeredPushables = new List<Sc_Pushable>();

    Quaternion _originalRot;
    Quaternion _destRot;

    protected override void Awake()
    {
        base.Awake();

        _originalRot = _rb.rotation;
        _destRot = Quaternion.Euler(_rb.rotation.eulerAngles + new Vector3(0, _rotateAngle, 0f));
    }

    private void TransmitRotation(float angle, Quaternion difference)
    {
        //angle = angle * .22f;

        foreach (Sc_CharacterController controller in _parentedControllers)
        {
            Vector3 point = new Vector3(controller.transform.position.x, 0f, controller.transform.position.z);
            Vector3 origin = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 toPoint = RotatePointAroundPoint(point, origin, angle);
            toPoint = new Vector3(toPoint.x, 0f, toPoint.y);
            Vector3 toVel = (toPoint - point)/Time.fixedDeltaTime;

            controller.InheritedVelocity += toVel * .485f;
            controller.InheritedYaw -= angle * .48f;


            //Vector3 point = new Vector3(rb.position.x, 0f, rb.position.z);
            //Vector3 origin = new Vector3(transform.position.x, 0f, transform.position.z);
            //Vector3 toPoint = RotatePointAroundPoint(point, origin, angle);
            //toPoint = new Vector3(toPoint.x, rb.position.y, toPoint.y);

            ////Vector3 euler = rb.rotation.eulerAngles;
            ////euler = new Vector3(euler.x, euler.y -angle, euler.z);
            ////Quaternion newRot = Quaternion.Euler(euler);

            ////rb.MovePosition(toPoint);
            //Quaternion toRot = rb.transform.rotation * difference;
            //rb.Move(toPoint, toRot);
            ////rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, rb.transform.rotation * difference, .5f);
        }

        foreach (Sc_Pushable pushable in _parentedPushables)
        {
            Vector3 point = new Vector3(pushable.transform.position.x, 0f, pushable.transform.position.z);
            Vector3 origin = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 toPoint = RotatePointAroundPoint(point, origin, angle);
            toPoint = new Vector3(toPoint.x, 0f, toPoint.y);
            Vector3 toVel = (toPoint - point) / Time.fixedDeltaTime;

            pushable.InheritedVelocity += toVel * .50f;
            pushable.InheritedYaw -= angle * .48f;


            //Vector3 point = new Vector3(rb.position.x, 0f, rb.position.z);
            //Vector3 origin = new Vector3(transform.position.x, 0f, transform.position.z);
            //Vector3 toPoint = RotatePointAroundPoint(point, origin, angle);
            //toPoint = new Vector3(toPoint.x, rb.position.y, toPoint.y);

            ////Vector3 euler = rb.rotation.eulerAngles;
            ////euler = new Vector3(euler.x, euler.y -angle, euler.z);
            ////Quaternion newRot = Quaternion.Euler(euler);

            ////rb.MovePosition(toPoint);
            //Quaternion toRot = rb.transform.rotation * difference;
            //rb.Move(toPoint, toRot);
            ////rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, rb.transform.rotation * difference, .5f);
        }
    }

    private Vector3 RotatePointAroundPoint(Vector3 origin, Vector3 point, float angle)
    {
        angle *= Mathf.Deg2Rad;
        var x = Mathf.Cos(angle) * (origin.x - point.x) - Mathf.Sin(angle) * (origin.z - point.z) + point.x;
        var y = Mathf.Sin(angle) * (origin.x - point.x) + Mathf.Cos(angle) * (origin.z - point.z) + point.z;
        return new Vector3(x, y);
    }

    protected override void OnReachedTop()
    {
        //if (_rotateCo != null)
        //{
        //    StopCoroutine(_rotateCo);
        //}
        //_rotateCo = StartCoroutine(Rotate());
    }

    protected override void OnReachedBottom()
    {
        //base.OnReachedBottom();
    }

    public void GaugeRotate(float gauge)
    {
        //if (gauge > 1f) gauge = 1f;
        if (Lock != null)
        {
            Lock.GaugeSpin(gauge);
            if (!Lock.IsActivated) return;
        }

        Vector3 euler = new Vector3(0f, _rotateAngle * gauge, 0f);     
        Quaternion newRot = Quaternion.Euler(euler);
        float angle = Quaternion.Angle(_rb.rotation, newRot);
        Quaternion difference = Quaternion.Inverse(transform.rotation) * newRot;
        //_rb.MoveRotation(newRot);
        //euler = transform.rotation.eulerAngles;
        //euler = new Vector3(euler.x, euler.y - angle, euler.z);
        //newRot = Quaternion.Euler(euler);
        //transform.rotation = newRot;
        _rb.MoveRotation(newRot);

        Debug.Log(111);
        if (gauge > _cachedGauge)
        {
            Debug.Log(222);
            TransmitRotation(-angle, difference);
            ContinuousStoneScrape(true);
        }
        else if(gauge < _cachedGauge)
        {
            Debug.Log(333);
            TransmitRotation(angle, difference);
            ContinuousStoneScrape(false);
        }

        RebuildNavMesh();

        _cachedGauge = gauge;
        //transform.rotation = Quaternion.Euler(new Vector3(0f, _rotateAngle * gauge, 0f));
        //transform.Rotate(new Vector3(0f, _rotateAngle * gauge, 0f));
        //transform.rotation = newRot;
    }

    public void StartRotate()
    {
        if (Lock != null)
        {
            Lock.Spin(true);
            if (!Lock.IsActivated) return;
        }

        StopRotate();
        _rotateCo = StartCoroutine(Rotate());
    }

    public void StopRotate()
    {
        if (_rotateCo != null)
        {
            StopCoroutine(_rotateCo);
        }
    }

    private IEnumerator Rotate()
    {
        float time = 0f;
        float angle;
        Quaternion difference;
        Quaternion originalRot = _rb.rotation;
        Quaternion destRot = Quaternion.Euler(_rb.rotation.eulerAngles + new Vector3(0, _rotateAngle, 0f));

        //Vector3 originalRot = transform.rotation.eulerAngles;
        //Vector3 toEulerRot = transform.rotation.eulerAngles + new Vector3(0f, transform.rotation.eulerAngles.y + _rotateAngle, 0f);
        while (time < _rotateOverTime)
        {
            ContinuousStoneScrape(true);

            float alpha = _movementCurve.Evaluate(time / _rotateOverTime);
            Quaternion toRot = Quaternion.Lerp(originalRot, destRot, alpha);
            angle = Quaternion.Angle(_rb.rotation, toRot);
            difference = Quaternion.Inverse(transform.rotation) * toRot;
            //Vector3 toRot = Vector3.Slerp(originalRot, toEulerRot, alpha);
            //_rb.Move(_rb.position, toRot);
            _rb.MoveRotation(toRot);
            if (_rotateAngle >= 0)
            {
                TransmitRotation(-angle, difference);
            }
            else
            {
                TransmitRotation(angle, difference);
            }
            //transform.rotation = toRot;
            time += Time.deltaTime;

            RebuildNavMesh();

            yield return null;
        }
        //_rb.Move(_rb.position, destRot);
        //transform.rotation = destRot;
        angle = Quaternion.Angle(_rb.rotation, destRot);
        difference = Quaternion.Inverse(transform.rotation) * destRot;
        _rb.MoveRotation(destRot);
        if (_rotateAngle >= 0)
        {
            TransmitRotation(-angle, difference);
        }
        else
        {
            TransmitRotation(angle, difference);
        }

        RebuildNavMesh();

        _rotateCo = null;

        //Move(false);
    }
}
