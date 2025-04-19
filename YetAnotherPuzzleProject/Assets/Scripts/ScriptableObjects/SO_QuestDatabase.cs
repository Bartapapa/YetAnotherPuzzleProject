using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Quest/BaseQuestDatabase", fileName = "Quest_Database")]
public class SO_QuestDatabase : ScriptableObject
{
    public List<SO_QuestData> Quests = new List<SO_QuestData>();
}
