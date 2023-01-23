using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Guns")]
public class Gun : Weapon
{
    public int MagazineSize;
    public float ReloadTime;
    public float ScreenShakeIntensity;

    public float CritChance;
    public float CritMultiplier;

    public int BasePiercing;

    public bool HitScan;

    public bool AOE { get => AreaOfEffect != null; }
    public AreaOfEffect AreaOfEffect;

    public bool Charge;
    public float MaxCharge;

}
