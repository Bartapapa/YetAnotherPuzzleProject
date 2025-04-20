using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Dialoguer : MonoBehaviour
{
    [Header("DIALOGUE OBJECT REFS")]
    public Sc_Interactible Interactible;
    public SO_ContextParser ContextParser;
    public List<SO_Dialogue> Dialogues = new List<SO_Dialogue>();

    private Sc_Character_Player _currentInteractingPlayerCharacter;
    private Sc_DialogueManager _dialogueManager;

    private SO_Dialogue GetDialogueFromContext(StoryContext context)
    {
        int dialogueIndex = 0;
        if (ContextParser != null)
        {
            dialogueIndex = ContextParser.ParseContext(context);
        }
        else
        {
            Debug.LogWarning(this.name + " has no context parser, returning dialogue 0 by default.");
        }
        if (dialogueIndex > Dialogues.Count - 1)
        {
            Debug.LogWarning(this.name + " tried to find a dialogue that doesn't exist! Wanted to find dialogue index " + dialogueIndex + ", but dialogue count is " + Dialogues.Count + ". Returning index 0.");
            dialogueIndex = 0;
        }
        return Dialogues[dialogueIndex];
    }

    public void StartDialogue(Sc_Character interactor)
    {
        Interactible.CanBeInteractedWith = false;

        Sc_DialogueManager dialogueManager = Sc_DialogueManager.instance;
        if (!dialogueManager)
        {
            Debug.LogWarning("No dialoguemanager found! Returning.");
            return;
        }
        _dialogueManager = dialogueManager;

        dialogueManager.DialogueStarted -= OnDialogueStart;
        dialogueManager.DialogueEnded -= OnDialogueEnd;
        dialogueManager.DialogueStarted += OnDialogueStart;
        dialogueManager.DialogueEnded += OnDialogueEnd;

        //Finds a dialogue among the list of dialogues by parsing the current story context, and plays it.
        dialogueManager.StartDialogue(GetDialogueFromContext(Sc_StoryManager.instance.Context));

        //Make interactor in dialogue - notably to change player input and how that reacts. Perhaps change input mapping completely?
        Sc_Character_Player playerCharacter = interactor.GetComponent<Sc_Character_Player>();
        if (playerCharacter)
        {
            _currentInteractingPlayerCharacter = playerCharacter;
            playerCharacter.ControllingPlayer.SwitchActionMap("Dialogue");
        }
    }

    private void OnDialogueStart()
    {

    }

    private void OnDialogueEnd()
    {
        _currentInteractingPlayerCharacter.ControllingPlayer.SwitchActionMap("Player");
        _currentInteractingPlayerCharacter = null;
        Interactible.CanBeInteractedWith = true;

        _dialogueManager.DialogueStarted -= OnDialogueStart;
        _dialogueManager.DialogueEnded -= OnDialogueEnd;
        _dialogueManager = null;
    }
}
