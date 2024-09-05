using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class Sc_CameraManager : MonoBehaviour
{
    public static Sc_CameraManager instance { get; private set; }

    [Header("CAMERAS")]
    public CinemachineVirtualCamera _defaultVC;
    public Sc_CameraFocus _defaultCameraFocus;

    private Coroutine _cameraRotateCO;

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

    public void AddFocus(Sc_CameraFocus toFocus, Transform focus, int weight = 1)
    {
        FocusObject newFocusObject = new FocusObject(focus, weight);
        toFocus.FocusObjects.Add(newFocusObject);
    }

    public void RemoveFocus(Sc_CameraFocus fromFocus, Transform focus)
    {
        foreach(FocusObject focusObject in fromFocus.FocusObjects)
        {
            if (focus = focusObject.Focus)
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
            if (focus = focusObject.Focus)
            {
                hasFocus = true;
                break;
            }
        }

        return hasFocus;
    }

    public void RotateDefaultCamera(float degreesEuler)
    {
        if (_cameraRotateCO != null)
        {
            StopCoroutine(_cameraRotateCO);
        }
        _cameraRotateCO = StartCoroutine(RotateCamera(_defaultCameraFocus, degreesEuler, 2f));
    }

    private IEnumerator RotateCamera(Sc_CameraFocus focus, float degreesEuler, float overTime)
    {
        float time = 0f;
        Quaternion fromRot = focus.transform.rotation;
        Quaternion toRot = Quaternion.Euler(focus.transform.rotation.eulerAngles + new Vector3(0, degreesEuler, 0f));

        while (time < overTime)
        {
            Quaternion rot = Quaternion.Lerp(fromRot, toRot, time/overTime);
            focus.transform.rotation = rot;
            time += Time.deltaTime;
            yield return null;
        }

        focus.transform.rotation = toRot;
        _cameraRotateCO = null;
    }
}
