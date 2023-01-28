using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectApplier : MonoBehaviour
{
    PlayerStats _playerStats;

    private float _critChance => _playerStats.PlayerCritChance;
    private float _critMultiplier => _playerStats.PlayerCritMultiplier;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
    }
    public void ApplyAttackEffects(AttackData playerAttack, IDamageable damageable)
    {
        //apply modifiers here
        playerAttack = ApplyCritHit(playerAttack);
        damageable.TakeDamage(playerAttack);
    }

    private AttackData ApplyCritHit(AttackData playerAttack)
    {
        float randomValue = Random.value;
        if (randomValue <= (_critChance + playerAttack.CritChance))
        {
            int damage = playerAttack.Damage + Mathf.CeilToInt(playerAttack.Damage * (_critMultiplier + playerAttack.CritMultiplier));
            playerAttack.Damage = damage;
            playerAttack.WasCrit = true;
        }
        return playerAttack;
    }
}
