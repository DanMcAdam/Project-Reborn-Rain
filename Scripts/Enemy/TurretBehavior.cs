using FMODUnity;
using MoreMountains.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class TurretBehavior : MonoBehaviour, IDamageable, IDamageGiver, IEnemyBehavior
{
    [SerializeField]
    private ParticleSystem _laser, _lightningCharge, _backCharge;
    [SerializeField]
    private ChainIKConstraint _baseChain, _tipChain;
    [SerializeField]
    private float _timeToCharge, _timeBetweenAttacks, MaxAttackDistance;
    [SerializeField]
    private int _attackDamage, _turretMaxHealth;
    [SerializeField]
    private Transform _followTransform;
    [SerializeField]
    private EventReference _chargeSound;
    [SerializeField]
    private DetectPlayer _detectPlayer;
    [SerializeField]
    private MMHealthBar _healthBar;
    [SerializeField]
    private List<ParticleSystem> _particles;
    [SerializeField]
    private Rigidbody _mainBody;
    [SerializeField]
    private List<Collider> _colliderList;

    public float TurretHealth;
    private Queue<int> _previousAttackIDs = new Queue<int>(5);
    private AttackData _playerAttack;
    private Transform _target;
    private TimerScript _attackTimer, _cooldownTimer;
    private FMOD.Studio.EventInstance _instance;
    private float _chargePower = 0f;
    private bool _canCharge = true;
    public bool IsTargeting => _target != null;

    private float _baseChainStartWeight, _tipChainStartWeight;

    public Transform VisionTransform { get => _tipChain.data.tip; }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }

    private void Start()
    {
        _instance = RuntimeManager.CreateInstance(_chargeSound);
        _instance.setVolume(.7F);
        RuntimeManager.AttachInstanceToGameObject(_instance, _tipChain.transform);

        _baseChainStartWeight = _baseChain.weight;
        _tipChainStartWeight = _tipChain.weight;
        _attackTimer = new TimerScript();
        _cooldownTimer = new TimerScript();

        _playerAttack = new AttackData();
        _playerAttack.GeneratedByPlayer = false;
        _playerAttack.Damage = _attackDamage;
        TurretHealth = _turretMaxHealth;

        _detectPlayer.BeginInvoke(this);
    }

    private void Update()
    {

        if (_attackTimer.CountdownTime != _timeToCharge)
        {
            _attackTimer.CountdownTime = _timeToCharge;
            _attackTimer.Reset();
        }
        if (_cooldownTimer.CountdownTime != _timeToCharge)
        {
            _cooldownTimer.CountdownTime = _timeToCharge;
            _cooldownTimer.Reset();
        }
        if (!IsTargeting)
        {
            Idle();
        }
        else
        {
            AttackTarget();
        }
    }

    public AttackData GiveDamage()
    {
        _playerAttack.ID = UnityEngine.Random.Range(0, 10000);
        return _playerAttack;
    }

    private void Idle()
    {
        _baseChain.weight = 0f;
        _tipChain.weight = 0f;
        CancelInvoke();
    }

    private void AttackTarget()
    {

        if (_target != null)
        {
            _followTransform.position = _target.position;
        }
        if (_canCharge)
        {

            if (!_lightningCharge.isPlaying)
            {
                PlayChargeEffects();
            }
            if (_attackTimer.Tick(Time.deltaTime))
            {
                Fire();
            }
            else
            {
                _chargePower = Mathf.InverseLerp(_attackTimer.CountdownTime, 0, _attackTimer.CurrentTime);
                _instance.setParameterByName("ChargeShotLevel", _chargePower);
            }

        }
        else if (_cooldownTimer.Tick(Time.deltaTime))
        {
            _canCharge = true;
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
        _canCharge = false;
        StopChargeEffects();
    }

    private void StopChargeEffects()
    {
        _lightningCharge.Stop();
        _backCharge.Stop();
    }

    public void TakeDamage(AttackData attack)
    {
        if (_previousAttackIDs.Contains(attack.ID))
        {

        }
        else
        {
            TurretHealth -= attack.Damage;
            _healthBar.UpdateBar((float)TurretHealth, 0f, (float)_turretMaxHealth, true);
            EffectManager.Instance.GenerateFloatingText(transform.position, attack.Damage, this.transform);
            if (TurretHealth < 0)
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
                    body.AddExplosionForce(attack.Force, (attack.HitPosition - body.transform.position).normalized * 5, 20f);
                    Destroy(collider.gameObject, Random.Range(30f, 90f));
                }
                _target = null;
                Destroy(this.gameObject);
            }
            else if (_previousAttackIDs.Count == 5)
            {
                _previousAttackIDs.Dequeue();
            }
            _previousAttackIDs.Enqueue(attack.ID);
        }
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void EnemyDetected(PlayerAbilityController abilityController)
    {
        _target = abilityController.PlayerTarget;
        _tipChain.weight = _tipChainStartWeight;
        _baseChain.weight = _baseChainStartWeight;
        InvokeRepeating("CheckDistance", 5f, 1f);
    }

    public void CheckDistance()
    {
        if (Vector3.Distance(transform.position, _target.position) > MaxAttackDistance)
        {
            EnterIdle();
        }
    }

    public void EnterIdle()
    {
        CancelInvoke();
        StopChargeEffects();
        _instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _target = null;
        Idle();
        _detectPlayer.BeginInvoke(this);
    }
}
