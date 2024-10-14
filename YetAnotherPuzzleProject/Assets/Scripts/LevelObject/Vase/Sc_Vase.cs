using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reward
{
    public RewardType type = RewardType.None;
    public List<Sc_Item> itemRewards = new List<Sc_Item>();
    public int weight = 10;
}

public enum RewardType
{
    None,
    Treasure,
    Item,
    Length,
}

public class Sc_Vase : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Interactible _interactible;
    public Transform _itemSpawn;
    public ParticleSystem _dustPoof;

    [Header("PARAMETERS")]
    public List<Reward> _rewards = new List<Reward>();

    [ReadOnly][SerializeField] private int _totalWeight = -1;
    private int TotalWeight
    {
        get
        {
            if (_totalWeight == -1)
            {
                CalculateTotalWeight();
            }
            return _totalWeight;
        }
    }

    public void OnInteract(Sc_Character interactor)
    {
        CheckVase();
        _interactible.CanBeInteractedWith = false;
    }

    private void CheckVase()
    {
        Reward foundReward = GetReward();

        if (foundReward == null)
        {
            NoTreasure();
            return;
        }

        switch (foundReward.type)
        {
            case RewardType.None:
                NoTreasure();
                break;
            case RewardType.Treasure:
                break;
            case RewardType.Item:
                int randomItem = UnityEngine.Random.Range(0, foundReward.itemRewards.Count);
                Sc_Item foundItem = foundReward.itemRewards[randomItem];
                Sc_Item newItem = Instantiate<Sc_Item>(foundItem, _itemSpawn.position, _itemSpawn.rotation, _itemSpawn);

                //Get interactor, and equip item if possible.
                break;
            default:
                NoTreasure();
                break;
        }
    }

    private Reward GetReward()
    {
        int roll = UnityEngine.Random.Range(0, TotalWeight);
        Reward chosenReward = null;

        for (int i = 0; i < _rewards.Count; i++)
        {
            roll -= _rewards[i].weight;

            if (roll < 0)
            {
                chosenReward = _rewards[i];
            }
        }

        return chosenReward;
    }

    private void NoTreasure()
    {
        _dustPoof.Play();
    }

    private void CalculateTotalWeight()
    {
        _totalWeight = 0;
        for (int i = 0; i < _rewards.Count; i++)
        {
            _totalWeight += _rewards[i].weight;
        }
    }


}
