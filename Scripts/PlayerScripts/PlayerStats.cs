using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int _playerMaxHealth, _playerJumpCount, _playerDashCount;
    [SerializeField]
    private float _playerSpeed, _playerJumpHeight;
    public int PlayerMaxHealth { get => _playerMaxHealth; }
    public int PlayerHealth { get; private set; }
    public float PlayerSpeed { get => _playerSpeed; }
    public float PlayerJumpHeight { get => _playerJumpHeight; }
    public int PlayerJumpCount { get => _playerJumpCount; }
    public int PlayerDashCount { get => _playerDashCount; }

    void Start()
    {
        PlayerHealth = _playerMaxHealth;
    }


    void Update()
    {

    }

    private void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        Debug.Log("I was hit by something! " + other.name);
        TurretBehavior turret = other.GetComponentInParent<TurretBehavior>();
        TakeDamage(turret.GiveDamage());

    }

    public void TakeDamage(PlayerAttack attack)
    {
        if (!attack.GeneratedByPlayer)
        {
            PlayerHealth -= attack.Damage;
            Debug.Log("took " + attack.Damage + " damage");
            if (PlayerHealth < 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("Bummer, I'm dead.");
    }
}
