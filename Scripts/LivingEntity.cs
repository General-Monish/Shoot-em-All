using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour,IDamagable // Interface
{
  [SerializeField]  public float startingHealth;
  [SerializeField]  protected float Health;
    protected bool dead;
    public event Action onDeath; // Event
    public void TakeHit(float damage, RaycastHit hit)
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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Health = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
