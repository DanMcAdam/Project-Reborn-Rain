using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/OnTimer/Explosion")]

public class ExplosionOnTimer : BaseItem, IExplodeObject
{
    public AreaOfEffect AOE;
    public int Damage;
    public float size;
    public float Force;

    public float Size => size;

    AreaOfEffect IExplodeObject.AOE => AOE;

    int IExplodeObject.Damage => Damage;

    float IExplodeObject.Force => Force;

    float IExplodeObject.Time => Time;

    public override void OnTimerCooldown(PlayerStats player)
    {
        player.EffectApplier.SetOffPooledEvent(this);
    }
}
