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

    #region Activateable implementation
    public override bool Activate(bool toggleOn)
    {
        if (base.Activate(toggleOn))
        {
            StartRotate();
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void ForceActivate(bool toggleOn)
    {
        //base.ForceActivate(toggleOn);
        //ForceRotate();
    }
    #endregion

    private void TransmitRotation(float angle, Quaternion difference)
    {
        foreach (Sc_CharacterController controller in _parentedControllers)
        {
            Vector3 point = new Vector3(controller.transform.position.x, 0f, controller.transform.position.z);
            Vector3 origin = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 toPoint = RotatePointAroundPoint(point, origin, angle);
            toPoint = new Vector3(toPoint.x, 0f, toPoint.y);
            Vector3 toVel = (toPoint - point)/Time.fixedDeltaTime;

            controller.InheritedVelocity += toVel * .485f;
            controller.InheritedYaw -= angle * .48f;
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

    }

    protected override void OnReachedBottom()
    {

    }

    public void ForceRotate()
    {
        StopRotate();

    }

    public void GaugeRotate(float gauge)
    {
        if (Lock != null)
        {
            //Lock.GaugeSpin(gauge);
            if (!Lock.IsActivated) return;
        }

        Vector3 euler = new Vector3(0f, _rotateAngle * gauge, 0f);     
        Quaternion newRot = Quaternion.Euler(euler);
        float angle = Quaternion.Angle(_rb.rotation, newRot);
        Quaternion difference = Quaternion.Inverse(transform.rotation) * newRot;
        _rb.MoveRotation(newRot);

        if (gauge > _cachedGauge)
        {
            TransmitRotation(-angle, difference);
            ContinuousStoneScrape(true);
        }
        else if(gauge < _cachedGauge)
        {
            TransmitRotation(angle, difference);
            ContinuousStoneScrape(false);
        }

        RebuildNavMesh();

        _cachedGauge = gauge;
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
            _rotateCo = null;
        }
    }

    private IEnumerator Rotate()
    {
        float time = 0f;
        float angle;
        Quaternion difference;
        Quaternion originalRot = _rb.rotation;
        Quaternion destRot = Quaternion.Euler(_rb.rotation.eulerAngles + new Vector3(0, _rotateAngle, 0f));

        while (time < _rotateOverTime)
        {
            //Pillar sound
            ContinuousStoneScrape(true);

            //Find angle values to parse into TransmitRotation()
            float alpha = MovementCurve.Evaluate(time / _rotateOverTime);
            Quaternion toRot = Quaternion.Lerp(originalRot, destRot, alpha);
            angle = Quaternion.Angle(_rb.rotation, toRot);
            difference = Quaternion.Inverse(transform.rotation) * toRot;

            _rb.MoveRotation(toRot);
            if (_rotateAngle >= 0)
            {
                TransmitRotation(-angle, difference);
            }
            else
            {
                TransmitRotation(angle, difference);
            }

            time += Time.deltaTime;

            RebuildNavMesh();

            yield return null;
        }

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

    }
}
