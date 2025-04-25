using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public enum EquippedItem
{
    None,
    Lantern,
    Rocks,
    Seeds,
    Pickax,
    Hauled,
    Length,
}
public class Sc_Inventory_New : MonoBehaviour
{
    [Header("INVENTORY OBJECT REFS")]
    public Sc_Character_Player Character;
    public Transform ItemHoldAnchor;
    public Transform ItemThrowAnchor;

    [Header("ITEM REFS")]
    private EquippedItem _internalEquippedItem = EquippedItem.None;
    public EquippedItem CurrentItem { get { return _internalEquippedItem; } }
    [SerializeField] private Sc_Item _lanternItem;
    [SerializeField] private List<Sc_Item> _rockItems = new List<Sc_Item>();
    public Transform RockParent;
    [SerializeField] private Sc_Item _seedItem;
    [SerializeField] private Sc_Item _pickaxItem;
    [SerializeField] private Sc_Item _effigyItem;

    private Sc_Item _currentRock;
    private Sc_Item CurrentRock { get { return SetCurrentRock(); } }

    [Header("THROWING")]
    public LayerMask ThrownObjectCollisionLayers;
    public LineRenderer TrajectoryLine;
    public GameObject ImpactSphere;
    private Vector3 _actualAimDir;
    public bool CanAim { get { return Character.Controller.IsClimbing || Character.Controller.IsAnchoring || Character.Controller.IsAnchoredToValve || !Character.Controller.IsGrounded ? false : true; } }
    [ReadOnly] public bool IsAiming = false;

    public bool HasUnlockedLantern
    { get 
        {   
            if (Sc_StoryManager.instance == null)
            {
                return false;
            }
            else
            {
                StoryContext context = Sc_StoryManager.instance.Context;
                QuestObject quest = context.GetActiveQuest(1);
                if (quest == null)
                {
                    return false;
                }
                else
                {
                    return quest.IsQuestFinished;
                }
            }
        }
    }
    public bool HasUnlockedRocks
    {
        get
        {
            if (Sc_StoryManager.instance == null)
            {
                return false;
            }
            else
            {
                StoryContext context = Sc_StoryManager.instance.Context;
                QuestObject quest = context.GetActiveQuest(2);
                if (quest == null)
                {
                    return false;
                }
                else
                {
                    return quest.IsQuestFinished;
                }
            }
        }
    }
    public bool HasUnlockedRoboarm
    {
        get
        {
            if (Sc_StoryManager.instance == null)
            {
                return false;
            }
            else
            {
                StoryContext context = Sc_StoryManager.instance.Context;
                QuestObject quest = context.GetActiveQuest(3);
                if (quest == null)
                {
                    return false;
                }
                else
                {
                    return quest.IsQuestFinished;
                }
            }
        }
    }
    public bool HasUnlockedPickax
    {
        get
        {
            if (Sc_StoryManager.instance == null)
            {
                return false;
            }
            else
            {
                StoryContext context = Sc_StoryManager.instance.Context;
                QuestObject quest = context.GetActiveQuest(4);
                if (quest == null)
                {
                    return false;
                }
                else
                {
                    return quest.IsQuestFinished;
                }
            }
        }
    }
    public bool HasUnlockedSeeds
    {
        get
        {
            if (Sc_StoryManager.instance == null)
            {
                return false;
            }
            else
            {
                StoryContext context = Sc_StoryManager.instance.Context;
                QuestObject quest = context.GetActiveQuest(5);
                if (quest == null)
                {
                    return false;
                }
                else
                {
                    return quest.IsQuestFinished;
                }
            }
        }
    }

    public delegate void DefaultEvent();
    public DefaultEvent StartedAiming;
    public delegate void ItemEvent(Sc_Item item);
    public ItemEvent TreasureFound;
    public ItemEvent ItemThrown;
    public ItemEvent ItemDropped;
    public ItemEvent ItemPickedUp;
    public ItemEvent ItemFound;
    public delegate void EquipmentEvent(EquippedItem item);
    public EquipmentEvent UnlockedItem;
    public EquipmentEvent UnsheathedItem;

    private void Update()
    {
        HandleAiming();
    }

