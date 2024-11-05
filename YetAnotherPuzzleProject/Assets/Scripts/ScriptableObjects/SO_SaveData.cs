using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerCharacterSaveProfile
{
    public Sc_Player Player;
    public int[] PlayerCharacterInventoryIDs = new int[3] { -1, -1, -1 };
    public int CurrentHeldItemIndex = -1;

    public PlayerCharacterSaveProfile (PlayerInput player)
    {
        Sc_Player playerObject = Sc_GameManager.instance.PlayerManager.GetPlayerFromPInput(player);
        if (playerObject.PlayerCharacter == null) return;

        Player = playerObject;
        Sc_Inventory playerInventory = playerObject.PlayerCharacter.Inventory;
        for (int i = 0; i < PlayerCharacterInventoryIDs.Length; i++)
        {
            if (playerInventory._items[i] != null)
            {
                PlayerCharacterInventoryIDs[i] = playerInventory._items[i]._itemData.ID;
            }
            else
            {
                PlayerCharacterInventoryIDs[i] = -1;
            }
        }
        if (playerInventory.CurrentlyHeldItem != null)
        {
            CurrentHeldItemIndex = playerInventory.GetInventoryIndexOfCurrentlyHeldItem();
        }
        else
        {
            CurrentHeldItemIndex = -1;
        }
    }
}

[System.Serializable]
public class LevelSaveProfile
{
    public Loader.Scene Level = Loader.Scene.Managers;
    public List<bool> VasesChecked = new List<bool>();

    public LevelSaveProfile (Sc_Level level)
    {
        Level = level.CurrentScene;
        for (int i = 0; i < level.TreasureVases.Count; i++)
        {
            VasesChecked.Add(level.TreasureVases[i].HasBeenSearchedThrough);
        }
    }

    public void OverwriteSave(Sc_Level level)
    {
        VasesChecked.Clear();
        for (int i = 0; i < level.TreasureVases.Count; i++)
        {
            VasesChecked.Add(level.TreasureVases[i].HasBeenSearchedThrough);
        }
    }
}

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Save/BlankSaveData", fileName = "BlankSaveData")]
public class SO_SaveData : ScriptableObject
{
    public List<PlayerCharacterSaveProfile> CharacterSaveProfiles = new List<PlayerCharacterSaveProfile>();
    public List<LevelSaveProfile> LevelSaveProfiles = new List<LevelSaveProfile>();

    #region Character save
    public void CreateCharacterSaveProfiles(List<PlayerInput> players)
    {
        ClearCharacterSaveProfiles();

        for (int i = 0; i < players.Count; i++)
        {
            PlayerCharacterSaveProfile newSaveProfile = new PlayerCharacterSaveProfile(players[i]);

            CharacterSaveProfiles.Add(newSaveProfile);
        }
    }

    public void ClearCharacterSaveProfiles()
    {
        CharacterSaveProfiles.Clear();
    }
    #endregion
    #region Level save
    public void CreateLevelSaveProfile(Sc_Level level)
    {
        LevelSaveProfile saveProfile = null;
        foreach (LevelSaveProfile lsp in LevelSaveProfiles)
        {
            if (lsp.Level == level.CurrentScene)
            {
                saveProfile = lsp;
                break;
            }
        }
        if (saveProfile == null)
        {
            saveProfile = new LevelSaveProfile(level);
            LevelSaveProfiles.Add(saveProfile);
        }
        else
        {
            saveProfile.OverwriteSave(level);
        }
    }

    public LevelSaveProfile GetLevelSaveProfileForLevel(Sc_Level level)
    {
        LevelSaveProfile lsp = null;
        foreach(LevelSaveProfile levelsave in LevelSaveProfiles)
        {
            if (levelsave.Level == level.CurrentScene)
            {
                lsp = levelsave;
            }
        }
        return lsp;
    }
    #endregion

}
