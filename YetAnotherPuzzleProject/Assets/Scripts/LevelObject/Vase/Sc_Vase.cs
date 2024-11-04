using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reward
{
    public List<Sc_Item> itemRewards = new List<Sc_Item>();
    public int weight = 10;
}

public class Sc_Vase : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Interactible _interactible;
    public Transform _itemSpawn;
    public ParticleSystem _dustPoof;

    [Header("PARAMETERS")]
    public List<Reward> _rewards = new List<Reward>();
    public bool IsReusable = false;
    [ReadOnly] public bool HasBeenSearchedThrough = false;

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
        CheckVase(interactor);
        if (!IsReusable)
        {
            _interactible.CanBeInteractedWith = false;
            HasBeenSearchedThrough = true;
            Sc_GameManager.instance.SaveLevelData(Sc_GameManager.instance.CurrentLevel);
        }
    }

    public void CheckVase(Sc_Character interactor)
    {
        Reward foundReward = GetReward();

        if (foundReward == null)
        {
            NoTreasure();
            return;
        }

        int randomItem = UnityEngine.Random.Range(0, foundReward.itemRewards.Count);
        Sc_Item foundItem = foundReward.itemRewards[randomItem];

        switch (foundItem._itemData.Type)
        {
            case ItemType.None:
                break;
            case ItemType.Item:
                break;
            case ItemType.Treasure:
                if (Sc_TreasureManager.instance == null)
                {
                    NoTreasure();
                    return;
                }

                if (foundItem._itemData.ID == 190)
                {
                    //Found a random treasure.
                    foundItem = Sc_TreasureManager.instance.FindRandomNewTreasure();
                }
                else if (foundItem._itemData.ID == 191)
                {
                    //Found a random common treasure.
                    foundItem = Sc_TreasureManager.instance.FindRandomNewCommonTreasure();
                }
                else if (foundItem._itemData.ID == 192)
                {
                    //Found a random rare treasure.
                    foundItem = Sc_TreasureManager.instance.FindRandomNewRareTreasure();
                }
                else if (foundItem._itemData.ID == 193)
                {
                    //Found a random very rare treasure.
                    foundItem = Sc_TreasureManager.instance.FindRandomNewVeryRareTreasure();
                    Debug.Log(foundItem);
                }

                if (foundItem == null)
                {
                    NoTreasure();
                    return;
                }
                break;
        }

        Sc_Item newItem = Instantiate<Sc_Item>(foundItem, _itemSpawn.position, _itemSpawn.rotation, _itemSpawn);

        switch (newItem._itemData.Type)
        {
            case ItemType.None:
                break;
            case ItemType.Item:
                if (interactor != null)
                {
                    Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
                    if (player)
                    {
                        if (!player.Inventory.IsCurrentlyHoldingItem)
                        {
                            newItem.OnInteractedWith(interactor);
                        }
                    }
                }
                break;
            case ItemType.Treasure:
                if (interactor != null)
                {
                    newItem.AcquireTreasure(interactor.transform);
                }
                else
                {
                    newItem.AcquireTreasure(_itemSpawn);
                }
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
                break;
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
