using KinematicCharacterController;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Flea behavior routine: 
/// Enters in idle, wandering randomly in _unitIdleRangeCheck range via coroutine  
/// If enemy is detected, enters pursuit stage 
/// if within _leapRange distance, Invokes Leap after _timeToLeap 
/// While waiting for leap, rotates towards target
/// Leaps, which turns off player collisions by changing own layer
///     - if not about to leap again, turns player collisions back on
/// If not within _leapRange of target, pursue
/// </summary>

public class FleaBehavior : BaseEnemy
{
    private KinematicCharacterMotor _kCCMotor;
    private CustomMoveController _moveController;
    private AttackData _attackData;
    [FoldoutGroup("Agent Stats/Flea")]
    [SerializeField, Range(0, 20)]
    private float _leapRange, _leapPower, _timeToLeap, _unitIdleRangeCheck;
    [FoldoutGroup("Agent Stats/Flea")]
    [SerializeField, Range(0f, 1f)]
    private float _leapAngle;
    [FoldoutGroup("Agent Stats/Flea")]
    [SerializeField]
    private LayerMask _noPlayerCollisionMask, _playerOnlyMask;
    private AICharacterInputs _aiInput;


    public override void Awake()
    {
        base.Awake();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
        _kCCMotor = GetComponent<KinematicCharacterMotor>();
        _moveController = GetComponent<CustomMoveController>();
        _navMeshAgent.speed = _moveController.MaxStableMoveSpeed;
        _aiInput = new AICharacterInputs();
    }

