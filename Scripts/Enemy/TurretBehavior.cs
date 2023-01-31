using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class TurretBehavior : BaseEnemy, IDamageable, IDamageGiver, IEnemyBehavior
{
    [SerializeField]
    private ParticleSystem _laser, _lightningCharge, _backCharge;
    [SerializeField]
    private ChainIKConstraint _baseChain, _tipChain;
    [SerializeField]
    private float _timeToCharge, _timeBetweenAttacks, MaxAttackDistance;
    [SerializeField]
    private int _attackDamage;
    [SerializeField]
    private Transform _followTransform;
    [SerializeField]
    private EventReference _chargeSound;
    [SerializeField]
    private List<ParticleSystem> _particles;
    [SerializeField]
    private Rigidbody _mainBody;
    [SerializeField]
    private List<Collider> _colliderList;

    private AttackData _attackData;
    private TimerScript _attackChargeTimer, _cooldownTimer;
    private FMOD.Studio.EventInstance _instance;
    private float _chargePower = 0f;

    [SerializeField]
    private float _baseChainStartWeight = 1, _tipChainStartWeight = 1;

    public new Transform VisionTransform { get => _tipChain.data.tip; }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }

    private void Start()
    {
        _instance = RuntimeManager.CreateInstance(_chargeSound);
        _instance.setVolume(.4F);
        RuntimeManager.AttachInstanceToGameObject(_instance, _tipChain.transform);


        _attackChargeTimer = new TimerScript();
        _cooldownTimer = new TimerScript();

        _attackData = new AttackData();
        _attackData.GeneratedByPlayer = false;
        _attackData.Damage = _attackDamage;
    }

    private void Update()
    {

        if (_target != null)
        {
            _followTransform.position = _target.position;
        }
        switch (_currentState)
        {
            case EnemyBehaviourState.Pursue:
                break;
            case EnemyBehaviourState.Idle:
                break;
            case EnemyBehaviourState.ChargingAttack:
                ChargeAttack();
                break;
            case EnemyBehaviourState.AttackRecovery:
                if (_cooldownTimer.Tick(Time.deltaTime))
                {
                    TransitionState(EnemyBehaviourState.ChargingAttack);
                }
                break;
            case EnemyBehaviourState.Frozen:
                break;
            case EnemyBehaviourState.AttackStyle1:
                Fire();
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
        //Reset timers if something changes them
        if (_attackChargeTimer.CountdownTime != _timeToCharge)
        {
            _attackChargeTimer.CountdownTime = _timeToCharge;
            _attackChargeTimer.Reset();
        }
        if (_cooldownTimer.CountdownTime != _timeBetweenAttacks)
        {
            _cooldownTimer.CountdownTime = _timeBetweenAttacks;
            _cooldownTimer.Reset();
        }
    }
    public override void OnStateEnter(EnemyBehaviourState newState, EnemyBehaviourState oldState)
    {
        switch (newState)
        {
            case EnemyBehaviourState.Pursue:
                break;
            case EnemyBehaviourState.Idle:
                EnterIdle();
                CancelInvoke();
                StopChargeEffects();
                _instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _target = null;
                Idle();
                break;
            case EnemyBehaviourState.ChargingAttack:
                break;
            case EnemyBehaviourState.AttackRecovery:
                break;
            case EnemyBehaviourState.Frozen:
                break;
            case EnemyBehaviourState.AttackStyle1:
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
                {
                    _instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    _instance.release();
                    StopChargeEffects();
                    foreach (ParticleSystem particle in _particles)
                    {
                        particle.Stop();
                    }
                    foreach (Collider collider in _colliderList)
                    {
                        Rigidbody body = collider.gameObject.AddComponent<Rigidbody>();
                        collider.transform.parent = null;
                        collider.gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
                        body.isKinematic = false;
                        body.useGravity = true;
                        body.mass = .5f;
                        body.AddExplosionForce(100, transform.TransformDirection(PlayerManager.Instance.Player.transform.position - body.transform.position).normalized * 5, 20f);
                        Destroy(collider.gameObject, Random.Range(30f, 90f));
                    }
                    _target = null;
                    Destroy(this.gameObject, .1f);
                }
                break;
            default:
                break;
        }
    }

    public override void OnStateExit(EnemyBehaviourState newState, EnemyBehaviourState oldState)
    {
        switch (oldState)
        {
            case EnemyBehaviourState.Pursue:
                break;
            case EnemyBehaviourState.Idle:
                _tipChain.weight = _tipChainStartWeight;
                _baseChain.weight = _baseChainStartWeight;
                InvokeRepeating("CheckDistance", 5f, 1f);
                break;
            case EnemyBehaviourState.ChargingAttack:
                break;
            case EnemyBehaviourState.AttackRecovery:
                break;
            case EnemyBehaviourState.Frozen:
                break;
            case EnemyBehaviourState.AttackStyle1:
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

    public override AttackData GiveDamage()
    {
        _attackData.ID = UnityEngine.Random.Range(0, 10000);
        return _attackData;
    }

    private void Idle()
    {
        _baseChain.weight = 0f;
        _tipChain.weight = 0f;
    }

    private void ChargeAttack()
    {
        if (!_lightningCharge.isPlaying)
        {
            PlayChargeEffects();
        }
        if (_attackChargeTimer.Tick(Time.deltaTime))
        {
            TransitionState(EnemyBehaviourState.AttackStyle1);
        }
        else
        {
            _chargePower = Mathf.InverseLerp(_attackChargeTimer.CountdownTime, 0, _attackChargeTimer.CurrentTime);
            _instance.setParameterByName("ChargeShotLevel", _chargePower);
        }
    }

    private void PlayChargeEffects()
    {
        _instance.start();
        _lightningCharge.Play();
        _backCharge.Play();
    }

    private void Fire()
    {
        _instance.setParameterByName("ChargeShotLevel", 2f);
        _laser.Play();
        StopChargeEffects();
        TransitionState(EnemyBehaviourState.AttackRecovery);
    }

    private void StopChargeEffects()
    {
        _lightningCharge.Stop();
        _backCharge.Stop();
    }


    private void OnDestroy()
    {
        CancelInvoke();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public override void EnemyDetected(PlayerAbilityController abilityController)
    {
        _target = abilityController.PlayerTarget;
        TransitionState(EnemyBehaviourState.ChargingAttack);
    }

    public void CheckDistance()
    {
        if (Vector3.Distance(transform.position, _target.position) > MaxAttackDistance)
        {
            TransitionState(EnemyBehaviourState.Idle);
        }
    }

    public override void EnterIdle()
    {
        base.EnterIdle();
    }

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void TransitionState(EnemyBehaviourState newState)
    {
        base.TransitionState(newState);


    }


}
