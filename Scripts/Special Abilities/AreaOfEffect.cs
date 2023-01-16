using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    public PlayerAttack PlayerAttack;
    public float AOE;

    public bool ParticlesInstantiated;
    public ParticleSystem Vfx;
    public PlayerAbilityController AbilityController;

    public AreaOfEffect(PlayerAbilityController abilityController, PlayerAttack playerAttack, ParticleSystem vfx, float aOE)
    {
        PlayerAttack = playerAttack;
        AOE = aOE;
        Vfx = Instantiate(Vfx, PlayerAttack.HitPosition, Quaternion.identity);
        Vfx.Stop();
        AbilityController = abilityController;
        ParticlesInstantiated = true;
    }
    
    public AreaOfEffect()
    {

    }

    public void SetupParticles(ParticleSystem particles)
    {
        Vfx = Instantiate(particles, PlayerAttack.HitPosition, Quaternion.identity);
        Vfx.Stop();
        ParticlesInstantiated = true;
    }

    public void SetOff()
    {
        Vfx.transform.position = PlayerAttack.HitPosition;
        ParticleSystem.MainModule main = Vfx.main;
        main.startSizeMultiplier = AOE;
        foreach (ParticleSystem subPart in Vfx.GetComponentsInChildren<ParticleSystem>())
        {
            if (subPart.name == "Shockwave")
            {
                main = subPart.main;
                main.startSizeMultiplier = AOE; 
            }
            else
            {

            }
        }
        Vfx.Play();
        Collider[] colliders = Physics.OverlapSphere(PlayerAttack.HitPosition, AOE);
        foreach (Collider collider in colliders)
        {

            if (collider.TryGetComponent(out IDamageable damageable) && collider.transform != this.transform)
            {
                AbilityController.Effect.ApplyAttackEffects(PlayerAttack, damageable);
            }
            else
            {
                damageable = collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    AbilityController.Effect.ApplyAttackEffects(PlayerAttack, damageable);
                }
            }
        }
        Invoke("Disable", 1);
    }

    private void Disable()
    {
        this.gameObject.SetActive(false);
    }
}
