using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public enum ConditionType
{
    None,
    And,
    Or,
    InteractorHasItemEquipped,
    InteractorHasRoboArm,
}

[System.Serializable]
public class Condition
{
    public ConditionType Type = ConditionType.None;
    [Header("And")]
    public List<Condition> _andConditions = new List<Condition>();
    [Header("Or")]
    public List<Condition> _orConditions = new List<Condition>();
    [Header("InteractorHasItemEquipped")]
    public int _itemIdToEquip = -1;
    [Header("InteractorHasRoboArm")]
    public bool _checkIfNoRoboArm = false;

    public virtual bool CheckPlayerCondition(Sc_Character_Player playerCharacter)
    {
        if (playerCharacter == null) return false;

        switch (Type)
        {
            case ConditionType.None:
                return true;
            case ConditionType.And:
                return AndConditionCheck(playerCharacter);
            case ConditionType.Or:
                return OrConditionCheck(playerCharacter);
            case ConditionType.InteractorHasItemEquipped:
                if (playerCharacter.Inventory == null) return false;
                if (playerCharacter.Inventory.CurrentlyHeldItem == null) return false;
                int currentHeldItemKey = playerCharacter.Inventory.CurrentlyHeldItem._itemData.ID;
                return currentHeldItemKey == _itemIdToEquip;
            case ConditionType.InteractorHasRoboArm:
                return _checkIfNoRoboArm ? !playerCharacter.HasRoboArm : playerCharacter.HasRoboArm;
        }

        return false;
    }

    public virtual bool CheckPlayerItemCondition(int itemID)
    {
        List<int> passingItemIDs = new List<int>();

        switch (Type)
        {
            case ConditionType.And:
                foreach (Condition cond in _andConditions)
                {
                    switch (cond.Type)
                    {
                        case ConditionType.InteractorHasItemEquipped:
                            passingItemIDs.Add(cond._itemIdToEquip);
                            break;
                    }
                }
                break;
            case ConditionType.Or:
                foreach (Condition cond in _andConditions)
                {
                    switch (cond.Type)
                    {
                        case ConditionType.InteractorHasItemEquipped:
                            passingItemIDs.Add(cond._itemIdToEquip);
                            break;
                    }
                }
                break;
            case ConditionType.InteractorHasItemEquipped:
                passingItemIDs.Add(_itemIdToEquip);
                break;
        }

        return passingItemIDs.Contains(itemID);
    }

    private bool AndConditionCheck(Sc_Character_Player playerCharacter)
    {
        bool andCheck = true;
        foreach(Condition cond in _andConditions)
        {
            if (cond.CheckPlayerCondition(playerCharacter) == false)
            {
                andCheck = false;
                break;
            }
        }
        return andCheck;
    }

    private bool OrConditionCheck(Sc_Character_Player playerCharacter)
    {
        bool orCheck = false;
        foreach (Condition cond in _orConditions)
        {
            if (cond.CheckPlayerCondition(playerCharacter) == true)
            {
                orCheck = true;
                break;
            }
        }
        return orCheck;
    }
}
