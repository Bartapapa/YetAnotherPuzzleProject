using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestObject
{
    private SO_QuestData _data;
    public SO_QuestData Data { get { return _data; } }
    public int CurrentStep = -1;
    public bool IsQuestFinished { get { return CurrentStep >= Data.Steps.Count; } }

    public QuestObject(SO_QuestData data)
    {
        _data = data;
    }
}
public class StoryContext
{
    public List<QuestObject> ActiveQuests;

    public StoryContext(List<QuestObject> activeQuests)
    {
        ActiveQuests = activeQuests;
    }

    public QuestObject GetActiveQuest(int id)
    {
        QuestObject quest = null;
        foreach(QuestObject queryQuest in ActiveQuests)
        {
            if (queryQuest.Data.ID == id)
            {
                quest = queryQuest;
            }
        }
        return quest;
    }

    public int GetQuestCurrentStep(int id)
    {
        int currentStep = -1;
        QuestObject quest = GetActiveQuest(id);
        if (quest != null)
        {
            currentStep = quest.CurrentStep;
        }
        return currentStep;
    }
}

public class Sc_StoryManager : MonoBehaviour
{
    public static Sc_StoryManager instance { get; private set; }

    [Header("QUESTS")]
    public SO_QuestDatabase QuestDB;
    [ReadOnly] public List<QuestObject> ActiveQuests = new List<QuestObject>();

    [Header("DEBUG")]
    public List<Vector2Int> OnStartQuests = new List<Vector2Int>();

    public StoryContext Context { get
        {
            StoryContext context = new StoryContext(ActiveQuests);
            return context;
        } }

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

    private void Start()
    {
        foreach(Vector2Int questIdAndStep in OnStartQuests)
        {
            AddQuestToActiveQuests(questIdAndStep.x, questIdAndStep.y);
        }
    }

    private SO_QuestData GetQuestFromDB(int id)
    {
        SO_QuestData quest = null;

        foreach(SO_QuestData queryQuest in QuestDB.Quests)
        {
            if (queryQuest.ID == id)
            {
                quest = queryQuest;
            }
        }

        return quest;
    }

    public void AddQuestToActiveQuests(int id, int toStep = 1)
    {
        SO_QuestData foundQuest = GetQuestFromDB(id);
        if (foundQuest == null)
        {
            Debug.LogWarning("Quest id " + id + " was not found in the quest DB!");
            return;
        }
        QuestObject foundQuestObject = Context.GetActiveQuest(id);
        if (foundQuestObject != null)
        {
            Debug.LogWarning("Quest id " + id + " is already an active quest!");
            return;
        }
        else
        {
            foundQuestObject = new QuestObject(foundQuest);
            ActiveQuests.Add(foundQuestObject);
            QuestGoToStep(foundQuestObject.Data.ID, toStep);
        }
    }

    public void QuestGoToStep(int id, int toStep)
    {
        QuestObject foundQuestObject = Context.GetActiveQuest(id);
        if (foundQuestObject == null)
        {
            Debug.LogWarning("Quest id " + id + " is not already an active quest, adding now.");
            AddQuestToActiveQuests(id, toStep);
            return;
        }
        else
        {
            if (toStep < foundQuestObject.CurrentStep)
            {
                Debug.LogWarning("Quest id " + id + " is already at a higher step than the one asked for. (Current: " + foundQuestObject.CurrentStep + "; Asked for: " + toStep + ")");
            }
            else if (toStep == foundQuestObject.CurrentStep)
            {
                Debug.LogWarning("Quest id " + id + " is already at that step " + toStep + ".");
            }
            else
            {
                foundQuestObject.CurrentStep = toStep;
            }       
        }
    }

    #region Savedata
    public void LoadStoryProfile(StorySaveProfile profile)
    {
        ActiveQuests.Clear();
        for (int i = 0; i < profile.ActiveQuestIDs.Count; i++)
        {
            AddQuestToActiveQuests(profile.ActiveQuestIDs[i], profile.ActiveQuestsCurrentStepIndex[i]);
        }
    }
    #endregion
}
