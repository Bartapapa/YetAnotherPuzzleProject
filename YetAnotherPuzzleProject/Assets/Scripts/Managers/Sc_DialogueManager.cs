using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_DialogueManager : MonoBehaviour
{
    public static Sc_DialogueManager instance { get; private set; }

    [Header("CURRENT DIALOGUE")]
    [SerializeField][ReadOnly] private SO_Dialogue _currentDialogue;
    [SerializeField][ReadOnly] private int _currentLineIndex = -1;
    public SO_Dialogue CurrentDialogue { get { return _currentDialogue; } }

    private string _startOfLineEventMethodName = "";
    private string _endOfLineEventMethodName = "";

    public delegate void DefaultEvent();
    public event DefaultEvent DialogueStarted;
    public event DefaultEvent DialogueEnded;

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

    public void StartDialogue(SO_Dialogue dialogue, bool force = false)
    {
        Sc_UIManager UImanager = Sc_UIManager.instance;
        Sc_StoryManager storyManager = Sc_StoryManager.instance;
        if (!UImanager || !storyManager) return;
        if (_currentDialogue != null)
        {
            if (force)
            {
                EndCurrentDialogue();
            }
            else
            {
                Debug.LogWarning("Dialogue is already playing! Returning.");
                return;
            }
        }

        _currentDialogue = dialogue;

        //zoom camera
        //open dialogue box
        UImanager.OpenDialogueBox();
        //start PlayDialogueLine with first line of dialogue.
        PlayDialogueLine(_currentDialogue.Lines[0]);
        _currentLineIndex = 0;

        DialogueStarted?.Invoke();
    }

    private void PlayDialogueLine (DialogueLine line)
    {
        if (line.StartLineEvent.EventName.Length <= 0)
        {
            //No method here.
        }
        else
        {
            Debug.LogWarning("Found start of line event: " + line.StartLineEvent.EventName);
            _startOfLineEventMethodName = line.StartLineEvent.EventName;
        }
        if (line.EndLineEvent.EventName.Length <= 0)
        {
            //No method here.
        }
        else
        {
            Debug.LogWarning("Found end of line event: " + line.EndLineEvent.EventName);
            _endOfLineEventMethodName = line.EndLineEvent.EventName;
        }

        //Write speaker name (or alias, if any useAliasID) and line text
        WriteLine(line);
        OnWriteLineStart();
        //Invoke methods if any startline event within StoryManager
        //Write out line coroutine - remember, can be skipped
        //Invoke methods if any endline event within StoryManager
        //If choice, then prompt choice from player.
    }

    private void WriteLine (DialogueLine line)
    {
        Sc_UIManager.instance.DialogueBox.WriteLine(line);
    }

    public void GoToNextLine()
    {
        if (Sc_UIManager.instance.DialogueBox.IsWritingDialogueText)
        {
            Sc_UIManager.instance.DialogueBox.SkipWritingCurrentText();
        }
        else
        {
            _currentLineIndex++;
            if (_currentLineIndex >= _currentDialogue.Lines.Count)
            {
                EndCurrentDialogue();
            }
            else
            {
                PlayDialogueLine(_currentDialogue.Lines[_currentLineIndex]);
            }
        }
    }

    private void SkipCurrentLine()
    {
        //Immediately write to the end of line.
    }

    public void OnWriteLineStart()
    {
        //Play out start of line events, if any.
        if (_startOfLineEventMethodName.Length >= 0)
        {
            Invoke(_startOfLineEventMethodName, 0f);
            _startOfLineEventMethodName = "";
        }
    }

    public void OnWriteLineEnd()
    {
        //Play out end of line events, if any.
        if (_endOfLineEventMethodName.Length >= 0)
        {
            Invoke(_endOfLineEventMethodName, 0f);
            _endOfLineEventMethodName = "";
        }

        //If choice, prompt choice input from player.
    }

    public void EndCurrentDialogue()
    {
        //unzoom camera
        //close dialogue box
        Sc_UIManager.instance.CloseDialogueBox();

        _currentDialogue = null;
        _currentLineIndex = -1;

        DialogueEnded?.Invoke();
    }

    #region DialogueLineEvents
    #region TestDialogue
    private void TestDialogue_Line01_OnLineStart()
    {
        Debug.LogWarning("Line01_OnLineStart event!");
        Sc_StoryManager.instance.AddQuestToActiveQuests(0);
    }
    private void TestDialogue_Line01_OnLineEnd()
    {
        Debug.LogWarning("Line01_OnLineEnd event!");
    }
    #endregion
    #endregion
}
