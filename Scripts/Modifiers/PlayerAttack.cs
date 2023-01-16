using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerAttack
{
    public Vector3 HitPosition;
    public int Damage;
    public float Force;
    public int ID;
    public bool GeneratedByPlayer;

    public PlayerAttack(Vector3 hitPosition, int damage, float force, int ID, bool generatedByPlayer)
    {
        HitPosition = hitPosition;
        Damage = damage;
        Force = force;
        this.ID = ID;
        GeneratedByPlayer = generatedByPlayer;
    }

    public PlayerAttack(int damage, float force, int ID, bool generatedByPlayer)
    {
        HitPosition = Vector3.zero;
        Damage = damage;
        Force = force;
        this.ID = ID;
        GeneratedByPlayer = generatedByPlayer;
    }

    public override string ToString()
    {
        return ("HitPosition = " + HitPosition + Environment.NewLine + "Damage = " + Damage + Environment.NewLine + "Force = " + Force);
    }
}
