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

    [Header("TREASURE PARTICLES")]
    public ParticleSystem CommonParticles;
    public ParticleSystem RareParticles;
    public ParticleSystem VeryRareParticles;

    [Header("TREASURE SOUNDS")]
    public AudioClip CommonTreasure;
    public AudioClip RareTreasure;
    public AudioClip VeryRareTreasure;

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

    public Sc_Item FindRandomNewCommonTreasure()
    {
        Sc_Item foundTreasure = GetNewRandomCommonTreasure();
        FindTreasure(foundTreasure);
        return foundTreasure;
    }

    public Sc_Item FindRandomNewRareTreasure()
    {
        Sc_Item foundTreasure = GetNewRandomRareTreasure();
        if (!foundTreasure) foundTreasure = GetNewRandomCommonTreasure();
        FindTreasure(foundTreasure);
        return foundTreasure;
    }

    public Sc_Item FindRandomNewVeryRareTreasure()
    {
        Sc_Item foundTreasure = GetNewRandomVeryRareTreasure();
        if (!foundTreasure) foundTreasure = GetNewRandomRareTreasure();
        if (!foundTreasure) foundTreasure = GetNewRandomCommonTreasure();
        FindTreasure(foundTreasure);
        return foundTreasure;
    }

    public void FindTreasure(Sc_Item treasure)
    {
        if (_foundTreasures.Contains(treasure)) return;
        if (treasure == null) return;

        _foundTreasures.Add(treasure);
        switch (treasure._itemData.Rarity)
        {
            case TreasureRarity.Common:
                Sc_GameManager.instance.SoundManager.PlaySFX(Sc_GameManager.instance.SoundManager.UISource, CommonTreasure);
                break;
            case TreasureRarity.Rare:
                Sc_GameManager.instance.SoundManager.PlaySFX(Sc_GameManager.instance.SoundManager.UISource, RareTreasure);
                break;
            case TreasureRarity.VeryRare:
                Sc_GameManager.instance.SoundManager.PlaySFX(Sc_GameManager.instance.SoundManager.UISource, VeryRareTreasure);
                break;
            default:
                break;
        }

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

    private Sc_Item GetNewRandomCommonTreasure()
    {
        List<Sc_Item> commonTreasures = new List<Sc_Item>();
        for (int i = 0; i < _potentialTreasures.Count; i++)
        {
            if (_potentialTreasures[i]._itemData.Rarity == TreasureRarity.Common)
            {
                commonTreasures.Add(_potentialTreasures[i]);
            }
        }

        int totalWeight = 0;
        for (int j = 0; j < commonTreasures.Count; j++)
        {
            totalWeight += WeightFromRarity(commonTreasures[j]._itemData.Rarity);
        }

        int roll = UnityEngine.Random.Range(0, totalWeight);
        Sc_Item chosenTreasure = null;
        for (int k = 0; k < commonTreasures.Count; k++)
        {
            roll -= WeightFromRarity(commonTreasures[k]._itemData.Rarity);

            if (roll < 0)
            {
                chosenTreasure = commonTreasures[k];
                break;
            }
        }

        return chosenTreasure;
    }
    private Sc_Item GetNewRandomRareTreasure()
    {
        List<Sc_Item> rareTreasures = new List<Sc_Item>();
        for (int i = 0; i < _potentialTreasures.Count; i++)
        {
            if (_potentialTreasures[i]._itemData.Rarity == TreasureRarity.Rare)
            {
                rareTreasures.Add(_potentialTreasures[i]);
            }
        }

        int totalWeight = 0;
        for (int j = 0; j < rareTreasures.Count; j++)
        {
            totalWeight += WeightFromRarity(rareTreasures[j]._itemData.Rarity);
        }

        int roll = UnityEngine.Random.Range(0, totalWeight);
        Sc_Item chosenTreasure = null;
        for (int k = 0; k < rareTreasures.Count; k++)
        {
            roll -= WeightFromRarity(rareTreasures[k]._itemData.Rarity);

            if (roll < 0)
            {
                chosenTreasure = rareTreasures[k];
                break;
            }
        }

        return chosenTreasure;
    }
    private Sc_Item GetNewRandomVeryRareTreasure()
    {
        List<Sc_Item> veryRareTreasures = new List<Sc_Item>();
        for (int i = 0; i < _potentialTreasures.Count; i++)
        {
            if (_potentialTreasures[i]._itemData.Rarity == TreasureRarity.VeryRare)
            {
                veryRareTreasures.Add(_potentialTreasures[i]);
            }
        }

        int totalWeight = 0;
        for (int j = 0; j < veryRareTreasures.Count; j++)
        {
            totalWeight += WeightFromRarity(veryRareTreasures[j]._itemData.Rarity);
        }

        int roll = UnityEngine.Random.Range(0, totalWeight);
        Sc_Item chosenTreasure = null;
        for (int k = 0; k < veryRareTreasures.Count; k++)
        {
            roll -= WeightFromRarity(veryRareTreasures[k]._itemData.Rarity);

            if (roll < 0)
            {
                chosenTreasure = veryRareTreasures[k];
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
            case TreasureRarity.Rare:
                return 5;
            case TreasureRarity.VeryRare:
                return 1;
            default:
                return 20;
        }
    }
}