    #region GENERAL
    private void StowItem(EquippedItem itemToStow)
    {
        switch (_internalEquippedItem)
        {
            case EquippedItem.None:
                break;
            case EquippedItem.Lantern:
                StowLantern();
                break;
            case EquippedItem.Rocks:
                break;
            case EquippedItem.Seeds:
                break;
            case EquippedItem.Pickax:
                break;
            case EquippedItem.Hauled:
                break;
            default:
                break;
        }
        _internalEquippedItem = EquippedItem.None;
    }
    #endregion
    #region LANTERN
    public void UnlockLantern()
    {
        //Set new active quest, set step to completion.
        Sc_StoryManager storyManager = Sc_StoryManager.instance;
        if (storyManager == null) return;

        storyManager.AddQuestToActiveQuests(1, 1);
        UnlockedItem?.Invoke(EquippedItem.Lantern);
    }

    public void EquipLantern()
    {
        if (!HasUnlockedLantern) return;
        if (CurrentItem == EquippedItem.Lantern)
        {
            StowItem(CurrentItem);
            return;
        }
        if (CurrentItem != EquippedItem.Lantern && CurrentItem != EquippedItem.None)
        {
            StowItem(CurrentItem);
            UnsheatheLantern();
            return;
        }
        if (CurrentItem == EquippedItem.None)
        {
            UnsheatheLantern();
            return;
        }

    }

    private void StowLantern()
    {
        //hide lantern gameobject in hand.
        ShowLanternMesh(true);
        _lanternItem.gameObject.SetActive(false);
    }

    private void UnsheatheLantern()
    {
        _internalEquippedItem = EquippedItem.Lantern;

        ShowLanternMesh(true);
        _lanternItem.gameObject.SetActive(true);
        _lanternItem._interactible.CanBeInteractedWith = false;
        //show lantern gameobject in hand.
        //animator handler should change animations.
        //sound handler should play lantern unsheathing sound.
    }

    public void ShowLanternMesh(bool show)
    {
        if (CurrentItem != EquippedItem.Lantern) return;
        _lanternItem.Mesh.gameObject.SetActive(show);
        //hide the lantern mesh, but keep the light
        //for example when swinging pickax in the dark or throwing rock
    }

    #endregion
    #region ROCKS
    public void UnlockRocks()
    {

    }

    private Sc_Item SetCurrentRock()
    {
        Sc_Item chosenRock = null;
        if (_currentRock != null)
        {
            chosenRock = _currentRock;
        }
        else
        {
            foreach(Sc_Item potentialRock in _rockItems)
            {
                if (!potentialRock.IsBeingThrown
                    && !potentialRock.IsBeingDestroyed)
                {
                    chosenRock = potentialRock;
                    break;
                }
            }
        }
        return chosenRock;
    }
    public void ThrowRock()
    {
        SetRock(CurrentRock);
    }

    public void SetRock(Sc_Item item)
    {
        SetForThrow(item);
        item.ThrowItem(Character, _actualAimDir);
        _currentRock = null;
        ItemThrown?.Invoke(item);

        //CurrentItem = EquippedItem.None;

        //if (GetIndexFromItem(item) >= 0)
        //{

        //}
    }

    private void SetForThrow(Sc_Item item)
    {
        item.gameObject.SetActive(true);
        item._interactible.CanBeInteractedWith = false;
        item.IsEquipped = false;

        if (Sc_Level.instance != null)
        {
            item.transform.parent = Sc_Level.instance.transform;
        }
        else
        {
            item.transform.parent = null;
        }
        item.transform.position = ItemThrowAnchor.position;
        item.transform.rotation = ItemThrowAnchor.rotation;
    }

    public void AimThrow()
    {
        if (!HasUnlockedRocks) return;
        if (!CanAim) return;
        if (CurrentRock == null) return;
        if (CurrentItem != EquippedItem.None)
        {
            StowItem(CurrentItem);
        }

        StartedAiming?.Invoke();

        IsAiming = true;
        TrajectoryLine.enabled = true;
        Character.Controller.CanMove = false;
        CurrentRock.gameObject.SetActive(true);
        CurrentRock._interactible.CanBeInteractedWith = false;
    }

    public void StopAiming()
    {
        IsAiming = false;
        TrajectoryLine.enabled = false;
        ImpactSphere.SetActive(false);
        Character.Controller.CanMove = true;
    }

