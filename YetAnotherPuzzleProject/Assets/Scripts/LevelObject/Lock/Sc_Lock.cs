using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Lock : Sc_Activateable
{
    [Header("LOCK OBJECT REFS")]
    public Animator Anim;
    public Sc_Destructible Destructible;

    [Header("LOCK PARAMETERS")]
    public int DestroyWeightThreshold = 50;

    [Header("SPIN PARAMETERS")]
    public float SpinDuration = 1f;
    public float SpinRotation = 360f;

    [Header("MESH REFS")]
    public Transform TopPivot;
    public Transform BottomPivot;

    private Coroutine _spinCo;
    private float _input = 0f;

    public delegate void DefaultEvent();
    public delegate void BoolEvent(bool on);
    public DefaultEvent LockDestroyed;
    public BoolEvent LockEngaged;

    #region Activateable implementation
    public override bool Activate(bool toggleOn)
    {
        if (base.Activate(toggleOn))
        {
            ToggleEngage(toggleOn);
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public override void ForceActivate(bool toggleOn)
    {
        base.ForceActivate(toggleOn);
        ForceEngage(toggleOn);
    }
    #endregion

    public void LockWasDestroyed()
    {
        ForceActivate(false);
        LockDestroyed?.Invoke();
        Destructible.DestructibleDestroy(null);
    }

    public void ForceEngage(bool engage)
    {
        //When sounds come, redo this.
        ToggleEngage(engage);
    }

    public void ToggleEngage(bool engage)
    {
        if (engage)
        {
            Engage();
        }
        else
        {
            Disengage();
        }

        LockEngaged?.Invoke(engage);
    }

    public void Engage()
    {
        TopPivot.localPosition = new Vector3(0f, 0f, 0f);
        TopPivot.localEulerAngles = BottomPivot.localEulerAngles;


        Anim.SetBool("Engaged", true);
    }

    public void Disengage()
    {
        TopPivot.localPosition = new Vector3(0f, .75f, 0f);
        TopPivot.localEulerAngles = Vector3.zero;

        Anim.SetBool("Engaged", false);
    }

    public void FailToEngage()
    {
        Anim.Play("FailToEngage");
    }

    public void Spin(bool activated)
    {
        StopSpin();
        _spinCo = StartCoroutine(SpinCoroutine(activated));
    }

    public void GaugeSpin(float gauge, bool input = false)
    {
        float alpha = 0f;
 
        if (input)
        {
            _input += gauge;
            alpha = _input;
        }
        else
        {
            alpha = gauge;
            
        }

        Vector3 euler = new Vector3(0f, SpinRotation * alpha, 0f);

        if (IsActivated)
        {
            TopPivot.localEulerAngles = euler;
            BottomPivot.localEulerAngles = euler;
        }
        else
        {
            TopPivot.localEulerAngles = Vector3.zero;
            BottomPivot.localEulerAngles = euler;
        }
    }

    private void StopSpin()
    {
        if (_spinCo != null)
        {
            StopCoroutine(_spinCo);
            _spinCo = null;
        }

        CanBeActivated = true;

        //TopPivot.eulerAngles = Vector3.zero;
        //BottomPivot.eulerAngles = Vector3.zero;
    }

    private IEnumerator SpinCoroutine(bool activated)
    {
        float timer = 0f;
        TopPivot.localEulerAngles = Vector3.zero;
        BottomPivot.localEulerAngles = Vector3.zero;

        CanBeActivated = false;

        while (timer < SpinDuration)
        {
            float rotateValue = (SpinRotation / SpinDuration) * Time.deltaTime;
            float multiplier = activated ? 1f : -1f;
            rotateValue = rotateValue * multiplier;
            if (IsActivated)
            {
                TopPivot.Rotate(0f, rotateValue, 0f);
                BottomPivot.Rotate(0f, rotateValue, 0f);
            }
            else
            {
                BottomPivot.Rotate(0f, rotateValue, 0f);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        CanBeActivated = true;

        TopPivot.localEulerAngles = Vector3.zero;
        BottomPivot.localEulerAngles = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Destructible.Destroyed) return;

        Sc_WeightedObject wObject = other.GetComponent<Sc_WeightedObject>();
        if (wObject)
        {
            if (wObject._weight >= DestroyWeightThreshold)
            {
                if (wObject.RBVelocity.y <= -3f)
                {
                    LockWasDestroyed();
                }
            }
        }
    }
}
