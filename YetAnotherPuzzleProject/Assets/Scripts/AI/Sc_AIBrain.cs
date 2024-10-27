using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
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
    public SC_AISense Sight;
    public SC_AISense Hearing;
    public AISenseType MainSense = AISenseType.Sight;

    [Header("STATE")]
    public Sc_State StartingState;
    [ReadOnly][SerializeField] private Sc_State _currentState;
    [ReadOnly][SerializeField] private bool _canNoticePlayers = false;

    [Header("MOVEMENT")]
    public float IdleMoveSpeed = 2f;
    public float InvestigateMoveSpeed = 4f;
    public float PursueMoveSpeed = 6f;

    [Header("ALERTED")]
    public float PlayerAwarenessGracePeriod = .2f;
    public float AwarenessThreshold = 1f;
    [ReadOnly][SerializeField] private float _currentAwareness = 0f;
    public float CurrentAwareness { get { return _currentAwareness; } set { _currentAwareness = value; } }
    public float AwarenessResetValue = .8f;
    public float AwarenessDecayRate = .5f;
    public float AwarenessDecayGracePeriod = 1f;
    public AnimationCurve MinMaxGenerateRateCurve;
    public Vector2 MinMaxAwarenessGenerationDistance = new Vector2(6f, 1f);
    public Vector2 MinMaxVisualAwarenessGenerationRate = new Vector2(.2f, 5f);
    public Vector2 MinMaxSoundAwarenessGenerationRate = new Vector2(.1f, 1f);
    private Coroutine _awarenessDecayGraceCO = null;
    private List<Sc_Character_Player> _seenCharacters = new List<Sc_Character_Player>();
    public List<Sc_Character_Player> SeenCharacters { get { return _seenCharacters; } }
    public bool SeesCharacter { get { return _seenCharacters.Count > 0; } }
    public bool CanAwarenessDecay { get { return (_awarenessDecayGraceCO != null || _currentAwareness >= AwarenessThreshold || _currentAwareness <= 0f || HasBeenAlerted) ? false : true; } }
    public bool IsAlerted { get { return _currentAwareness >= AwarenessThreshold; } }
    public bool HasBeenAlerted = false;
    private List<Sc_VisualStimuli> _seenPlayerStimuli = new List<Sc_VisualStimuli>();
    private Coroutine _seenPlayerStimuliGraceCo = null;

    [Header("ATTACK")]
    public float WindUpTime = 1f;
    public float BaseRotationRate = 10f;
    public float WindUpRotationRate = 7f;
    private Coroutine _windUpCo = null;
    private Vector3 _attackingPoint = Vector3.zero;
    public Vector3 AttackingPoint { get { return _attackingPoint; } set { _attackingPoint = value; } }

    public bool IsWindingUp { get { return _windUpCo != null; } }


    [Header("NAVIGATION")]
    private NavMeshPath _path;

    [Header("INPUTS")]
    private Vector2 _movement = Vector2.zero;


    private void OnEnable()
    {
        if (Sight)
        {
            Sight.OnSeeSomething -= OnSeeStimuli;
            Sight.OnSeeSomething += OnSeeStimuli;
            Sight.OnUnseeSomething -= OnUnseeStimuli;
            Sight.OnUnseeSomething += OnUnseeStimuli;
        }

        if (Hearing)
        {
            Hearing.OnHearSomething -= OnHearStimuli;
            Hearing.OnHearSomething += OnHearStimuli;
        }
    }

    private void OnDisable()
    {
        if (Sight)
        {
            Sight.OnSeeSomething -= OnSeeStimuli;
            Sight.OnUnseeSomething -= OnUnseeStimuli;
        }

        if (Hearing)
        {
            Hearing.OnHearSomething -= OnHearStimuli;
        }
    }

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
            _currentState.OnStateEntered(this);
        }
    }

    private void Update()
    {
        //Before
        HandleAgent();
        HandleState();
        HandleAwareness();

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

    private void HandleAwareness()
    {
        if (CanAwarenessDecay)
        {
            _currentAwareness -= Time.deltaTime * AwarenessDecayRate;
        }

        if (SeesCharacter && _canNoticePlayers)
        {
            GeneratePlayerAwareness(true, GetClosestSeenPlayer());
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
    #region SenseEvents
    private void OnSeeStimuli(Sc_VisualStimuli vstimuli)
    {
        //When first seeing a player stimuli, add a grace period. If UnseeStimuli hasn't been called for the same stimuli within the grace period, then do all of this.
        //_currentState.OnSawSomething(this, vstimuli);

        if (vstimuli.Player != null)
        {
            if (!_seenCharacters.Contains(vstimuli.Player))
            {
                _seenCharacters.Add(vstimuli.Player);
            }
            if (!_seenPlayerStimuli.Contains(vstimuli))
            {
                _seenPlayerStimuli.Add(vstimuli);
            }

            if (!_canNoticePlayers)
            {
                if (_seenPlayerStimuliGraceCo == null)
                {
                    _seenPlayerStimuliGraceCo = StartCoroutine(SeePlayerStimuliGraceCo(vstimuli));
                }               
            }
            else
            {
                _currentState.OnSawSomething(this, vstimuli);
            }
        }
        else
        {
            _currentState.OnSawSomething(this, vstimuli);
        }
    }

    private IEnumerator SeePlayerStimuliGraceCo(Sc_VisualStimuli vstimuli)
    {
        float timer = 0f;
        while (timer < PlayerAwarenessGracePeriod)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _canNoticePlayers = true;
        _seenPlayerStimuliGraceCo = null;
        _currentState.OnSawSomething(this, vstimuli);
    }

    private void OnUnseeStimuli(Sc_VisualStimuli vstimuli)
    {
        if (vstimuli.Player != null)
        {
            _seenCharacters.Remove(vstimuli.Player);
            _seenPlayerStimuli.Remove(vstimuli);

            if (_canNoticePlayers && _seenPlayerStimuli.Count == 0)
            {
                _canNoticePlayers = false;

                if (_seenPlayerStimuliGraceCo != null)
                {
                    StopCoroutine(_seenPlayerStimuliGraceCo);
                }
            }
        }
    }

    private void OnHearStimuli(Sc_SoundStimuli sstimuli)
    {
        _currentState.OnHearSomething(this, sstimuli);

        if (sstimuli.Player != null)
        {
            GeneratePlayerAwareness(false, sstimuli.Player);
        }
    }
    #endregion
    #region StateMachine
    private void HandleState()
    {
        if (_currentState == null) return;

        Sc_State newState = _currentState.Tick(this);
        GoToState(newState);
    }

    public void GoToState(Sc_State toState)
    {
        if (toState != _currentState)
        {
            _currentState.OnStateExited(this);
            _currentState = toState;
            _currentState.OnStateEntered(this);
        }
    }
    #endregion
    #region AI Controller actions
    public bool MoveTo(Vector3 destinationPoint, float destinationReachedThreshold = .5f, bool useNavMesh = true)
    {
        bool reached = false;

        if (Vector3.Distance(transform.position, destinationPoint) <= destinationReachedThreshold)
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
                if (NavMesh.SamplePosition(destinationPoint, out nmHit, 1f, NavMesh.AllAreas))
                {
                    if(!NavMesh.CalculatePath(transform.position, nmHit.position, NavMesh.AllAreas, _path))
                    {
                        reached = true;
                        _movement = Vector2.zero;
                        return reached;
                    }
                }
                else
                {
                    reached = true;
                    _movement = Vector2.zero;
                    return reached;
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

    public void MoveForwards()
    {
        Vector3 forwardDestination = transform.position + (transform.forward * 1f);
        MoveTo(forwardDestination, .1f, false);
    }

    public void Attack(Vector3 point)
    {
        StopAttack();
        _attackingPoint = point;
        _windUpCo = StartCoroutine(WindUpCoroutine());
    }

    public void StopAttack()
    {
        StopAttackWindup();
    }

    private void StopAttackWindup()
    {
        if (_windUpCo != null)
        {
            StopCoroutine(_windUpCo);
            _windUpCo = null;
        }
    }

    private IEnumerator WindUpCoroutine()
    {
        float timer = 0f;
        Controller._rotationSharpness = WindUpRotationRate;
        while (timer < WindUpTime)
        {
            Controller.LookAt(_attackingPoint);
            timer += Time.deltaTime;
            yield return null;
        }
        ThrowAttack();
    }

    private void ThrowAttack()
    {
        Controller.CanMove = false;
        Controller.CanRotate = false;

        //Attack anim, make collision.
    }

    #endregion
    #region Awareness
    private float GetPlayerAwarenessGenerationDistanceAlpha(Transform playerCharacter)
    {
        float distance = Vector3.Distance(transform.position, playerCharacter.transform.position);
        float alpha = (distance - MinMaxAwarenessGenerationDistance.x) / (MinMaxAwarenessGenerationDistance.y - MinMaxAwarenessGenerationDistance.x);
        alpha = MinMaxGenerateRateCurve.Evaluate(alpha);
        return alpha;
    }

    private float GeneratePlayerAwarenessVision(Transform playerCharacter)
    {
        float generationRate = Mathf.Lerp(MinMaxVisualAwarenessGenerationRate.x, MinMaxVisualAwarenessGenerationRate.y, GetPlayerAwarenessGenerationDistanceAlpha(playerCharacter));
        generationRate = generationRate * Time.deltaTime;

        return generationRate;
    }

    private float GeneratePlayerAwarenessSound(Transform playerCharacter)
    {
        float generationRate = Mathf.Lerp(MinMaxSoundAwarenessGenerationRate.x, MinMaxSoundAwarenessGenerationRate.y, GetPlayerAwarenessGenerationDistanceAlpha(playerCharacter));

        return generationRate;
    }

    public void GeneratePlayerAwareness(bool visual, Sc_Character_Player playerCharacter)
    {
        float generatedAwareness = 0f;
        if (visual)
        {
            generatedAwareness = GeneratePlayerAwarenessVision(playerCharacter.transform);
        }
        else
        {
            generatedAwareness = GeneratePlayerAwarenessSound(playerCharacter.transform);
        }

        _currentAwareness += generatedAwareness;
        if (_currentAwareness < AwarenessThreshold)
        {
            if (_awarenessDecayGraceCO != null)
            {
                StopCoroutine(_awarenessDecayGraceCO);
            }
            _awarenessDecayGraceCO = StartCoroutine(AwarenessDecayGraceCoroutine());
        }
        else
        {
            OnCrossAwarenessThreshold(playerCharacter);
        }
    }

    private void OnCrossAwarenessThreshold(Sc_Character_Player playerCharacter)
    {
        //Set to pursue target state, and set playerCharacter as target.
        _currentState.OnAwarenessThresholdReached(this, playerCharacter);

        HasBeenAlerted = true;
    }

    private IEnumerator AwarenessDecayGraceCoroutine()
    {
        float timer = 0f;
        while (timer < AwarenessDecayGracePeriod)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _awarenessDecayGraceCO = null;
    }

    private Sc_Character_Player GetClosestSeenPlayer()
    {
        Sc_Character_Player closestPlayer = null;
        float closestDistance = float.MaxValue;
        foreach(Sc_Character_Player player in _seenCharacters)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }
        return closestPlayer;
    }


    #endregion
}
