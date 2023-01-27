using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int _health;

    private Rigidbody _rgbd;

    private void Awake()
    {
        _rgbd = GetComponent<Rigidbody>();
    }

    public void TakeDamage(AttackData attack)
    {
        //Debug.Log("Player did " + attack.Damage + " Damage");
        _rgbd.AddExplosionForce(attack.Force, attack.HitPosition, 5f);
        _health -= attack.Damage;
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
