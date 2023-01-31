using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackData
{
    public Vector3 HitPosition;
    public float CritChance;
    public float CritMultiplier;
    public int Damage;
    public float Force;
    public int ID;
    public bool GeneratedByPlayer;
    public bool WasCrit;
    public bool ShouldNotGenerateOnHitEffect;

    public AttackData(Vector3 hitPosition, int damage, float critChance, float critMultiplier, float force, int ID, bool generatedByPlayer)
    {
        HitPosition = hitPosition;
        Damage = damage;
        Force = force;
        this.ID = ID;
        GeneratedByPlayer = generatedByPlayer;
        CritChance= critChance;
        CritMultiplier = critMultiplier;
        WasCrit = false;
        ShouldNotGenerateOnHitEffect = false;
    }

    public AttackData(int damage, float force, int ID, bool generatedByPlayer)
    {
        HitPosition = Vector3.zero;
        Damage = damage;
        Force = force;
        this.ID = ID;
        GeneratedByPlayer = generatedByPlayer;
        CritChance = 0;
        CritMultiplier = 0;
        WasCrit = false;
        ShouldNotGenerateOnHitEffect = false;
    }

    public override string ToString()
    {
        return ("HitPosition = " + HitPosition + Environment.NewLine + "Damage = " + Damage + Environment.NewLine + "Force = " + Force);
    }
}
