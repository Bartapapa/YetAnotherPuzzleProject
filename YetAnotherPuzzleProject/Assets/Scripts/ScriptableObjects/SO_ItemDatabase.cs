using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Database/ItemDatabase", fileName = "ItemDatabase")]
public class SO_ItemDatabase : ScriptableObject
{
    public List<Sc_Item> AllItems = new List<Sc_Item>();

    public Sc_Item GetItemFromID(int ID)
    {
        foreach(Sc_Item item in AllItems)
        {
            if (item._itemData.ID == ID)
            {
                return item;
            }
        }

        return null;
    }
}
