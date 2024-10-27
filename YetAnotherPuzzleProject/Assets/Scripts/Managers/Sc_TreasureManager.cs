using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sc_TreasureManager : MonoBehaviour
{
    public static Sc_TreasureManager instance { get; private set; }

    [Header("ITEM DATABASE")]
    public SO_ItemDatabase ItemDatabase;

    [Header("TREASURES")]
    [SerializeField] private List<Sc_Item> _potentialTreasures = new List<Sc_Item>();
    [ReadOnly][SerializeField] private List<Sc_Item> _foundTreasures = new List<Sc_Item>();
    private int _totalWeight = -1;
    private bool _dirtyWeight = true;

    private int TotalWeight
    {
        get
        {
            if (_dirtyWeight)
            {
                CalculateTotalWeight();
            }
            return _totalWeight;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public Sc_Item FindRandomNewTreasure()
    {
        Sc_Item foundTreasure = GetNewRandomTreasure();
        FindTreasure(foundTreasure);
        return foundTreasure;
    }

    public void FindTreasure(Sc_Item treasure)
    {
        if (_foundTreasures.Contains(treasure)) return;

        _foundTreasures.Add(treasure);

        RemovePotentialTreasure(treasure);
    }

    private void RemovePotentialTreasure(Sc_Item treasureToRemove)
    {
        if (_potentialTreasures.Remove(treasureToRemove))
        {
            _dirtyWeight = true;
        }
    }

    private Sc_Item GetNewRandomTreasure()
    {
        int roll = UnityEngine.Random.Range(0, TotalWeight);
        Sc_Item chosenTreasure = null;

        for (int i = 0; i < _potentialTreasures.Count; i++)
        {
            roll -= WeightFromRarity(_potentialTreasures[i]._itemData.Rarity);

            if (roll < 0)
            {
                chosenTreasure = _potentialTreasures[i];
                break;
            }
        }

        return chosenTreasure;
    }

    private void CalculateTotalWeight()
    {
        _dirtyWeight = false;
        _totalWeight = 0;
        for (int i = 0; i < _potentialTreasures.Count; i++)
        {
            _totalWeight += WeightFromRarity(_potentialTreasures[i]._itemData.Rarity);
        }
    }

    private int WeightFromRarity(TreasureRarity rarity)
    {
        switch (rarity)
        {
            case TreasureRarity.None:
                return 0;
            case TreasureRarity.Common:
                return 20;
            case TreasureRarity.Exotic:
                return 5;
            case TreasureRarity.Legendary:
                return 1;
            default:
                return 20;
        }
    }
}
