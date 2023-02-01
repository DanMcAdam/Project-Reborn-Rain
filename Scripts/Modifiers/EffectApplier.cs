using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectApplier : MonoBehaviour
{
    private PlayerStats _playerStats;
    private PlayerInventory _playerInventory;
    private PlayerAbilityController _abilityController;
    private Dictionary<BaseItem, TimerScript> _countdownItems => _playerInventory.CountdownItems;
    private List<BaseItem> _itemInventory => _playerInventory.ItemInventory;
    private float _critChance => _playerStats.PlayerCritChance;
    private float _critMultiplier => _playerStats.PlayerCritMultiplier;

    private void Awake()
    {
        _abilityController = GetComponent<PlayerAbilityController>();
        _playerStats = GetComponent<PlayerStats>();
        _playerInventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        HandleCooldownItems();
    }

    private void HandleCooldownItems()
    {
        foreach (var pair in _countdownItems)
        {
            if (pair.Value.Tick(Time.deltaTime))
            {
                pair.Key.OnTimerCooldown(_playerStats);
            }
        }
    }

    public AttackData ApplyOnEventItems(ItemProperties property, AttackData attackData)
    {
        if (attackData.ShouldNotGenerateOnHitEffect == true) return attackData;
        foreach (BaseItem item in _itemInventory)
        {
            switch (property)
            {
                case ItemProperties.OnHit:
                    if (item.ItemProperties.Contains(property))
                    {
                        attackData = item.OnHit(_playerStats, attackData);
                    }
                    break;
                case ItemProperties.OnCrit:
                    if (item.ItemProperties.Contains(property))
                    {
                        attackData = item.OnCrit(_playerStats, attackData);
                    }
                    break;
                case ItemProperties.OnTakeDamage:
                    if (item.ItemProperties.Contains(property))
                    {
                        attackData = item.OnTakeDamage(_playerStats, attackData);
                    }
                    break;
                case ItemProperties.OnTakeHealthDamage:
                    if (item.ItemProperties.Contains(property))
                    {
                        attackData = item.OnTakeHealthDamage(_playerStats, attackData);
                    }
                    break;

                default:
                    Debug.LogError(($"AttackData ApplyOnEventItems(ItemProperties property, AttackData attackData) received the wrong input, input recieved is {property}"));
                    break;
            }
        }
        return attackData;
    }

    public void ApplyOnEventItems(ItemProperties property)
    {
        foreach (BaseItem item in _itemInventory)
        {
            switch (property)
            {
                case ItemProperties.OnShieldBreak:
                    if (item.ItemProperties.Contains(property))
                    {
                        item.OnShieldBreak(_playerStats);
                    }
                    break;
                case ItemProperties.OnBarrierBreak:
                    if (item.ItemProperties.Contains(property))
                    {
                        item.OnBarrierBreak(_playerStats);
                    }
                    break;
                case ItemProperties.OnJump:
                    if (item.ItemProperties.Contains(property))
                    {
                        item.OnJump(_playerStats);
                    }
                    break;
                case ItemProperties.OnDash:
                    if (item.ItemProperties.Contains(property))
                    {
                        item.OnDash(_playerStats);
                    }
                    break;
                case ItemProperties.OnPrimaryAttack:
                    if (item.ItemProperties.Contains(property))
                    {
                        item.OnPrimaryAttack(_playerStats);
                    }
                    break;
                case ItemProperties.OnSecondaryAttack:
                    if (item.ItemProperties.Contains(property))
                    {
                        item.OnSecondaryAttack(_playerStats);
                    }
                    break;
                case ItemProperties.OnPrimaryAbility:
                    if (item.ItemProperties.Contains(property))
                    {
                        item.OnPrimaryAbility(_playerStats);
                    }
                    break;
                case ItemProperties.OnSecondaryAbility:
                    if (item.ItemProperties.Contains(property))
                    {
                        item.OnSecondaryAbility(_playerStats);
                    }
                    break;
                default: Debug.LogError(($"ApplyOnEventItems(ItemProperties property) received the wrong input, input recieved is {property}")); break;
            }
        }
    }

    public int ApplyOnEventItems(ItemProperties property, int amount)
    {
        foreach (BaseItem item in _itemInventory)
        {
            if (item.ItemProperties.Contains(property))
            {
                amount = item.OnHeal(_playerStats, amount);
            }
        }
        return amount;
    }

    public void ApplyOnEventItems(ItemProperties property, BaseEnemy enemy)
    {
        foreach (BaseItem item in _itemInventory)
        {
            if (item.ItemProperties.Contains(property))
            {
                item.OnEnemyKilled(_playerStats, enemy);
            }
        }
    }

    public void ApplyAttackEffects(AttackData playerAttack, IDamageable damageable)
    {
        playerAttack = ApplyOnEventItems(ItemProperties.OnHit, playerAttack);
        playerAttack = ApplyCritHit(playerAttack);
        damageable.TakeDamage(playerAttack);
    }

    private AttackData ApplyCritHit(AttackData playerAttack)
    {
        float randomValue = Random.value;
        if (randomValue <= (_critChance + playerAttack.CritChance))
        {
            playerAttack = ApplyOnEventItems(ItemProperties.OnCrit, playerAttack);
            int damage = playerAttack.Damage + Mathf.CeilToInt(playerAttack.Damage * (_critMultiplier + playerAttack.CritMultiplier));
            playerAttack.Damage = damage;
            playerAttack.WasCrit = true;
        }
        return playerAttack;
    }
    
    private Dictionary<IExplodeObject, MMMiniObjectPooler> _explosionDictionary;

    public void SetOffPooledEvent(IExplodeObject item)
    {
        //TODO create script to handle all player object pools and refactor this method into it
        //set off by: ExplosionOnTimer Scriptable Object

        AreaOfEffect aoeObj;
        aoeObj = ReturnPooledExplosion(item);
        AttackData attackData = new AttackData();
        attackData.Damage = item.Damage;
        attackData.GeneratedByPlayer = true;
        attackData.Force = item.Force;
        attackData.HitPosition = transform.position;
        attackData = IDGenerator.GenerateID(attackData);
        attackData.ShouldNotGenerateOnHitEffect = true;
        aoeObj.PlayerAttack = attackData;
        aoeObj.AbilityController = _abilityController;

        aoeObj.AOE = item.Size;
        if (!aoeObj.ParticlesInstantiated) aoeObj.SetupParticles();
        aoeObj.transform.position = transform.position;
        aoeObj.gameObject.SetActive(true);
        aoeObj.SetOff();
    }
    public void SetOffPooledEvent(IExplodeObject item, Vector3 location)
    {
        //TODO create script to handle all player object pools and refactor this method into it
        //set off by: ExplosionOnTimer Scriptable Object

        AreaOfEffect aoeObj;
        aoeObj = ReturnPooledExplosion(item);
        AttackData attackData = new AttackData();
        attackData.Damage = item.Damage;
        attackData.GeneratedByPlayer = true;
        attackData.Force = item.Force;
        attackData.HitPosition = location;
        attackData = IDGenerator.GenerateID(attackData);
        attackData.ShouldNotGenerateOnHitEffect = true;
        aoeObj.PlayerAttack = attackData;
        aoeObj.AbilityController = _abilityController;

        aoeObj.AOE = item.Size;
        if (!aoeObj.ParticlesInstantiated) aoeObj.SetupParticles();
        aoeObj.transform.position = location;
        aoeObj.gameObject.SetActive(true);
        aoeObj.SetOff();
    }

    private AreaOfEffect ReturnPooledExplosion(IExplodeObject item)
    {
        AreaOfEffect aoeObj;
        if (_explosionDictionary == null) _explosionDictionary = new Dictionary<IExplodeObject, MMMiniObjectPooler>();

        if (_explosionDictionary.TryGetValue(item, out MMMiniObjectPooler pool))
        {
            aoeObj = pool.GetPooledGameObject().GetComponent<AreaOfEffect>();
        }
        else
        {
            Debug.Log("Adding explosion pooler");
            MMMiniObjectPooler explosionPool = _abilityController.ObjectPoolerObject.AddComponent<MMMiniObjectPooler>();
            Debug.Log("explosion pooler added");
            explosionPool.NestWaitingPool = true;
            explosionPool.PoolSize = 2;
            explosionPool.GameObjectToPool = item.AOE.gameObject;
            explosionPool.FillObjectPool();
            _explosionDictionary.Add(item, explosionPool);
            aoeObj = explosionPool.GetPooledGameObject().GetComponent<AreaOfEffect>();
        }

        return aoeObj;
    }
}
