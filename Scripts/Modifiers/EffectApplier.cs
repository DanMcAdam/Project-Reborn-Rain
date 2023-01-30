using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectApplier : MonoBehaviour
{
    PlayerStats _playerStats;
    PlayerInventory _playerInventory;

    private Dictionary<BaseItem, TimerScript> _countdownItems => _playerInventory.CountdownItems;
    private List<BaseItem> _itemInventory => _playerInventory.ItemInventory;
    private float _critChance => _playerStats.PlayerCritChance;
    private float _critMultiplier => _playerStats.PlayerCritMultiplier;

    private void Awake()
    {
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

    public void ApplyOnEventItems(ItemProperties property, float amount)
    {
        foreach (BaseItem item in _itemInventory)
        {
            if (item.ItemProperties.Contains(property))
            {
                item.OnHeal(_playerStats, amount);
            }
        }
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
}
