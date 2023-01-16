using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ScriptableObject
{
    public int BaseDamage;
    public int Interrupt;
    public float AttacksPerMinute;
    public float AttackDistance;
    public float Force;
    public float MoveSpeedModifier;
    public float PoisonProc;
    public float BleedProc;
    public float ZapProc;
    public float BounceProc;
    public float AOEProc;
    public float AOESize;

    public List<ParticleSystem> ParticleSystems;
    public List<FMODUnity.EventReference> AudioRef;
    public Mesh Mesh;
    public Sprite WeaponImage;
}
