using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectApplier : MonoBehaviour
{
    //add reference to player inventory

    public void ApplyAttackEffects(PlayerAttack playerAttack, IDamageable damageable)
    {
        //apply modifiers here
        playerAttack.Damage = playerAttack.Damage * 2;
        damageable.TakeDamage(playerAttack);
    }
}
