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
    [SerializeField] [TextArea(3, 5)] private string _internalSpokenLine = "";
    public List<DialogueLineEvent> DialogueVariableGet = new List<DialogueLineEvent>();
    public string SpokenLine { get { return ParseInternalSpokenLine(); } }
    public float OverrideCharacterDelay = -1f;
    public DialogueLineEvent StartLineEvent;
    public DialogueLineEvent EndLineEvent;

    public SpeakerAlias GetAlias { get { return Speaker.Aliases[UseAliasID]; } }

    protected virtual string ParseInternalSpokenLine()
    {
        string parsedLines = _internalSpokenLine;
        int parsedVarStartIndex = 0;
        int length = 0;
        bool parsingVar = false;
        List<string> parsedVariables = new List<string>();

        //Create substrings for each variable.
        for (int i = 0; i < parsedLines.Length; i++)
        {
            if (parsedLines[i] == "#"[0])
            {
                if (!parsingVar)
                {
                    parsedVarStartIndex = i;
                    parsingVar = true;
                }
                else
                {
                    string newParsedVar = parsedLines.Substring(parsedVarStartIndex, length+1);
                    parsedVariables.Add(newParsedVar);

                    parsedVarStartIndex = 0;
                    length = 0;
                    parsingVar = false;
                }
            }

            if (parsingVar)
            {
                length++;
            }
        }

        //Create a method for each variable necessary to get in a child SO.
        Sc_DialogueManager dialogueManager = Sc_DialogueManager.instance;
        for (int i = 0; i < parsedVariables.Count; i++)
        {
            string methodName = parsedVariables[i].Replace("#", "");
            string replacedString = "?DID_NOT_FIND_VAR?";
            switch (methodName)
            {
                case "GetTestNumber01":
                    replacedString = dialogueManager.GetTestNumber01();
                    break;
                case "GetTestNumber02":
                    replacedString = dialogueManager.GetTestNumber02();
                    break;
                case "GetTestSpokenToNumber":
                    replacedString = dialogueManager.GetTestSpokenToNumber();
                    break;
                default:
                    break;
            }
            parsedLines = parsedLines.Replace(parsedVariables[i], replacedString);
        }

        return parsedLines;
    }
}

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Dialogue/BaseDialogue", fileName = "Dialogue_Speaker_Event")]
public class SO_Dialogue : ScriptableObject
{
    public List<DialogueLine> Lines = new List<DialogueLine>();
}
