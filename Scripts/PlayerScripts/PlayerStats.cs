using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerStats : MonoBehaviour, IDamageable
{
    #region Private Variables
    [TitleGroup("Player Stats")]
    [SerializeField]
    [TabGroup("Player Stats/TabGroup", "Health and Shields")]
    [TabGroup("Player Stats/TabGroup/Health and Shields/HealthGroup", "Health")]
    private int _playerHealth, _playerMaxHealth;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup/Health and Shields/HealthGroup", "Barrier")]
    private int _playerBarrier, _playerMaxBarrier;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup/Health and Shields/HealthGroup", "Shield")]
    private int _playerShield, _playerMaxShield;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup/Health and Shields/HealthGroup", "Shield")]
    private float _playerShieldTime;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup/Health and Shields/HealthGroup", "Barrier")]
    private float _playerBarrierRechargeTime;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup", "Movement")]
    [TabGroup("Player Stats/TabGroup/Movement/MovementGroup", "Jump")]
    private int _playerJumpCount;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup/Movement/MovementGroup", "Jump")]
    private float _playerJumpHeight;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup/Movement/MovementGroup", "Dash")]
    private int _playerDashCount, _playerMaxDashCount;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup/Movement/MovementGroup", "Dash")]
    private float _playerDashTime, _playerDashSpeed, _playerDashCooldown;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup/Movement/MovementGroup", "Dash")]
    private float _playerSpeed;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup", "Damage")]
    [TabGroup("Player Stats/TabGroup/Damage/DamageGroup", "Crit")]
    private float _playerBaseCritChance, _playerCritModifier, _playerCritMultiplier;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup/Damage/DamageGroup", "Damage")]
    private float _playerDamageModifier;
    [SerializeField]
    [TabGroup("Player Stats/TabGroup/Damage/DamageGroup", "Damage")]
    private Gradient _shieldColor, _barrierColor, _healthColor;
    #endregion
    #region Exposed Properties
    public int PlayerMaxHealth { get => _playerMaxHealth; }
    public int PlayerHealth { get => _playerHealth; }
    public float PlayerSpeed { get => _playerSpeed; set => _playerSpeed += value; }
    public float PlayerJumpHeight { get => _playerJumpHeight; set => _playerJumpHeight += value; }
    public int PlayerJumpCount { get => _playerJumpCount; }
    public int PlayerDashCount { get => _playerDashCount; }

    public int PlayerShield { get => _playerShield; }

    public int PlayerBarrier
    {
        get => _playerBarrier;
        set { _playerBarrier -= value; }
    }

    public float PlayerCritChance { get { return _playerBaseCritChance + _playerCritModifier; } set { _playerCritModifier += value; } }

    public float PlayerCritMultiplier { get { return _playerCritMultiplier; } set => _playerCritMultiplier = value; }

    public float PlayerDashTime { get => _playerDashTime; set => _playerDashTime += value; }
    public float PlayerDashSpeed { get => _playerDashSpeed; set => _playerDashSpeed += value; }
    public float PlayerDashCooldown { get => _playerDashCooldown; set => _playerDashCooldown += value; }
    public int PlayerMaxDashCount { get => _playerMaxDashCount; set => _playerMaxDashCount += value; }
    #endregion

    public PlayerAbilityController AbilityController;
    public EffectApplier EffectApplier;
    [SerializeField]
    [Button]
    private void AddShield()
    {
        AddShield(30);
    }

    private float _currentTime;
    private Queue<ShieldStack> _shieldQueue;

    private TimerScript _barrierRegenTimer;
    //called if non-shield damage is taken, restarting barrier regen
    private bool _justTookDamage;
    private bool _startRegen;

    void Start()
    {
        AbilityController = GetComponent<PlayerAbilityController>();
        EffectApplier = AbilityController.Effect;
        _playerHealth = _playerMaxHealth;
        _playerBarrier = _playerMaxBarrier;
        _barrierRegenTimer = new TimerScript(_playerBarrierRechargeTime);
        _shieldQueue = new Queue<ShieldStack>();
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

    public Queue<int> _previousAttackIDs = new Queue<int>(5);

    public void TakeDamage(AttackData attack)
    {
        if (!attack.GeneratedByPlayer)
        {
            if (_previousAttackIDs.Contains(attack.ID))
            {

            }
            else
            {
                attack = EffectApplier.ApplyOnEventItems(ItemProperties.OnTakeDamage, attack);
                int attackValue = attack.Damage;
                if (_playerShield > 0)
                {
                    if (_playerShield >= attackValue) { _playerShield -= attackValue; attackValue = 0; }
                    else { attackValue -= _playerShield; _playerShield = 0; }
                }
                if (attackValue <= 0)
                {
                    EffectManager.Instance.PlayerTakeDamage(transform.position, attack.Damage, attack.Damage, transform, false, _shieldColor);
                    return;
                }
                _justTookDamage = true;
                if (attackValue > 0 && _playerBarrier > 0)
                {
                    if (PlayerBarrier > attackValue) { PlayerBarrier = attackValue; attackValue = 0; EffectManager.Instance.PlayerTakeDamage(transform.position, attack.Damage, attack.Damage, transform, false, _barrierColor); }
                    else { attackValue -= _playerBarrier; PlayerBarrier = _playerBarrier; EffectApplier.ApplyOnEventItems(ItemProperties.OnBarrierBreak); }
                }
                if (attackValue > 0)
                {
                    attack = EffectApplier.ApplyOnEventItems(ItemProperties.OnTakeHealthDamage, attack);
                    _playerHealth -= attackValue;
                    EffectManager.Instance.PlayerTakeDamage(transform.position, attack.Damage, attack.Damage, transform, true, _healthColor);
                }

                if (PlayerHealth < 0)
                {
                    Die();
                }
                else if (_previousAttackIDs.Count == 5)
                {
                    _previousAttackIDs.Dequeue();
                }
                _previousAttackIDs.Enqueue(attack.ID);
            }
        }
    }

    public void HealPlayer(int amount)
    {
        amount = EffectApplier.ApplyOnEventItems(ItemProperties.OnHeal, amount);
    }

    public void AddShield(int amount)
    {
        _playerMaxShield += amount;
        _playerShield += amount;
        _shieldQueue.Enqueue(new ShieldStack(_currentTime + _playerShieldTime, amount));
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
