using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class Sc_Pillar : MonoBehaviour
{
    [Header("LOCK")]
    public Sc_Lock Lock;

    [Header("PARAMETERS")]
    public float _travelDistance = 2f;
    public float _overTime = 1f;
    public AnimationCurve _movementCurve;

    protected List<Sc_CharacterController> _parentedControllers = new List<Sc_CharacterController>();
    protected List<Sc_Pushable> _parentedPushables = new List<Sc_Pushable>();

    protected Rigidbody _rb;
    private Coroutine _movementCo;
    private Vector3 _bottomPos;
    private Vector3 _topPos;

    private Vector3 _cachedPos;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogWarning(this.name + " doesn't have a Rigidbody!");
            return;
        }

        _bottomPos = transform.position;
        _topPos = _bottomPos + (transform.up * _travelDistance);
        _cachedPos = transform.position;
    }

    public void Move(bool activated)
    {
        if (Lock != null)
        {
            Lock.Spin(activated);
            if (!Lock.IsEngaged) return;
        }

        if (_movementCo != null)
        {
            StopCoroutine(_movementCo);
        }
        _movementCo = StartCoroutine(Movement(activated));
    }

    private void TransmitVelocity(Vector3 toVel)
    {
        foreach (Sc_CharacterController controller in _parentedControllers)
        {
            controller.InheritedVelocity += toVel;

            //Vector3 offset = toPos - _rb.position;
            ////Debug.Log(offset);
            //offset = new Vector3(offset.x, 0f, offset.z);
            //rb.MovePosition(rb.position + (offset*.22f));
            ////WHY .22F???? DUNNO LMAO

            //rb.velocity = rb.velocity + toVel;
        }

        foreach (Sc_Pushable pushable in _parentedPushables)
        {
            pushable.InheritedVelocity += toVel;

            //Vector3 offset = toPos - _rb.position;
            ////Debug.Log(offset);
            //offset = new Vector3(offset.x, 0f, offset.z);
            //rb.MovePosition(rb.position + (offset*.22f));
            ////WHY .22F???? DUNNO LMAO

            //rb.velocity = rb.velocity + toVel;
        }
    }

    public void GaugeMove(float gauge)
    {
        if (Lock != null)
        {
            Lock.GaugeSpin(gauge);
            if (!Lock.IsEngaged) return;
        }

        if (gauge > 1f) gauge = 1f;
        if (gauge < 0f) gauge = 0f;

        float alpha = _movementCurve.Evaluate(gauge / 1f);
        Vector3 newPos = Vector3.Lerp(_bottomPos, _topPos, alpha);
        _rb.Move(newPos, _rb.rotation);
        Vector3 transmittedVel = (transform.position - _cachedPos)/Time.fixedDeltaTime;
        TransmitVelocity(newPos);

        RebuildNavMesh();

        _cachedPos = transform.position;

        if (gauge >= 1f)
        {
            OnReachedTop();
        }
        else if (gauge <= 0f)
        {
            OnReachedBottom();
        }
    }

    public void ForceMove(bool activated)
    {
        if (_movementCo != null)
        {
            StopCoroutine(_movementCo);
            _movementCo = null;          
        }

        _rb.Move(activated ? _topPos : _bottomPos, _rb.rotation);

        if (activated)
        {
            OnReachedTop();
        }
        else
        {
            OnReachedBottom();
        }
    }

    private IEnumerator Movement(bool up)
    {
        Vector3 fromPos = transform.position;
        Vector3 toPos = up ? _topPos : _bottomPos;
        Vector3 transmittedVel = Vector3.zero;
        float time = 0f;
        while (time < _overTime)
        {
            float alpha = _movementCurve.Evaluate(time / _overTime);
            Vector3 newPos = Vector3.Lerp(fromPos, toPos, alpha);
            //transform.position = newPos;
            _rb.Move(newPos, _rb.rotation);
            transmittedVel = (transform.position - _cachedPos) / Time.fixedDeltaTime;
            TransmitVelocity(transmittedVel);
            time += Time.deltaTime;

            RebuildNavMesh();

            _cachedPos = transform.position;

            yield return null;
        }
        _rb.Move(toPos, _rb.rotation);
        transmittedVel = (transform.position - _cachedPos) / Time.fixedDeltaTime;
        TransmitVelocity(transmittedVel);

        RebuildNavMesh();

        _cachedPos = transform.position;
        //transform.position = toPos;
        if (up)
        {
            OnReachedTop();
        }
        else
        {
            OnReachedBottom();
        }
        _movementCo = null;
    }

    protected virtual void OnReachedTop()
    {

    }

    protected virtual void OnReachedBottom()
    {

    }

    protected void RebuildNavMesh()
    {
        if (Sc_GameManager.instance != null)
        {
            Sc_GameManager.instance.CurrentLevel.RequestRebuildNavmesh();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Sc_CharacterController character = other.GetComponent<Sc_CharacterController>();
        if (character)
        {
            //character.ParentToObject(_headParent);
            if (_parentedControllers.Contains(character))
            {
                return;
            }
            _parentedControllers.Add(character);
            Debug.Log("Added Controller to platform: " + character.name);
            return;
        }

        Sc_Pushable pushable = other.GetComponent<Sc_Pushable>();
        if (pushable)
        {
            if (_parentedPushables.Contains(pushable))
            {
                return;
            }
            _parentedPushables.Add(pushable);
            Debug.Log("Added Pushable to platform: " + pushable.name);
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Sc_CharacterController character = other.GetComponent<Sc_CharacterController>();
        if (character)
        {
            //character.ParentToObject(_headParent);
            if (_parentedControllers.Contains(character))
            {
                return;
            }
            _parentedControllers.Add(character);
            Debug.Log("Added Controller to platform: " + character.name);
            return;
        }

        Sc_Pushable pushable = other.GetComponent<Sc_Pushable>();
        if (pushable)
        {
            if (_parentedPushables.Contains(pushable))
            {
                return;
            }
            _parentedPushables.Add(pushable);
            Debug.Log("Added Pushable to platform: " + pushable.name);
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Sc_CharacterController character = other.GetComponent<Sc_CharacterController>();
        if (character)
        {
            //character.ParentToObject(null);
            _parentedControllers.Remove(character);
            //_parentedRBs.Remove(character.RB);
            Debug.Log("Removed Controller from platform: " + character.name);
        }

        Sc_Pushable pushable = other.GetComponent<Sc_Pushable>();
        if (pushable)
        {
            _parentedPushables.Remove(pushable);
            //_parentedRBs.Remove(pushable.RB);
            Debug.Log("Removed Pushable from platform: " + pushable.name);
            return;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.up * _travelDistance));
        Gizmos.DrawSphere(transform.position + (transform.up * _travelDistance), .1f);
    }
}
