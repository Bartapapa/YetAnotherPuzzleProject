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
        if (playerInventory._currentlyHeldItem != null)
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
    public Sc_Level Level;
}

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Save/BlankSaveData", fileName = "BlankSaveData")]
public class SO_SaveData : ScriptableObject
{
    public List<PlayerCharacterSaveProfile> CharacterSaveProfiles = new List<PlayerCharacterSaveProfile>();

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
}
