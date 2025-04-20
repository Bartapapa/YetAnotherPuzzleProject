using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Dialogue/BaseVoiceParameters", fileName = "Voice_InternalName_Alias")]
public class SO_DialogueVoiceParameters : ScriptableObject
{
    public int BlipCharacterDelay = 1;
    [SerializeField] private List<AudioClip> _voiceBlips = new List<AudioClip>();
    [SerializeField] private Vector2 _voiceMinMaxPitch = new Vector2(1f, 1f);
    [SerializeField] private float _volume = 1f;

    public void VoiceBlip()
    {
        Sc_SoundManager soundManager = Sc_SoundManager.instance;
        if (!soundManager)
        {
            Debug.LogWarning("No SoundManager found! Cannot create voiceBlip.");
            return;
        }

        soundManager.PlayRandomSFX(soundManager.UISource, _voiceBlips, _volume, _voiceMinMaxPitch);
    }
}
