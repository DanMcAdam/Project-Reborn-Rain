using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/OnTimer/Explosion")]

public class ExplosionOnTimer : BaseItem
{
    public AreaOfEffect AOE;
    public int Damage;
    public float size;
    public float Force;
    public override void OnTimerCooldown(PlayerStats player)
    {
        player.SetOffPooledEvent(this);
    }
}
