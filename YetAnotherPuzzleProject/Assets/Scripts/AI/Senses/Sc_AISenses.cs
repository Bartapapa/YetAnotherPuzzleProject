using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AISenses : MonoBehaviour
{
    [Header("TYPE")]
    public AISenseType Type = AISenseType.Sight;

    [Header("GENERAL PARAMETERS")]
    public float Range = 5f;
    [SerializeField] private SphereCollider _collider;
    [ReadOnly] public List<Sc_VisualStimuli> SeenVisualStimuli = new List<Sc_VisualStimuli>();
    [ReadOnly] public List<Sc_SoundStimuli> HeardSoundStimuli = new List<Sc_SoundStimuli>();

    [Header("SIGHT PARAMETERS")]
    [Range(0, 360)] public float Angle = 60f;
    public float DarkVisionRange = 2f;
    public LayerMask ObstacleLayers;

    public delegate void VisualStimuliEvent(Sc_VisualStimuli vstimuli);
    public delegate void SoundStimuliEvent(Sc_SoundStimuli sstimuli);
    public event VisualStimuliEvent OnNewVisualStimuliSeen;
    public event VisualStimuliEvent OnUnseeVisualStimuli;
    public event SoundStimuliEvent OnNewSoundStimuliHeard;

    private void Start()
    {
        _collider.isTrigger = true;
        _collider.radius = Range;
    }

    private void RemoveVisualStimuli(Sc_VisualStimuli vstimuli)
    {
        SeenVisualStimuli.Remove(vstimuli);
        OnUnseeVisualStimuli?.Invoke(vstimuli);
    }

    private void AddVisualStimuli(Sc_VisualStimuli vstimuli)
    {
        if (!SeenVisualStimuli.Contains(vstimuli))
        {
            SeenVisualStimuli.Add(vstimuli);
            OnNewVisualStimuliSeen?.Invoke(vstimuli);
        }
    }

    private void RemoveSoundStimuli(Sc_SoundStimuli sstimuli)
    {
        HeardSoundStimuli.Remove(sstimuli);
    }

    private void AddSoundStimuli(Sc_SoundStimuli sstimuli)
    {
        if (!HeardSoundStimuli.Contains(sstimuli))
        {
            HeardSoundStimuli.Add(sstimuli);
            OnNewSoundStimuliHeard?.Invoke(sstimuli);

            sstimuli.OnSoundEnded -= OnSoundStimuliEnded;
            sstimuli.OnSoundEnded += OnSoundStimuliEnded;
        }
    }

    private void OnSoundStimuliEnded(Sc_SoundStimuli sstimuli)
    {
        RemoveSoundStimuli(sstimuli);
        sstimuli.OnSoundEnded -= OnSoundStimuliEnded;
    }

    private float GetAngleFromDirection(Vector3 position, Vector3 dir)
    {
        Vector3 forwardLimitPos = position + dir;
        float angles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

        return angles;
    }

    private void OnTriggerStay(Collider other)
    {
        switch (Type)
        {
            case AISenseType.None:
                break;
            case AISenseType.Sight:
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
        switch (Type)
        {
            case AISenseType.None:
                break;
            case AISenseType.Sight:
                break;
            case AISenseType.Hearing:
                break;
            default:
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        switch (Type)
        {
            case AISenseType.Sight:
                Gizmos.color = Color.yellow;
                float angles = GetAngleFromDirection(transform.position, transform.forward);
                Vector3 initialPos = transform.position;
                Vector3 posA = initialPos;
                float angleSteps = Angle / 20f;
                float angle = angles - Angle / 2;
                for (int i = 0; i <= 20; i++)
                {
                    float rad = Mathf.Deg2Rad * angle;
                    Vector3 posB = initialPos;
                    posB += new Vector3(Range * Mathf.Cos(rad), 0, Range * Mathf.Sin(rad));
                    Gizmos.DrawLine(posA, posB);
                    angle += angleSteps;
                    posA = posB;
                }
                Gizmos.DrawLine(posA, initialPos);
                Gizmos.color = Color.red;
                angles = GetAngleFromDirection(transform.position, transform.forward);
                initialPos = transform.position;
                posA = initialPos;
                angleSteps = Angle / 20f;
                angle = angles - Angle / 2;
                for (int i = 0; i <= 20; i++)
                {
                    float rad = Mathf.Deg2Rad * angle;
                    Vector3 posB = initialPos;
                    posB += new Vector3(DarkVisionRange * Mathf.Cos(rad), 0, DarkVisionRange * Mathf.Sin(rad));
                    Gizmos.DrawLine(posA, posB);
                    angle += angleSteps;
                    posA = posB;
                }
                Gizmos.DrawLine(posA, initialPos);
                break;
            case AISenseType.Hearing:
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, Range);
                break;
            default:
                break;
        }
    }
}
