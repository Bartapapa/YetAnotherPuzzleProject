using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSaveProfile
{
    public PlayerInput Player;
    public int SkinIndex = -1;
    public List<int> InventoryItemIDs = new List<int>();
    public int CurrentHeldItemID = -1;

    public PlayerSaveProfile (PlayerInput player)
    {
        Sc_Player playerObject = Sc_PlayerManager.instance.GetPlayerFromPInput(player);
        if (playerObject.PlayerCharacter == null) return;

        Player = player;
        SkinIndex = playerObject.PlayerCharacter.SkinIndex;
        Sc_Inventory inventory = playerObject.PlayerCharacter.Inventory;

        for (int i = 0; i < inventory._items.Count; i++)
        {
            InventoryItemIDs.Add(inventory._items[i]._itemData.ID);
        }
        if (inventory._currentlyHeldItem != null)
        {
            CurrentHeldItemID = inventory._currentlyHeldItem._itemData.ID;
        }
    }
}

public class SO_SaveData : MonoBehaviour
{
    public List<PlayerSaveProfile> SaveProfiles = new List<PlayerSaveProfile>();

    public void CreateSaveProfiles(List<PlayerInput> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            PlayerSaveProfile newSaveProfile = new PlayerSaveProfile(players[i]);

            //Verification if player has been saved.
            if (newSaveProfile.Player != null)
            {
                SaveProfiles.Add(newSaveProfile);
            }
        }
    }

    public void ClearSaveProfiles()
    {
        SaveProfiles.Clear();
    }
}
