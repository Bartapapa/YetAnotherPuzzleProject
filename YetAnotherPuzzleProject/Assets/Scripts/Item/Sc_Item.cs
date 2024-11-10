using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Item : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Interactible _interactible;
    public ParticleSystem ItemDestroyParticles;
    public Transform MeshCenterPoint;
    [ReadOnly] public Sc_Inventory _inInventory;
    protected Rigidbody _rb;
    protected Collider _coll;
    private Renderer[] _renderers;

    [Header("ITEM DATA")]
    public SO_ItemData _itemData;

    [Header("EQUIPPED")]
    [ReadOnly] public bool IsEquipped = false;

    [Header("THROW PARAMETERS")]
    [ReadOnly] public bool IsBeingThrown = false;
    protected Sc_Character _thrownByCharacter;

    [Header("Sounds")]
    public AudioSource Source;
    public AudioClip Store;
    public AudioClip Equip;
    public AudioClip Drop;
    public AudioClip Break;

    private float _destroyItemDuration = .25f;
    private float _destroyItemFlashIntensity = 2f;

    private Vector3 _treasureAcquireOffset = new Vector3(0f, 2f, 0f);
    private Transform _treasureAcquireAnchor = null;
    private Vector3 _treasureAcquirePoint = Vector3.zero;
    private float _commonTreasureLifetime = 1.5f;
    private float _rareTreasureLifetime = 2f;
    private float _veryRareTreasureLifetime = 3f;
    private bool _acquire;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
        _renderers = GetComponentsInChildren<Renderer>();
    }

    public virtual void OnInteractedWith(Sc_Character interactor)
    {
        Sc_Character_Player player = interactor.GetComponent<Sc_Character_Player>();
        if (player)
        {
            if (player.Inventory.IsCurrentlyHoldingItem)
            {
                player.Inventory.PickUpItem(this);
            }
            else
            {
                player.Inventory.PickUpItemAndEquip(this);
            }          
        }
    }

    public virtual void UseItem()
    {
        DestroyItem();
    }

    public virtual void UseItemSpecial(int index)
    {

    }

    public virtual bool OnBeforeItemStore()
    {
        return true;
    }

    public virtual void StopUsingItem()
    {
        if (!_inInventory.IsUsingItem) return;
    }

    public virtual bool UseItemAsKey(Sc_Interactible lockedInteractible)
    {
        DestroyItem();
        return true;
    }

    public virtual void AcquireTreasure(Transform acquirer)
    {
        _treasureAcquireAnchor = acquirer;
        _treasureAcquirePoint = acquirer.position;
        StartCoroutine(AcquireTreasureCoroutine());
    }

    private IEnumerator AcquireTreasureCoroutine()
    {
        float timer = 0f;
        Vector3 dir = -Camera.main.transform.forward;
        ParticleSystem particles;
        float lifetime = 1f;
        switch (_itemData.Rarity)
        {
            case TreasureRarity.Common:
                lifetime = _commonTreasureLifetime;
                particles = Instantiate<ParticleSystem>(Sc_TreasureManager.instance.CommonParticles, _treasureAcquirePoint + _treasureAcquireOffset, Quaternion.identity, Sc_GameManager.instance.CurrentLevel.transform);
                break;
            case TreasureRarity.Rare:
                lifetime = _rareTreasureLifetime;
                particles = Instantiate<ParticleSystem>(Sc_TreasureManager.instance.RareParticles, _treasureAcquirePoint + _treasureAcquireOffset, Quaternion.identity, Sc_GameManager.instance.CurrentLevel.transform);
                break;
            case TreasureRarity.VeryRare:
                lifetime = _veryRareTreasureLifetime;
                particles = Instantiate<ParticleSystem>(Sc_TreasureManager.instance.VeryRareParticles, _treasureAcquirePoint + _treasureAcquireOffset, Quaternion.identity, Sc_GameManager.instance.CurrentLevel.transform);
                break;
            default:
                lifetime = _commonTreasureLifetime;
                particles = Instantiate<ParticleSystem>(Sc_TreasureManager.instance.CommonParticles, _treasureAcquirePoint + _treasureAcquireOffset, Quaternion.identity, Sc_GameManager.instance.CurrentLevel.transform);
                break;
        }
        while (timer < lifetime)
        {
            if (_treasureAcquireAnchor != null)
            {
                _treasureAcquirePoint = _treasureAcquireAnchor.position;
            }
            if (particles != null)
            {
                particles.transform.position = _treasureAcquirePoint + _treasureAcquireOffset;
            }
            transform.position = _treasureAcquirePoint + _treasureAcquireOffset;
            transform.forward = dir;
            timer += Time.deltaTime;
            yield return null;
        }
        TreasureDisappear();
    }

    public virtual void ThrowItem(Sc_Character throwingCharacter, Vector3 throwDirection)
    {
        _interactible.CanBeInteractedWith = false;
        _rb.isKinematic = false;
        _rb.useGravity = true;
        _coll.isTrigger = false;
        _rb.AddTorque(_itemData.SpinForce, ForceMode.Impulse);

        Vector3 throwDir = throwDirection * _itemData.ThrowForce;

        _rb.AddForce(throwDir, ForceMode.Impulse);

        IsBeingThrown = true;
        _thrownByCharacter = throwingCharacter;

        //Take rigidbody, make is not kinematic no more.
        //Start throwing coroutine, wherein the object's velocity is set by an animation curve. It flies in a straight direction before starting to fall.
        //During this coroutine, it constantly checks in the direction of its trajectory with a spheretrace. If it hits anything, it breaks.
        //After a definite amount of time, it is self-destroyed anyhow to prevent it from actually going waaaay away.
    }

    private IEnumerator ThrowCoroutine()
    {
        yield return null;
    }

    public void TreasureDisappear()
    {
        if (_inInventory != null)
        {
            _inInventory.DropItem(this);
        }
        StartCoroutine(DisappearTreasureCo());
    }

    private IEnumerator DisappearTreasureCo()
    {
        float timer = 0f;
        while (timer < _destroyItemDuration)
        {
            timer += Time.deltaTime;

            foreach (Renderer rend in _renderers)
            {
                foreach (Material mat in rend.materials)
                {
                    mat.color = Color.white * ((timer / _destroyItemDuration) * _destroyItemFlashIntensity);
                }
            }
            yield return null;
        }

        if (ItemDestroyParticles)
        {
            ParticleSystem particles = Instantiate<ParticleSystem>(ItemDestroyParticles, MeshCenterPoint.position, Quaternion.identity);
        }

        OnTreasureDisappear();
    }

    public void OnTreasureDisappear()
    {
        if (Sc_GameManager.instance != null)
        {
            //Sc_GameManager.instance.SoundManager.PlaySFX(Source, Break);
            //Sc_GameManager.instance.SoundManager.CreateAudioSourceObject(Break, transform.position, .5f);
        }

        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    public void DestroyItem()
    {
        if (_inInventory != null)
        {
            _inInventory.DropItem(this);
        }
        StartCoroutine(DestroyItemCo());
    }

    private IEnumerator DestroyItemCo()
    {
        float timer = 0f;
        while (timer < _destroyItemDuration)
        {
            timer += Time.deltaTime;

            foreach(Renderer rend in _renderers)
            {
                foreach (Material mat in rend.materials)
                {
                    mat.color = Color.white * ((timer / _destroyItemDuration) * _destroyItemFlashIntensity);
                }
            }
            yield return null;
        }

        if (ItemDestroyParticles)
        {
            ParticleSystem particles = Instantiate<ParticleSystem>(ItemDestroyParticles, MeshCenterPoint.position, Quaternion.identity);
        }

        OnItemDestroyed();
    }

    protected void SpreadContact()
    {
        Vector3 positionAtContact = transform.position;
        Collider[] coll = Physics.OverlapSphere(positionAtContact, _itemData.OnContactAffectRange);
        if (_itemData.InteractsOnThrow)
        {
            List<Sc_Interactible> interactibles = new List<Sc_Interactible>();
            foreach (Collider collider in coll)
            {
                Sc_Interactible interact = collider.GetComponent<Sc_Interactible>();
                if (interact)
                {
                    if (!interactibles.Contains(interact))
                    {
                        interactibles.Add(interact);
                    }
                }
            }
            foreach(Sc_Interactible interactible in interactibles)
            {
                interactible.InteractWithThrow();
            }
        }

        if (_itemData.StunsOnThrow)
        {
            List<Sc_Character> characters = new List<Sc_Character>();
            foreach(Collider collider in coll)
            {
                Sc_Character character = collider.GetComponent<Sc_Character>();
                if (character)
                {
                    if (!characters.Contains(character))
                    {
                        characters.Add(character);
                    }
                }
            }
            foreach(Sc_Character character in characters)
            {
                Vector3 directionOfAttack = character.transform.position - positionAtContact;
                directionOfAttack = new Vector3(directionOfAttack.x, 0f, directionOfAttack.z);
                directionOfAttack = directionOfAttack.normalized;
                character.Hurt(_thrownByCharacter, directionOfAttack, positionAtContact);
            }
        }

        //If any interactibles in range of spread, interact with them.
        //If any enemies in range of spread, stun them.
    }

    public virtual void OnItemDrop()
    {
        if (Sc_GameManager.instance != null)
        {
            Sc_GameManager.instance.SoundManager.PlaySFX(Source, Drop, new Vector2(.9f, 1f));
        }
    }

    public virtual void OnItemStore()
    {
        if (Sc_GameManager.instance != null)
        {
            //Sc_GameManager.instance.SoundManager.PlaySFX(Source, Store);
            Sc_GameManager.instance.SoundManager.CreateAudioSourceObject(Store, transform.position, .3f);
        }
    }

    public virtual void OnItemEquip()
    {
        if (Sc_GameManager.instance != null)
        {
            Sc_GameManager.instance.SoundManager.PlaySFX(Source, Equip, new Vector2(.9f, 1f));
        }
    }

    public virtual void OnItemDestroyed()
    {
        if (Sc_GameManager.instance != null)
        {
            //Sc_GameManager.instance.SoundManager.PlaySFX(Source, Break);
            Sc_GameManager.instance.SoundManager.CreateAudioSourceObject(Break, transform.position, .5f);
        }

        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (IsBeingThrown)
        {
            Vector3 impactNormal = collision.GetContact(0).normal;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.AddTorque(-_itemData.SpinForce, ForceMode.Impulse);
            _rb.AddForce(impactNormal * 5f, ForceMode.Impulse);

            IsBeingThrown = false;

            SpreadContact();

            DestroyItem();
        }
    }




}
