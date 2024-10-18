using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
    [SerializeField] private Vector2 _offset = Vector2.zero;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _targetLayer;

    private List<Sc_VisualStimuli> _sensedSight = new List<Sc_VisualStimuli>();
    public List<Sc_VisualStimuli> SensedSight { get { return _sensedSight; } }
    private List<Sc_SoundStimuli> _sensedSound = new List<Sc_SoundStimuli>();
    public List<Sc_SoundStimuli> SensedSound { get { return _sensedSound; } }

    public delegate void VisualStimuliEvent(Sc_VisualStimuli stimuli);
    public delegate void SoundStimuliEvent(Sc_SoundStimuli stimuli);
    public event VisualStimuliEvent OnSeeSomething;
    public event SoundStimuliEvent OnHearSomething;

    private void Update()
    {
        SeeStimuli();
    }

    private List<Sc_VisualStimuli> SeeStimuli()
    {
        _sensedSight.Clear();
        Vector3 sightOrigin = transform.position + (transform.forward * _offset.x) + (transform.right * _offset.y);
        Collider[] collidersInSightRadius = Physics.OverlapSphere(sightOrigin, _range, _targetLayer);
        for (int i = 0; i < collidersInSightRadius.Length; i++)
        {
            Sc_VisualStimuli vstimuli = collidersInSightRadius[i].GetComponent<Sc_VisualStimuli>();
            if (vstimuli != null)
            {
                Vector3 targetPosition = vstimuli.transform.position;
                Vector3 directionToTarget = (targetPosition - sightOrigin).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < _angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
                    if (!Physics.Raycast(transform.position + Vector3.up, directionToTarget, distanceToTarget, _obstacleLayer))
                    {
                        if (vstimuli.IsInLight)
                        {
                            _sensedSight.Add(vstimuli);
                            OnSeeSomething?.Invoke(vstimuli);
                        }
                        else
                        {
                            if (_darkVision)
                            {
                                _sensedSight.Add(vstimuli);
                                OnSeeSomething?.Invoke(vstimuli);
                            }
                        }
                    }
                }
            }
        }
        return _sensedSight;
    }

    public List<Sc_Character_Player> SeesPlayers()
    {
        List<Sc_Character_Player> players = new List<Sc_Character_Player>();

        foreach(Sc_VisualStimuli vstimuli in _sensedSight)
        {
            if(vstimuli.Player != null)
            {
                if (!players.Contains(vstimuli.Player))
                {
                    players.Add(vstimuli.Player);
                }
            }
        }

        return players;
    }

    public void HearStimuli(Sc_SoundStimuli sstimuli)
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

    void OnDrawGizmosSelected()
    {
        switch (Sense)
        {
            case AISenseType.Sight:
                Gizmos.color = Color.red;
                float angles = GetAngleFromDirection(transform.position, transform.forward);
                Vector3 sightOrigin = transform.position + (transform.forward * _offset.x) + (transform.right * _offset.y);
                Vector3 initialPos = sightOrigin;
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
            default:
                break;
        }
    }
}
