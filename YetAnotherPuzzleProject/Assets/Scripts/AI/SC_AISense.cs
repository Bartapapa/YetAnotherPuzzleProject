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
    [SerializeField] private Vector2 _offset = Vector2.zero;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _targetLayer;

    private List<Sc_VisualStimuli> _sensedSight = new List<Sc_VisualStimuli>();

    public delegate void VisualStimuliEvent(Sc_VisualStimuli stimuli);
    public delegate void SoundStimuliEvent(Sc_SoundStimuli stimuli);
    private event VisualStimuliEvent OnSeeSomething;
    private event SoundStimuliEvent OnHearSomething;

    private void Update()
    {
        HandleSight();
    }

    private List<Sc_VisualStimuli> HandleSight()
    {
        _sensedSight.Clear();
        Collider[] collidersInSightRadius = Physics.OverlapSphere(transform.position, _range, _targetLayer);
        for (int i = 0; i < collidersInSightRadius.Length; i++)
        {
            Sc_VisualStimuli vstimuli = collidersInSightRadius[i].GetComponent<Sc_VisualStimuli>();
            if (vstimuli != null)
            {
                Vector3 targetPosition = vstimuli.transform.position;
                Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < _angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
                    if (!Physics.Raycast(transform.position + Vector3.up, directionToTarget, distanceToTarget, _obstacleLayer))
                    {
                        _sensedSight.Add(vstimuli);
                        OnSeeSomething?.Invoke(vstimuli);
                    }
                }
            }
        }
        return _sensedSight;
    }

    public Sc_Character_Player SeesPlayer()
    {
        Sc_Character_Player player = null;
        return player;
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
            default:
                break;
        }
    }
}
