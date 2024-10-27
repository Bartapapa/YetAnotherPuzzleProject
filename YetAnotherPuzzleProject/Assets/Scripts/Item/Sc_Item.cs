using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Sc_Item : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public Sc_Interactible _interactible;
    public ParticleSystem ItemDestroyParticles;
    public Transform MeshCenterPoint;
    [ReadOnly] public Sc_Inventory _inInventory;
    private Rigidbody _rb;
    private Collider _coll;
    private Renderer[] _renderers;

    [Header("ITEM DATA")]
    public SO_ItemData _itemData;

    [Header("THROW PARAMETERS")]
    [ReadOnly] public bool IsBeingThrown = false;
    private Sc_Character _thrownByCharacter;

    private float _destroyItemDuration = .25f;
    private float _destroyItemFlashIntensity = 2f;

    private Vector3 _treasureAcquireOffset = new Vector3(0f, 2f, 0f);
    private Transform _treasureAcquireAnchor = null;
    private Vector3 _treasureAcquirePoint = Vector3.zero;
    private float _treasureAcquireLifetime = 1f;
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
        if (_inInventory != null)
        {
            _inInventory.DropItem(this);
        }
        Destroy(this.gameObject);
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
        while (timer < _treasureAcquireLifetime)
        {
            if (_treasureAcquireAnchor != null)
            {
                _treasureAcquirePoint = _treasureAcquireAnchor.position;
            }
            transform.position = _treasureAcquirePoint + _treasureAcquireOffset;
            timer += Time.deltaTime;
            yield return null;
        }
        DestroyItem();
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

    public void DestroyItem()
    {
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

        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    private void SpreadContact()
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

    private void OnCollisionEnter(Collision collision)
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
