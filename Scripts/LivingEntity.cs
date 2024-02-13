using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamagable // Interface
{
    [SerializeField] public float startingHealth;
    [SerializeField] protected float Health;
    protected bool dead;
    public event Action onDeath; // Event

    protected virtual void Start()
    {
        Health = startingHealth;
    }
    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }





public virtual void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0 && !dead)
        {
            die();
        }
    }
    private void die()
    {
        dead = true;
        if (onDeath != null)
        {
            onDeath();
        }
        GameObject.Destroy(gameObject);
    }
}