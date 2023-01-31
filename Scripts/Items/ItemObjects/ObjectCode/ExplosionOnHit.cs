using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/OnHit/Explosion")]
public class ExplosionOnHit : BaseItem, IExplodeObject
{

    public AreaOfEffect AOE;
    public int Damage;
    public float Size;
    public float Force;

    AreaOfEffect IExplodeObject.AOE => AOE;

    int IExplodeObject.Damage => Damage;

    float IExplodeObject.Size => Size;

    float IExplodeObject.Force => Force;

    float IExplodeObject.Time => Time;


    public override AttackData OnHit(PlayerStats player, AttackData data)
    {
        player.EffectApplier.SetOffPooledEvent(this, data.HitPosition);
        return data;
    }
}
