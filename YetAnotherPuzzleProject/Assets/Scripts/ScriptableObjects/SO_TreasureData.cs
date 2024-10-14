using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Item/TreasureData", fileName = "TD_TreasureName")]
public class SO_TreasureData : ScriptableObject
{
    public enum CharacterPanoply
    {
        None,
        BigHatJoel,
        Wanderer,
        FishmongerReice,
        ExplorerJada,
    }

    [Header("ID")]
    [SerializeField] private int _selfId = -1;
    [SerializeField] private CharacterPanoply _panoply = CharacterPanoply.None;

    public int ID
    {
        get 
        {
            int id = -1;
            id += ((int)_panoply * 10);
            id += _selfId;
            return id;
        } 
    }

    public bool PlayerAlreadyHasTreasure
    {
        get
        {
            //Check save file to see if player already has treasure.
            Debug.LogWarning("PlayerAlreadyHasTreasure not implemented.");
            return false;
        }
    }
}