    bool waitForLanding;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case EnemyBehaviourState.Pursue:
                Pursue();
                break;
            case EnemyBehaviourState.Idle:
                break;
            case EnemyBehaviourState.ChargingAttack:
                ChargeAttack();
                break;
            case EnemyBehaviourState.AttackRecovery:
                break;
            case EnemyBehaviourState.Frozen:
                break;
            case EnemyBehaviourState.AttackStyle1:
                {
                    if (!waitForLanding)
                    {
                        _aiInput.LookVector = TargetDirectionNormalized();
                        _aiInput.MoveVector = Vector3.zero;
                        _moveController.SetInputs(ref _aiInput);
                    }
                }
                break;
            case EnemyBehaviourState.AttackStyle2:
                break;
            case EnemyBehaviourState.Stunned:
                break;
            case EnemyBehaviourState.Interrupted:
                break;
            case EnemyBehaviourState.Enraged:
                break;
            case EnemyBehaviourState.Dying:
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        if (waitForLanding)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f, _playerOnlyMask);
            foreach (Collider collider in hitColliders)
            {
                PlayerStats stats = collider.GetComponent<PlayerStats>();
                if (true)
                {
                    if (stats != null) stats.TakeDamage(_attackData);
                }
            }
            if (_kCCMotor.GroundingStatus.IsStableOnGround)
            {
                waitForLanding = false;
                TransitionState(EnemyBehaviourState.AttackRecovery);
            }
        }
    }

    private void Pursue()
    {
        _navMeshAgent.SetDestination(_target.position);
        Vector3 direction = (_navMeshAgent.nextPosition - transform.position).normalized;
        direction.y = 0;
        _aiInput.LookVector = direction;
        _aiInput.MoveVector = transform.forward;
        _moveController.SetInputs(ref _aiInput);

        if (Vector3.Distance(transform.position, _target.position) < _leapRange)
        {
            TransitionState(EnemyBehaviourState.ChargingAttack);
        }
    }

    Vector3 _idleTargetPos;
    int _idlePathIterations;
    private IEnumerator Idle()
    {
        while (_currentState == EnemyBehaviourState.Idle)
        {
            if (Vector3.Distance(transform.position, _idleTargetPos) < .3f || _idleTargetPos == Vector3.zero)
            {
                _navMeshAgent.nextPosition = transform.position;
                _aiInput.LookVector = Vector3.zero;
                _aiInput.MoveVector = Vector3.zero;
                _moveController.SetInputs(ref _aiInput);
                yield return new WaitForSeconds(1.5f);

                Vector3 randomPoint = transform.position + Random.insideUnitSphere * _unitIdleRangeCheck;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
                {
                    _idleTargetPos = hit.position;
                    _idlePathIterations = 0;
                }
                yield return null;
            }
            else
            {
                _navMeshAgent.SetDestination(_idleTargetPos);
                Vector3 direction = (_navMeshAgent.nextPosition - transform.position).normalized;
                direction.y = 0;
                _aiInput.LookVector = direction;
                _aiInput.MoveVector = transform.forward;
                _moveController.SetInputs(ref _aiInput);

                //sanity check if that will go off if next point takes too long
                if (_idlePathIterations > 400) _idleTargetPos = Vector3.zero;
                yield return null;
            }
        }
    }

    private void ChargeAttack()
    {
        bool facingTarget = Vector3.Dot(transform.forward, TargetDirectionNormalized()) > .97f;
        if (facingTarget)
        {
            TransitionState(EnemyBehaviourState.AttackStyle1);
        }
        else
        {
            _aiInput.LookVector = TargetDirectionNormalized();
            _aiInput.MoveVector = Vector3.zero;
            _moveController.SetInputs(ref _aiInput);
        }
    }

    public override void OnStateEnter(EnemyBehaviourState newState, EnemyBehaviourState oldState)
    {
        switch (newState)
        {
            case EnemyBehaviourState.Pursue:
                _navMeshAgent.nextPosition = transform.position;
                break;
            case EnemyBehaviourState.Idle:
                EnterIdle();
                break;
            case EnemyBehaviourState.ChargingAttack:
                break;
            case EnemyBehaviourState.AttackRecovery:
                {
                    if (Vector3.Distance(transform.position, _target.position) < _leapRange)
                    {
                        CancelInvoke();
                        TransitionState(EnemyBehaviourState.ChargingAttack);
                    }
                    else TransitionState(EnemyBehaviourState.Pursue);
                }
                break;
            case EnemyBehaviourState.Frozen:
                break;
            case EnemyBehaviourState.AttackStyle1:
                Invoke("Leap", _timeToLeap);
                break;
            case EnemyBehaviourState.AttackStyle2:
                break;
            case EnemyBehaviourState.Stunned:
                break;
            case EnemyBehaviourState.Interrupted:
                break;
            case EnemyBehaviourState.Enraged:
                break;
            case EnemyBehaviourState.Dying:
                Die();
                break;
            default:
                break;
        }
    }

    public override void OnStateExit(EnemyBehaviourState newState, EnemyBehaviourState oldState)
    {
        _aiInput.LookVector = Vector3.zero;
        _aiInput.MoveVector = Vector3.zero;
        _moveController.SetInputs(ref _aiInput);
        switch (oldState)
        {
            case EnemyBehaviourState.Pursue:
                break;
            case EnemyBehaviourState.Idle:
                _idleTargetPos = Vector3.zero;
                StopCoroutine(Idle());
                break;
            case EnemyBehaviourState.ChargingAttack:
                break;
            case EnemyBehaviourState.AttackRecovery:
                break;
            case EnemyBehaviourState.Frozen:
                break;
            case EnemyBehaviourState.AttackStyle1:
                {
                    StartCoroutine(SetCollisionWithPlayer(true));
                }
                break;
            case EnemyBehaviourState.AttackStyle2:
                break;
            case EnemyBehaviourState.Stunned:
                break;
            case EnemyBehaviourState.Interrupted:
                break;
            case EnemyBehaviourState.Enraged:
                break;
            case EnemyBehaviourState.Dying:
                break;
            default:
                break;
        }
    }

    private void Leap()
    {
        Vector3 velocityDirection = TargetDirectionNormalized();
        velocityDirection.y += _leapAngle;
        _moveController.AddVelocity(velocityDirection * _leapPower);
        StartCoroutine(SetCollisionWithPlayer(false));
        _attackData.Damage = Damage;
        _attackData.GeneratedByPlayer = false;
        _attackData = IDGenerator.GenerateID(_attackData);

        //waitForLanding controls FixedUpdate collision checks
        waitForLanding = true;
    }

    private Vector3 TargetDirectionNormalized()
    {
        //flea should always behave as though player is at same height when attacking
        Vector3 getTargetPosAtMyHeight = _target.position;
        getTargetPosAtMyHeight.y = transform.position.y;
        Vector3 targetPosition = (getTargetPosAtMyHeight - transform.position).normalized;
        return targetPosition;
    }

    private IEnumerator SetCollisionWithPlayer(bool collideWithPlayer)
    {
        if (collideWithPlayer)
        {
            //gives a slight delay if not charging, otherwise cancels to avoid logic conflicts
            if (_currentState != EnemyBehaviourState.AttackStyle1)
            {
                yield return new WaitForSeconds(.2f);
                transform.ChangeLayersRecursively("Enemy");
            }
            else yield return null;
        }
        else
        {
            transform.ChangeLayersRecursively("NoPlayerCollision");
        }
    }

    private void Die()
    {
        CancelInvoke();
        StopAllCoroutines();
        _target = null;
        Destroy(this.gameObject);
    }

    public override void EnemyDetected(PlayerAbilityController abilityController)
    {
        base.EnemyDetected(abilityController);
        TransitionState(EnemyBehaviourState.Pursue);
    }

    public override void EnterIdle()
    {
        base.EnterIdle();
        StartCoroutine(Idle());
    }

    public override AttackData GiveDamage()
    {
        return base.GiveDamage();
    }

    public override void TakeDamage(AttackData attack)
    {
        base.TakeDamage(attack);
    }

    public override void TransitionState(EnemyBehaviourState newState)
    {
        base.TransitionState(newState);
    }


}
