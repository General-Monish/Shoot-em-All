using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { idle,chasing,attacking};
    State currentState;
    public ParticleSystem enemyDeathEffect;
    NavMeshAgent pathFinder;
    Transform target;
    LivingEntity targetEntity;
    Material enemySkinMaterial;
    [SerializeField] private float Playerdamage;

    Color originalColor;

    float attackDistanceThreashold = .5f;
    float timeBetweenAttack = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;
    // Start is called before the first frame update
  protected override void Start()
    {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();
        enemySkinMaterial = GetComponent<Renderer>().material;
        originalColor = enemySkinMaterial.color;

        if ( GameObject.FindGameObjectWithTag("Player") != null) // target exist
        {
            currentState = State.chasing;
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform; // finding target pos
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.onDeath += TargetEntity_onDeath; // death event

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
     
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= Health)
        {
          Destroy(Instantiate(enemyDeathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject,enemyDeathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    private void TargetEntity_onDeath()
    {
        hasTarget = false;
        currentState = State.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqDistanceToTarget = (target.position - transform.position).sqrMagnitude; //squared distance 
                if (sqDistanceToTarget < Mathf.Pow(attackDistanceThreashold + myCollisionRadius + targetCollisionRadius, 2)) // sum of squared dist+ collision
                {
                    nextAttackTime = Time.time + timeBetweenAttack;
                    StartCoroutine(Attack());
                }

            }
        }
    
    }

    IEnumerator Attack()
    {
        currentState = State.attacking;
        pathFinder.enabled = false;

        Vector3 originalPos = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPos = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        enemySkinMaterial.color = Color.blue;
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage) // when reached 50% close to target then give damage to target
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(Playerdamage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);

            yield return null;
        }
        enemySkinMaterial.color = originalColor;
        currentState = State.chasing;
        pathFinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;
        while (hasTarget)
        {
            if (currentState == State.chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPos = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreashold / 2);

                if (!dead)
                {
                    pathFinder.SetDestination(targetPos);
                }
            }
            

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
