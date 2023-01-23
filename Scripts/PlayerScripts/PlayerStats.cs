using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int _playerHealth, _playerMaxHealth, _playerShield, _playerMaxShield, _playerBarrier, _playerMaxBarrier,  _playerJumpCount, _playerDashCount;
    [SerializeField]
    private float _playerSpeed, _playerJumpHeight, _playerBaseCritChance, _playerCritModifier, _playerDamageModifier, _playerShieldTime, _playerBarrierRechargeTime, _playerCritMultiplier;
    public int PlayerMaxHealth { get => _playerMaxHealth; }
    public int PlayerHealth { get => _playerHealth;  }
    public float PlayerSpeed { get => _playerSpeed; }
    public float PlayerJumpHeight { get => _playerJumpHeight; }
    public int PlayerJumpCount { get => _playerJumpCount; }
    public int PlayerDashCount { get => _playerDashCount; }

    public int PlayerShield { get => _playerShield; set { _playerMaxShield += value; _playerShield += value; _shieldQueue.Enqueue(new ShieldStack(_currentTime + _playerShieldTime, value)); } }

    public int PlayerBarrier 
    {   get => _playerBarrier;
        set { _playerBarrier -= value; } }

    public float PlayerCritChance { get { return _playerBaseCritChance + _playerCritModifier; } set { _playerCritModifier += value; } }

    public float PlayerCritMultiplier { get { return _playerCritMultiplier; } set => _playerCritMultiplier = value; }

    [Button]
    public void AddShield()
    {
        PlayerShield = 30;
    }

    private float _currentTime;
    private Queue<ShieldStack> _shieldQueue;

    private TimerScript _barrierRegenTimer;
    //called if non-shield damage is taken, restarting barrier regen
    private bool _justTookDamage;
    private bool _startRegen;
    void Start()
    {
        _playerHealth = _playerMaxHealth;
        _playerBarrier = _playerMaxBarrier;
        _barrierRegenTimer = new TimerScript(_playerBarrierRechargeTime);
        _shieldQueue= new Queue<ShieldStack>();
        _currentTime = 0;
    }


    void Update()
    {
        _currentTime += Time.deltaTime;
        HandleShield();
        HandleBarrier();
    }

    private void HandleBarrier()
    {
        if (_justTookDamage) { _barrierRegenTimer.Reset(); _startRegen = false; _justTookDamage = false; }
        if (_playerBarrier < _playerMaxBarrier)
        {
            if (!_startRegen && _barrierRegenTimer.Tick(Time.deltaTime))
            {
                _startRegen = true;
            }
            else if (_startRegen)
            {
                _playerBarrier++;
                if (_playerBarrier == _playerMaxBarrier)
                {
                    _startRegen = false;
                }
            }
        }
    }

    private void HandleShield()
    {
        if (_playerMaxShield > 0)
        {
            while (_shieldQueue.Count > 0)
            {
                if (_shieldQueue.TryPeek(out ShieldStack result))
                {
                    if (result.TimeToEnd < _currentTime)
                    {
                        _playerMaxShield -= result.ShieldingAmount;
                        if (_playerShield > _playerMaxShield) { _playerShield = _playerMaxShield; }
                        _shieldQueue.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        Debug.Log("I was hit by something! " + other.name);
        TurretBehavior turret = other.GetComponentInParent<TurretBehavior>();
        TakeDamage(turret.GiveDamage());

    }

    public void TakeDamage(AttackData attack)
    {
        if (!attack.GeneratedByPlayer)
        {
            int attackValue = attack.Damage;
            if (_playerShield > 0)
            {
                if (_playerShield >= attackValue){ _playerShield -= attackValue; attackValue = 0; }
                else { attackValue -= _playerShield; _playerShield = 0; }
            }
            if (attackValue <= 0) return;
            _justTookDamage = true;
            if (attackValue > 0 && _playerBarrier > 0)
            {
                if (PlayerBarrier > attackValue) { PlayerBarrier = attackValue; attackValue = 0; }
                else {  attackValue -= _playerBarrier; PlayerBarrier = _playerBarrier; }
            }
            if (attackValue > 0) { _playerHealth -= attackValue; }

            if (PlayerHealth < 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("Bummer, I'm dead.");
    }
}

public struct ShieldStack
{
    public float TimeToEnd;
    public int ShieldingAmount;

    public ShieldStack(float timeToEnd, int shieldingAmount)
    {
        TimeToEnd = timeToEnd;
        ShieldingAmount = shieldingAmount;
    }
}
