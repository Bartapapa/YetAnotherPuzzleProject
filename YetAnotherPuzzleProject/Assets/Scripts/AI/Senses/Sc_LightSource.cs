using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_LightSource : MonoBehaviour
{
    public bool GlobalLight = false;
    [ReadOnly] public List<Sc_VisualStimuli> VisualStimuliInRange = new List<Sc_VisualStimuli>();

    private void OnDisable()
    {
        foreach(Sc_VisualStimuli vstimuli in VisualStimuliInRange)
        {
            vstimuli.UnregisterLightSource(this);
        }
        VisualStimuliInRange.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        Sc_VisualStimuli vstimuli = other.GetComponent<Sc_VisualStimuli>();
        if (vstimuli)
        {
            vstimuli.RegisterLightSource(this);
            AddVisualStimuli(vstimuli);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Sc_VisualStimuli vstimuli = other.GetComponent<Sc_VisualStimuli>();
        if (vstimuli)
        {
            vstimuli.UnregisterLightSource(this);
            RemoveVisualStimuli(vstimuli);
        }
    }

    private void AddVisualStimuli(Sc_VisualStimuli vstimuli)
    {
        if (!VisualStimuliInRange.Contains(vstimuli))
        {
            VisualStimuliInRange.Add(vstimuli);
        }
    }

    private void RemoveVisualStimuli(Sc_VisualStimuli vstimuli)
    {
        VisualStimuliInRange.Remove(vstimuli);
    }
}
