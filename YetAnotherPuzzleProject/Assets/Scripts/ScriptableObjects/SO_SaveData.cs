using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class StorySaveProfile
{
    private StoryContext _context = null;
    public List<int> ActiveQuestIDs = new List<int>();
    public List<int> ActiveQuestsCurrentStepIndex = new List<int>();

    private List<int> GetActiveQuestIDs()
    {
        if (_context == null)
        {
            Debug.LogWarning("No saved story context found! Returning null.");
            return null;
        }
        List<int> activeQuestIDs = new List<int>();

        foreach(QuestObject quest in _context.ActiveQuests)
        {
            activeQuestIDs.Add(quest.Data.ID);
        }

        return activeQuestIDs;
    }

    private int GetStepFromActiveQuest(int questID)
    {
        if (_context == null)
        {
            Debug.LogWarning("No saved story context found! Returning -99.");
            return -99;
        }

        int currentStep = -1;
        foreach(QuestObject quest in _context.ActiveQuests)
        {
            if (quest.Data.ID == questID)
            {
                currentStep = quest.CurrentStep;
            }
        }
        return currentStep;
    }

    public void SaveStoryData()
    {
        Sc_StoryManager storyManager = Sc_StoryManager.instance;
        if (!storyManager)
        {
            Debug.LogWarning("No story manager found! Can't save story context.");
            return;
        }
        else
        {
            _context = storyManager.Context;
            ActiveQuestIDs = GetActiveQuestIDs();
            foreach(QuestObject quest in _context.ActiveQuests)
            {
                ActiveQuestsCurrentStepIndex.Add(GetStepFromActiveQuest(quest.Data.ID));
            }
        }
    }
}

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
        //Sc_Inventory playerInventory = playerObject.PlayerCharacter.Inventory;
        //for (int i = 0; i < PlayerCharacterInventoryIDs.Length; i++)
        //{
        //    if (playerInventory._items[i] != null)
        //    {
        //        PlayerCharacterInventoryIDs[i] = playerInventory._items[i]._itemData.ID;
        //    }
        //    else
        //    {
        //        PlayerCharacterInventoryIDs[i] = -1;
        //    }
        //}
        //if (playerInventory.CurrentlyHeldItem != null)
        //{
        //    CurrentHeldItemIndex = playerInventory.GetInventoryIndexOfCurrentlyHeldItem();
        //}
        //else
        //{
        //    CurrentHeldItemIndex = -1;
        //}
    }
}

[System.Serializable]
public class LevelSaveProfile
{
    public Loader.Scene Level = Loader.Scene.Managers;
    public List<bool> VasesChecked = new List<bool>();
    public List<bool> PotsPlanted = new List<bool>();

    public LevelSaveProfile (Sc_Level level)
    {
        Level = level.CurrentScene;
    }

    public void SaveVases(List<Sc_Vase> vases)
    {
        VasesChecked.Clear();
        for (int i = 0; i < vases.Count; i++)
        {
            VasesChecked.Add(vases[i].HasBeenSearchedThrough);
        }
    }

    public void SaveSeedBowls(List<Sc_SpiritSeedBowl> bowls)
    {
        PotsPlanted.Clear();
        for (int i = 0; i < bowls.Count; i++)
        {
            PotsPlanted.Add(bowls[i].HasBeenPlanted);
        }
    }

    public void OverwriteSave(Sc_Level level)
    {
        VasesChecked.Clear();
        PotsPlanted.Clear();
    }
}

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Save/BlankSaveData", fileName = "BlankSaveData")]
public class SO_SaveData : ScriptableObject
{
    public List<PlayerCharacterSaveProfile> CharacterSaveProfiles = new List<PlayerCharacterSaveProfile>();
    public List<LevelSaveProfile> LevelSaveProfiles = new List<LevelSaveProfile>();
    public StorySaveProfile StorySaveProfile = new StorySaveProfile();

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
    #region Story save
    public void CreateStorySaveProfile()
    {
        if (StorySaveProfile == null)
        {
            StorySaveProfile = new StorySaveProfile();
        }
        StorySaveProfile.SaveStoryData();
    }

    public void SaveStorySaveProfile()
    {
        if (StorySaveProfile == null)
        {
            CreateStorySaveProfile();
        }
        StorySaveProfile.SaveStoryData();
    }

    public void ClearStorySaveProfile()
    {
        StorySaveProfile = null;
    }
    #endregion

}
