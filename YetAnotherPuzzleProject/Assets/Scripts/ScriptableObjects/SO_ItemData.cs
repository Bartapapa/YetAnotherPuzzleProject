using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Item/ItemData", fileName = "ID_#_ItemName")]
public class SO_ItemData : ScriptableObject
{
    [Header("ID")]
    [SerializeField] private int _id = -1;

    [Header("Throw")]
    public float ThrowForce = 1f;
    public Vector3 SpinForce = new Vector3(0f, 0f, 0.2f);
    public float OnContactAffectRange = .6f;
    public bool InteractsOnThrow = false;
    public bool StunsOnThrow = false;

    public int ID { get { return _id; } }
}
