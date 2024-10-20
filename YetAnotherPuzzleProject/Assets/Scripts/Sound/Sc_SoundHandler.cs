using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_SoundHandler : MonoBehaviour
{
    [Header("SOUND OBJECT PREFAB")]
    public Sc_SoundStimuli SoundObject;

    protected void GenerateSoundObject(Vector3 location, float range = .5f, float duration = 1f)
    {
        Sc_SoundStimuli sstimuli = Instantiate<Sc_SoundStimuli>(SoundObject, location, Quaternion.identity);
        sstimuli.InitSoundStimuli(range, duration);
    }
}
