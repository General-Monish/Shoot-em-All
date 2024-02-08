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

    NavMeshAgent pathFinder;
    Transform target;
    Material enemySkinMaterial;

    Color originalColor;

    float attackDistanceThreashold = .5f;
    float timeBetweenAttack = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;
    // Start is called before the first frame update
  protected override void Start()
    {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();
        enemySkinMaterial = GetComponent<Renderer>().material;
        originalColor = enemySkinMaterial.color;

        currentState = State.chasing;
        target = GameObject.FindGameObjectWithTag("Player").transform;

        myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextAttackTime)
        {
            float sqDistanceToTarget = (target.position - transform.position).sqrMagnitude;
            if (sqDistanceToTarget < Mathf.Pow(attackDistanceThreashold + myCollisionRadius + targetCollisionRadius, 2));
            nextAttackTime = Time.time + timeBetweenAttack;
            StartCoroutine(Attack());
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

        while (percent <= 1)
        {
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
        while (target != null)
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
