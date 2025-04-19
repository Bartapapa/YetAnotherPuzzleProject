using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeakerAlias
{
    public string Alias = "";
    public Material Font;
    public Color NameColor = Color.white;
    //Voice?
}

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Dialogue/BaseDialogueSpeaker", fileName = "Speaker_InternalName")]
public class SO_DialogueSpeaker : ScriptableObject
{
    [Header("PARAMETERS")]
    [SerializeField] private string _internalName = "";
    public List<SpeakerAlias> Aliases = new List<SpeakerAlias>();

    [Header("SPEECH")]
    public float BaseCharacterDelay = .06f;

    public string InternalName { get { return _internalName; } }
}
