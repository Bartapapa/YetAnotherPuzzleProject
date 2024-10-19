using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_LightSource : MonoBehaviour
{
    public bool GlobalLight = false;

    private void OnTriggerEnter(Collider other)
    {
        Sc_VisualStimuli vstimuli = other.GetComponent<Sc_VisualStimuli>();
        if (vstimuli)
        {
            vstimuli.RegisterLightSource(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Sc_VisualStimuli vstimuli = other.GetComponent<Sc_VisualStimuli>();
        if (vstimuli)
        {
            vstimuli.UnregisterLightSource(this);
        }
    }
}
