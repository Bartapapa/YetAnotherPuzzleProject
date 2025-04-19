using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestStep
{
    [TextArea(3, 5)] public string StepDescription = "";
}

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Quest/BaseQuest", fileName = "Quest_#ID_Name")]
public class SO_QuestData : ScriptableObject
{
    public int _id = -1;
    public int ID { get { return _id; } }
    public List<QuestStep> Steps = new List<QuestStep>();
}
