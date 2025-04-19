using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_UIManager : MonoBehaviour
{
    public static Sc_UIManager instance { get; private set; }

    [Header("OBJECT REFS")]
    public Sc_Transition Transitioner;
    public Sc_DialogueBox DialogueBox;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    #region Dialogue
    public void OpenDialogueBox(bool force = false)
    {
        DialogueBox.ShowBox(force);
    }

    public void CloseDialogueBox(bool force = false)
    {
        DialogueBox.CloseBox(force);
    }
    #endregion

}
