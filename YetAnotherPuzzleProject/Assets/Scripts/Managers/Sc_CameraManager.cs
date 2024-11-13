using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using static UnityEngine.Rendering.DebugUI.Table;

public class Sc_CameraManager : MonoBehaviour
{
    public static Sc_CameraManager instance { get; private set; }

    [Header("CAMERAS")]
    public CinemachineVirtualCamera _defaultVC;
    public Sc_CameraFocus _defaultCameraFocus;

    [Header("ROTATION")]
    public AnimationCurve RotateCurve;

    private Coroutine _cameraRotateCO;
    private Coroutine _cameraShakeCO;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    #region CAMERAFOCUS

    public void AddFocus(Sc_CameraFocus toFocus, Transform focus, int weight = 1)
    {
        Debug.Log(focus.gameObject.name + " has been added to camera focus.");
        FocusObject newFocusObject = new FocusObject(focus, weight);
        toFocus.FocusObjects.Add(newFocusObject);
    }

    public void RemoveFocus(Sc_CameraFocus fromFocus, Transform focus)
    {
        foreach(FocusObject focusObject in fromFocus.FocusObjects)
        {
            if (focus == focusObject.Focus)
            {
                fromFocus.FocusObjects.Remove(focusObject);
                break;
            }
        }
    }

    public bool DoesCameraFocusHaveFocus(Sc_CameraFocus cameraFocus, Transform focus)
    {
        bool hasFocus = false;

        foreach (FocusObject focusObject in cameraFocus.FocusObjects)
        {
            if (focusObject.Focus == focus)
            {
                Debug.Log(focusObject.Focus.GetInstanceID() + " is the same as " + focus.GetInstanceID() + ". Not adding focus.");
                hasFocus = true;
                break;
            }
        }

        return hasFocus;
    }

    #endregion

    #region CAMERACOMMANDS

    public void ShiftCamera(float sign)
    {
        if (_cameraRotateCO != null) return;
        float degreesEuler = sign >= 0f ? -90f : 90f;
        RotateDefaultCamera(degreesEuler, 3f);
    }

    public void RotateDefaultCamera(float degreesEuler, float overTime)
    {
        if (_cameraRotateCO != null)
        {
            StopCoroutine(_cameraRotateCO);
        }
        _cameraRotateCO = StartCoroutine(RotateCamera(_defaultCameraFocus, degreesEuler, overTime));
    }

    private IEnumerator RotateCamera(Sc_CameraFocus focus, float degreesEuler, float overTime)
    {
        float time = 0f;
        Quaternion fromRot = focus.transform.rotation;
        Quaternion toRot = Quaternion.Euler(focus.transform.rotation.eulerAngles + new Vector3(0, degreesEuler, 0f));

        while (time < overTime)
        {
            Quaternion rot = Quaternion.LerpUnclamped(fromRot, toRot, RotateCurve.Evaluate(time/overTime));
            focus.transform.rotation = rot;
            time += Time.deltaTime;
            yield return null;
        }

        focus.transform.rotation = toRot;
        _cameraRotateCO = null;
    }

    public void ShakeDefaultCamera(float intensity, float overTime, float blendTime)
    {
        if (_cameraShakeCO != null)
        {
            StopCoroutine(_cameraShakeCO);
        }
        _cameraShakeCO = StartCoroutine(ShakeCamera(_defaultVC, intensity, overTime, blendTime));
    }

    private IEnumerator ShakeCamera(CinemachineVirtualCamera camera, float intensity, float overTime, float blendTime)
    {
        float time = 0f;
        CinemachineBasicMultiChannelPerlin perlin = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        float amplitude = 0f;

        if (blendTime * 2f > overTime)
        {
            blendTime = overTime * .5f;
        }

        float blendInTimeMarker = blendTime;
        float blendOutTimeMarker = overTime - blendTime;

        perlin.m_AmplitudeGain = 0f;

        while (time < overTime)
        {
            if (time <= blendInTimeMarker)
            {
                float blendInAlpha = time / blendTime;
                amplitude = Mathf.Lerp(0f, intensity, blendInAlpha);
                perlin.m_AmplitudeGain = amplitude;
            }
            else if (time > blendInTimeMarker && time < blendOutTimeMarker)
            {
                perlin.m_AmplitudeGain = intensity;
            }
            else if (time >= blendOutTimeMarker)
            {
                float blendOutAlpha = time - blendOutTimeMarker / blendTime;
                amplitude = Mathf.Lerp(intensity, 0f, blendOutAlpha);
                perlin.m_AmplitudeGain = amplitude;
            }

            time += Time.deltaTime;
            yield return null;
        }

        perlin.m_AmplitudeGain = 0f;

        _cameraShakeCO = null;
    }

    public void CameraShake(CinemachineImpulseSource impulseSource, float force = 1f)
    {
        impulseSource.GenerateImpulseWithForce(force);
    }
    #endregion
}
