using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MoreMountains.Tools.MMPath;

public class BaseItem : ScriptableObject
{
    public ItemProperties[] ItemProperties;

    public string Name;
    public string Description;
    public Image Icon;
    public Rarity rarityLevel;
    public float Time;

    public virtual AttackData OnHit(PlayerStats player, AttackData data) { return data; }


    public virtual AttackData OnCrit(PlayerStats player, AttackData data) { return data; }


    public virtual void OnEnemyKilled(PlayerStats player, BaseEnemy enemy) {  }


    public virtual AttackData OnTakeDamage(PlayerStats player, AttackData data) { return data; }


    public virtual AttackData OnTakeHealthDamage(PlayerStats player, AttackData data) { return data; }


    public virtual int OnHeal(PlayerStats player, int amount) { return amount; }

    public virtual void OnShieldBreak(PlayerStats player) { }

    public virtual void OnBarrierBreak(PlayerStats player) { }

    public virtual void OnJump(PlayerStats player) { }

    public virtual void OnDash(PlayerStats player) { }

    public virtual void OnPrimaryAttack(PlayerStats player) { }

    public virtual void OnSecondaryAttack(PlayerStats player) { }

    public virtual void OnPrimaryAbility(PlayerStats player) { }

    public virtual void OnSecondaryAbility(PlayerStats player) { }

    public virtual void OnTimerCooldown(PlayerStats player) { }

    public virtual void StatModifier(PlayerStats player) { }

    public virtual void FinishEffect() { }
}

public enum Rarity
{
    White, 
    Green, 
    Blue, 
    Purple, 
    Orange
}

public enum ItemProperties
{
    OnHit,
    OnCrit,
    OnEnemyKilled,
    OnTakeDamage,
    OnTakeHealthDamage,
    OnHeal,
    OnShieldBreak, 
    OnBarrierBreak, 
    OnJump, 
    OnDash, 
    OnPrimaryAttack, 
    OnSecondaryAttack,
    OnPrimaryAbility, 
    OnSecondaryAbility,
    OnTimerCooldown,
    StatModifier,
}

public interface IExplodeObject
{
    public AreaOfEffect AOE { get; }
    public int Damage { get; }
    public float Size { get; }
    public float Force { get; }
    public float Time { get; }
}