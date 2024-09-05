using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Item/ItemData", fileName = "ID_#_ItemName")]
public class SO_ItemData : ScriptableObject
{
    [Header("ID")]
    [SerializeField] private int _id = -1;

    public int ID { get { return _id; } }
}
