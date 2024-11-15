using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AISenseType
{
    None,
    Sight,
    Hearing
}

public class SC_AISense : MonoBehaviour
{
    [Header("SENSE TYPE")]
    public AISenseType Sense = AISenseType.None;

    [Header("SIGHT PARAMETERS")]
    [SerializeField] private float _range = 5f;
    [Range(0, 360)]
    [SerializeField] private float _angle = 60f;
    [SerializeField] private bool _darkVision = false;
    [SerializeField] private LayerMask _obstacleLayer;
    public SphereCollider Collider;

    private List<Sc_VisualStimuli> _sensedSight = new List<Sc_VisualStimuli>();
    public List<Sc_VisualStimuli> SensedSight { get { return _sensedSight; } }
    private List<Sc_SoundStimuli> _sensedSound = new List<Sc_SoundStimuli>();
    public List<Sc_SoundStimuli> SensedSound { get { return _sensedSound; } }
    private List<Sc_Character_Player> _sensedPlayer = new List<Sc_Character_Player>();
    public List<Sc_Character_Player> SensedPlayer { get { return _sensedPlayer; } }
    public bool SeesPlayers { get { return _sensedPlayer.Count > 0; } }

    public delegate void VisualStimuliEvent(Sc_VisualStimuli stimuli);
    public delegate void SoundStimuliEvent(Sc_SoundStimuli stimuli);
    public delegate void PlayerCharacterSenseEvent(Sc_Character_Player player);
    public event VisualStimuliEvent OnSeeSomething;
    public event VisualStimuliEvent OnUnseeSomething;
    public event SoundStimuliEvent OnHearSomething;
    public event PlayerCharacterSenseEvent OnNoticedPlayer;

    private void Start()
    {
        Collider.isTrigger = true;
        Collider.radius = _range;
    }

    private void AddVisualStimuli(Sc_VisualStimuli vstimuli)
    {
        if (!_sensedSight.Contains(vstimuli))
        {
            _sensedSight.Add(vstimuli);
            OnSeeSomething?.Invoke(vstimuli);

            if (vstimuli.PlayerSource != null)
            {
                if (!_sensedPlayer.Contains(vstimuli.PlayerSource))
                {
                    _sensedPlayer.Add(vstimuli.PlayerSource);
                }
            }
        }
    }

    private void RemoveVisualStimuli(Sc_VisualStimuli vstimuli)
    {
        _sensedSight.Remove(vstimuli);
        OnUnseeSomething?.Invoke(vstimuli);

        if (vstimuli.PlayerSource != null)
        {
            _sensedPlayer.Remove(vstimuli.PlayerSource);
        }
    }

    private void AddSoundStimuli(Sc_SoundStimuli sstimuli)
    {
        if (!_sensedSound.Contains(sstimuli))
        {
            _sensedSound.Add(sstimuli);

            OnHearSomething?.Invoke(sstimuli);

            sstimuli.OnSoundEnded -= OnSoundStimuliEnded;
            sstimuli.OnSoundEnded += OnSoundStimuliEnded;
        }
    }

    private void OnSoundStimuliEnded(Sc_SoundStimuli sstimuli)
    {
        _sensedSound.Remove(sstimuli);
        sstimuli.OnSoundEnded -= OnSoundStimuliEnded;
    }

    private float GetAngleFromDirection(Vector3 position, Vector3 dir)
    {
        Vector3 forwardLimitPos = position + dir;
        float angles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

        return angles;
    }

    public bool CanSee(Sc_VisualStimuli vstimuli)
    {
        bool canSee = false;
        Vector3 targetPosition = vstimuli.transform.position;
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget <= _range)
        {
            if (Vector3.Angle(transform.forward, directionToTarget) < _angle / 2)
            {
                if (!Physics.Raycast(transform.position + Vector3.up, directionToTarget, distanceToTarget, _obstacleLayer, QueryTriggerInteraction.Ignore))
                {
                    if (vstimuli.Active)
                    {
                        if (vstimuli.IsInLight || (!vstimuli.IsInLight && _darkVision))
                        {
                            canSee = true;
                        }
                    }
                }
            }
        }
        return canSee;
    }

    private void OnTriggerStay(Collider other)
    {
        switch (Sense)
        {
            case AISenseType.Sight:
                Sc_VisualStimuli vstimuli = other.GetComponent<Sc_VisualStimuli>();
                if (vstimuli)
                {
                    Vector3 targetPosition = vstimuli.transform.position;
                    Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                    if (Vector3.Angle(transform.forward, directionToTarget) < _angle / 2)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
                        if (!Physics.Raycast(transform.position + Vector3.up, directionToTarget, distanceToTarget, _obstacleLayer, QueryTriggerInteraction.Ignore))
                        {
                            if (vstimuli.Active)
                            {
                                if (vstimuli.IsInLight || (!vstimuli.IsInLight && _darkVision))
                                {
                                    AddVisualStimuli(vstimuli);
                                }
                                else
                                {
                                    RemoveVisualStimuli(vstimuli);
                                }
                            }
                            else
                            {
                                RemoveVisualStimuli(vstimuli);
                            }
                        }
                        else
                        {
                            RemoveVisualStimuli(vstimuli);
                        }
                    }
                    else
                    {
                        RemoveVisualStimuli(vstimuli);
                    }
                }
                break;
            case AISenseType.Hearing:
                Sc_SoundStimuli sstimuli = other.GetComponent<Sc_SoundStimuli>();
                if (sstimuli)
                {
                    AddSoundStimuli(sstimuli);
                }
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (Sense)
        {
            case AISenseType.Sight:
                Sc_VisualStimuli vstimuli = other.GetComponent<Sc_VisualStimuli>();
                if (vstimuli)
                {
                    RemoveVisualStimuli(vstimuli);
                }
                break;
            case AISenseType.Hearing:
                break;
            default:
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        switch (Sense)
        {
            case AISenseType.Sight:
                Gizmos.color = Color.red;
                float angles = GetAngleFromDirection(transform.position, transform.forward);
                Vector3 initialPos = transform.position;
                Vector3 posA = initialPos;
                float angleSteps = _angle / 20f;
                float angle = angles - _angle / 2;
                for (int i = 0; i <= 20; i++)
                {
                    float rad = Mathf.Deg2Rad * angle;
                    Vector3 posB = initialPos;
                    posB += new Vector3(_range * Mathf.Cos(rad), 0, _range * Mathf.Sin(rad));
                    Gizmos.DrawLine(posA, posB);
                    angle += angleSteps;
                    posA = posB;
                }
                Gizmos.DrawLine(posA, initialPos);
                break;
            case AISenseType.Hearing:
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, _range);
                break;
            default:
                break;
        }
    }
}
