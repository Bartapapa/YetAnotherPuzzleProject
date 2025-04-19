using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLineEvent
{
    public string EventName = "";
}

[System.Serializable]
public class DialogueLine
{
    public SO_DialogueSpeaker Speaker;
    public int UseAliasID = 0;
    [TextArea(3, 5)] public string SpokenLine = "";
    public float OverrideCharacterDelay = -1f;
    public int GoToLineIndex = -1;
    public Vector2Int ChoiceGoToLineIndex = new Vector2Int(-1, -1);
    public DialogueLineEvent StartLineEvent;
    public DialogueLineEvent EndLineEvent;

    public SpeakerAlias GetAlias { get { return Speaker.Aliases[UseAliasID]; } }
}

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Dialogue/BaseDialogue", fileName = "Dialogue_Speaker_Event")]
public class SO_Dialogue : ScriptableObject
{
    public List<DialogueLine> Lines = new List<DialogueLine>();
}
