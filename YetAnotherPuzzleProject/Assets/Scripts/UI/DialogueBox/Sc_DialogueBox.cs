using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sc_DialogueBox : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Transform ParentGroup;
    public TMP_Text DialogueText;
    public TMP_Text NameTagText;

    public bool IsWritingDialogueText { get { return _currentDialogueTextWritingCO == null ? false : true; } }
    public bool IsWritingNameTagText { get { return _currentNameTagWritingCO == null ? false : true; } }

    private SpeakerAlias _currentSpeakerAlias;
    private Coroutine _currentNameTagWritingCO;
    private Coroutine _currentDialogueTextWritingCO;
    private int _currentDialogueLineVisibleCharacterIndex = 0;
    private int _currentVoiceBlipCharacterIndex = 0;
    private WaitForSeconds _baseCharacterDelay;
    private WaitForSeconds _basePunctuationDelay;

    public void ShowBox(bool force = false)
    {
        //Play animation of opening, return OnBoxOpened when complete
        ParentGroup.gameObject.SetActive(true);
    }

    public void CloseBox(bool force = false)
    {
        //Play animation of closing, return OnBoxClosed when complete
        ParentGroup.gameObject.SetActive(false);
    }

    public void WriteLine(DialogueLine line)
    {
        WriteNameTag(line);
        WriteDialogueText(line);
    }

    private void WriteNameTag(DialogueLine line)
    {
        if (_currentSpeakerAlias == line.GetAlias)
        {
            //Do nothing, it's all good.
        }
        else
        {
            _currentSpeakerAlias = line.GetAlias;
            NameTagText.text = _currentSpeakerAlias.Alias;
            NameTagText.fontMaterial = _currentSpeakerAlias.Font;
            NameTagText.color = _currentSpeakerAlias.NameColor;
        }
    }

    private void WriteDialogueText(DialogueLine line)
    {
        if (_currentDialogueTextWritingCO != null)
        {
            StopCoroutine(_currentDialogueTextWritingCO);
            _currentDialogueTextWritingCO = null;
        }

        DialogueText.text = line.SpokenLine;
        DialogueText.maxVisibleCharacters = 0;
        _currentDialogueLineVisibleCharacterIndex = 0;
        _currentDialogueTextWritingCO = StartCoroutine(DialogueTextWritingCO(line));
    }

    private IEnumerator DialogueTextWritingCO(DialogueLine line)
    {
        DialogueText.ForceMeshUpdate();
        TMP_TextInfo textInfo = DialogueText.textInfo;
        float baseDelay = line.OverrideCharacterDelay >= 0f ? line.OverrideCharacterDelay : line.Speaker.BaseCharacterDelay;
        WaitForSeconds characterDelay = new WaitForSeconds(baseDelay);
        WaitForSeconds punctuationDelay = new WaitForSeconds(baseDelay * 4f);

        SO_DialogueVoiceParameters speakerVoice = line.GetAlias.Voice;
        if (speakerVoice != null)
        {
            //Blip at start of line, whatever the circumstances.
            speakerVoice.VoiceBlip();
        }

        while (_currentDialogueLineVisibleCharacterIndex < textInfo.characterCount)
        {
            char character = textInfo.characterInfo[_currentDialogueLineVisibleCharacterIndex].character;
            DialogueText.maxVisibleCharacters++;

            if(character == "?"[0] || character == "!"[0] || character == "."[0] || character == ","[0] || character == ":"[0] || character == ";"[0] || character == "-"[0])
            {
                yield return punctuationDelay;
            }
            else
            {
                _currentVoiceBlipCharacterIndex++;
                yield return characterDelay;
            }

            _currentDialogueLineVisibleCharacterIndex++;

            if (speakerVoice != null)
            {
                if (_currentVoiceBlipCharacterIndex >= speakerVoice.BlipCharacterDelay)
                {
                    _currentVoiceBlipCharacterIndex = 0;
                    speakerVoice.VoiceBlip();
                }
            }
        }

        _currentVoiceBlipCharacterIndex = 0;
        OnDialogueTextEndWriting();
        _currentDialogueTextWritingCO = null;
    }

    private void OnDialogueTextEndWriting()
    {
        Sc_DialogueManager.instance.OnWriteLineEnd();
    }

    public void SkipWritingCurrentText()
    {
        if (_currentDialogueTextWritingCO != null)
        {
            StopCoroutine(_currentDialogueTextWritingCO);
            _currentDialogueTextWritingCO = null;
        }
        DialogueText.maxVisibleCharacters = DialogueText.textInfo.characterCount;

        OnDialogueTextEndWriting();
    }
}
