using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None,
    Item,
    Treasure,
}

public enum TreasureRarity
{
    None,
    Common,
    Rare,
    VeryRare
}

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Item/ItemData", fileName = "ID_#_ItemName")]
public class SO_ItemData : ScriptableObject
{
    [Header("ID")]
    [SerializeField] private int _id = -1;
    [SerializeField] private string _name = "DefaultName";
    [SerializeField] private ItemType _itemType = ItemType.None;

    [Header("Treasure")]
    public TreasureRarity Rarity = TreasureRarity.Common;

    [Header("Throw")]
    public float ThrowForce = 1f;
    public Vector3 SpinForce = new Vector3(0f, 0f, 0.2f);
    public float OnContactAffectRange = .6f;
    public bool InteractsOnThrow = false;
    public bool StunsOnThrow = false;

    public int ID { get { return _id; } }
    public string Name { get { return _name; } }

    public ItemType Type { get { return _itemType; } }
}