    #region AIMING
    private void HandleAiming()
    {
        if (!IsAiming) return;
        DrawAimingTrajectory(ItemThrowAnchor.position, ItemThrowAnchor.forward * CurrentRock._itemData.ThrowForce);
        //DrawTrajectory(_itemThrowPoint.position, _currentlyHeldItem._itemData.ThrowForce);
    }

    private void DrawAimingTrajectory(Vector3 initialPos, Vector3 initialVel)
    {
        Vector3 pos = initialPos;
        Vector3 vel = initialVel;
        Vector3 gravity = Physics.gravity;
        Vector3 aimDirection = initialVel;
        _actualAimDir = transform.forward;
        int contactIndex = -1;

        for (int i = 0; i < TrajectoryLine.positionCount; i++)
        {
            if (MakesContact(pos))
            {
                Vector3 contactPos = pos;
                if (CurrentRock._itemData.InteractsOnThrow)
                {
                    Sc_Interactible contactedInteractible = GetClosestContactedInteractible(pos, CurrentRock._itemData.OnContactAffectRange);
                    if (contactedInteractible != null)
                    {
                        contactPos = contactedInteractible.transform.position;
                    }
                }
                Vector3 posIgnoreY = new Vector3(ItemThrowAnchor.position.x, 0f, ItemThrowAnchor.position.z);
                Vector3 contactPosIgnoreY = new Vector3(contactPos.x, 0f, contactPos.z);
                Vector3 dir = (contactPosIgnoreY - posIgnoreY).normalized;
                //float angle = GetAngleToHitPoint(_itemThrowPoint.position, contactPos, initialVel);
                //_actualAimDir = Quaternion.AngleAxis(angle, Vector3.Cross(dir, Vector3.up)) * dir;
                _actualAimDir = dir;
                //Debug.Log(GetAngleToHitPoint(_itemThrowPoint.position, contactPos, initialVel));
                contactIndex = i;
                break;
            }

            vel = vel + gravity * Time.fixedDeltaTime;
            pos = pos + vel * Time.fixedDeltaTime;
        }

        pos = initialPos;
        vel = _actualAimDir * CurrentRock._itemData.ThrowForce;

        for (int i = 0; i < TrajectoryLine.positionCount; i++)
        {
            TrajectoryLine.SetPosition(i, pos);

            if (i >= contactIndex)
            {
                ImpactSphere.SetActive(true);
                ImpactSphere.transform.position = pos;
                for (int j = i; j < TrajectoryLine.positionCount; j++)
                {
                    TrajectoryLine.SetPosition(j, pos);
                }
                break;
            }
            else
            {
                ImpactSphere.SetActive(false);
            }
            vel = vel + gravity * Time.fixedDeltaTime;
            pos = pos + vel * Time.fixedDeltaTime;
        }
    }

    private bool MakesContact(Vector3 position)
    {
        Collider[] coll = Physics.OverlapSphere(position, .3f, ThrownObjectCollisionLayers, QueryTriggerInteraction.Ignore);
        return coll.Length > 0;
    }

    private Sc_Interactible GetClosestContactedInteractible(Vector3 position, float contactAffectRange)
    {
        List<Sc_Interactible> contactedInteractibles = new List<Sc_Interactible>();
        Sc_Interactible chosenInteractible = null;

        Collider[] colls = Physics.OverlapSphere(position, contactAffectRange, ThrownObjectCollisionLayers);
        foreach (Collider coll in colls)
        {
            Sc_Interactible interactible = coll.GetComponent<Sc_Interactible>();
            if (interactible)
            {
                if (interactible.CanBeInteractedWithOnThrow)
                {
                    if (!contactedInteractibles.Contains(interactible))
                    {
                        contactedInteractibles.Add(interactible);
                    }
                }
            }
        }

        float closestDistance = float.MaxValue;
        foreach (Sc_Interactible interactible in contactedInteractibles)
        {
            float distance = Vector3.Distance(interactible.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                chosenInteractible = interactible;
            }
        }

        return chosenInteractible;
    }

    #endregion
    #endregion
    #region SEEDS
    public void UnlockSeeds()
    {

    }
    #endregion
    #region PICKAX
    public void UnlockPickax()
    {

    }
    #endregion

    #region TREASURE
    public void FoundTreasure(Sc_Item item)
    {
        TreasureFound?.Invoke(item);
    }
    #endregion





}
