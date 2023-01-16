using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Jumpslam")]
public class BaseJumpSlamScriptableObject : BaseUpgradeScriptableObject
{
    public float RadiusChange;
    public float DamageMultiplier;
    public float RangeMultiplier;
    public int HopNumber;
}
