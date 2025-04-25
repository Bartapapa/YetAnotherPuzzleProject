using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public enum ConditionType
{
    None,
    QuestStep,
    InteractorHasFinishedQuest,
    InteractorHasItem,
}

public enum Comparator
{
    EqualTo,
    NotEqualTo,
    LessOrEqualThan,
    LessThan,
    GreaterOrEqualThan,
    GreaterThan,
}

public enum ConditionItems
{
    None,
    Lantern,
    Rocks,
    Seeds,
    Pickax,
    Effigy,
    Roboarm,
}

[System.Serializable]
public class Condition
{
    public ConditionType Type = ConditionType.None;

    [Header("QUEST STEP")]
    public int Step_QuestID = -1;
    public Comparator Comparison = Comparator.EqualTo;
    public int Step_QuestStep = -1;

    [Header("FINISHED QUEST")]
    public int Finished_QuestID = -1;

    [Header("HAS ITEM")]
    public ConditionItems Item = ConditionItems.None;

    public virtual bool CheckPlayerCondition(Sc_Character_Player playerCharacter)
    {
        if (playerCharacter == null) return false;

        switch (Type)
        {
            case ConditionType.None:
                return true;
            case ConditionType.QuestStep:
                if (Sc_StoryManager.instance == null) return false;
                else
                {
                    StoryContext context = Sc_StoryManager.instance.Context;
                    int questStep = context.GetQuestCurrentStep(Step_QuestID);
                    switch (Comparison)
                    {
                        case Comparator.EqualTo:
                            return questStep == Step_QuestStep;
                        case Comparator.NotEqualTo:
                            return questStep != Step_QuestStep;
                        case Comparator.LessOrEqualThan:
                            return questStep <= Step_QuestStep;
                        case Comparator.LessThan:
                            return questStep < Step_QuestStep;
                        case Comparator.GreaterOrEqualThan:
                            return questStep >= Step_QuestStep;
                        case Comparator.GreaterThan:
                            return questStep > Step_QuestStep;
                        default:
                            return false;
                    }
                }
            case ConditionType.InteractorHasFinishedQuest:
                if (Sc_StoryManager.instance == null) return false;
                else
                {
                    StoryContext context = Sc_StoryManager.instance.Context;
                    QuestObject quest = context.GetActiveQuest(Finished_QuestID);
                    if (quest != null)
                    {
                        return quest.IsQuestFinished;
                    }
                    else
                    {
                        return false;
                    }
                }
            case ConditionType.InteractorHasItem:
                Sc_Inventory_New inventory = playerCharacter.Inventory;
                switch (Item)
                {
                    case ConditionItems.None:
                        return true;
                    case ConditionItems.Lantern:
                        return inventory.HasUnlockedLantern;
                    case ConditionItems.Rocks:
                        return inventory.HasUnlockedRocks;
                    case ConditionItems.Seeds:
                        return inventory.HasUnlockedSeeds;
                    case ConditionItems.Pickax:
                        return inventory.HasUnlockedPickax;
                    //case ConditionItems.Effigy:
                    //return inventory.HasUnlockedLantern;
                    case ConditionItems.Roboarm:
                        return inventory.HasUnlockedRoboarm;
                    default:
                        return false;
                }
            default:
                return true;
        }
    }

    public virtual bool CheckPlayerItemCondition(int itemID)
    {
        return true;

        //List<int> passingItemIDs = new List<int>();

        //switch (Type)
        //{
        //    case ConditionType.And:
        //        foreach (Condition cond in _andConditions)
        //        {
        //            switch (cond.Type)
        //            {
        //                case ConditionType.InteractorHasItemEquipped:
        //                    passingItemIDs.Add(cond._itemIdToEquip);
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        break;
        //    case ConditionType.Or:
        //        foreach (Condition cond in _orConditions)
        //        { 
        //            switch (cond.Type)
        //            {
        //                case ConditionType.InteractorHasItemEquipped:
        //                    passingItemIDs.Add(cond._itemIdToEquip);
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        break;
        //    case ConditionType.InteractorHasItemEquipped:
        //        passingItemIDs.Add(_itemIdToEquip);
        //        break;
        //    default:
        //        break; 
        //}

        //return passingItemIDs.Contains(itemID);
    }
}
