using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public struct AIInput
{
    public float moveX;
    public float moveY;
}

public class Sc_AIBrain : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    public NavMeshAgent Agent;
    public Sc_CharacterController Controller;

    [Header("STATE")]
    public Sc_State StartingState;
    [ReadOnly][SerializeField] private Sc_State _currentState;

    [Header("NAVIGATION")]
    [SerializeField] private float _destinationReachedThreshold = .5f;
    private NavMeshPath _path;

    [Header("INPUTS")]
    private Vector2 _movement = Vector2.zero;

    [Header("DEBUG")]
    public Transform TARGET;

    private void Start()
    {
        Agent.updatePosition = false;
        Agent.updateRotation = false;

        _path = new NavMeshPath();

        InitializeBrain();
    }

    private void InitializeBrain()
    {
        if (StartingState != null)
        {
            _currentState = StartingState;
            _currentState.OnStateEntered();
        }
    }

    private void Update()
    {
        //Before
        HandleAgent();
        HandleState();

        //Actions
        if (TARGET) MoveTo(TARGET.position);

        //End
        ApplyInputs();
    }

    #region OnTick
    private void HandleAgent()
    {
        if (Vector3.Distance(Agent.nextPosition, transform.position) >= 3f)
        {
            Agent.Warp(transform.position);
        }
        else
        {
            Agent.nextPosition = transform.position;
        }
    }

    private void ApplyInputs()
    {
        AIInput aiInputs = new AIInput();
        aiInputs.moveX = _movement.x;
        aiInputs.moveY = _movement.y;

        Controller.SetAIInputs(ref aiInputs);
    }
    #endregion
    #region StateMachine
    private void HandleState()
    {
        if (_currentState == null) return;

        Sc_State newState = _currentState.Tick(this);
        if (newState != _currentState)
        {
            _currentState.OnStateExited();
            _currentState = newState;
            _currentState.OnStateEntered();
        }
    }
    #endregion
    #region AI actions
    public bool MoveTo(Vector3 destinationPoint, bool useNavMesh = true)
    {
        bool reached = false;

        if (Vector3.Distance(transform.position, destinationPoint) <= _destinationReachedThreshold)
        {
            reached = true;
            _movement = Vector2.zero;
            return reached;
        }
        else
        {
            if (useNavMesh)
            {
                NavMeshHit nmHit;
                if (NavMesh.SamplePosition(destinationPoint, out nmHit, 5f, NavMesh.AllAreas))
                {
                    NavMesh.CalculatePath(transform.position, nmHit.position, NavMesh.AllAreas, _path);
                }

                if (_path.corners.Length > 0)
                {
                    Vector3 pathDirection = new Vector3(_path.corners[1].x - transform.position.x, 0, _path.corners[1].z - transform.position.z);
                    pathDirection = pathDirection.normalized;

                    _movement.y = pathDirection.z;
                    _movement.x = pathDirection.x;
                }
                else
                {
                    _movement = Vector2.zero;
                }
            }
            else
            {
                Vector3 direction = new Vector3(destinationPoint.x - transform.position.x, 0f, destinationPoint.z - transform.position.z);
                direction = direction.normalized;

                _movement.y = direction.z;
                _movement.x = direction.x;
            }
        }

        return reached;
    }
    #endregion
}
