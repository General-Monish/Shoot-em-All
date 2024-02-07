using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    NavMeshAgent pathFinder;
    Transform target;
    // Start is called before the first frame update
  protected override void Start()
    {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;
        while (target != null)
        {
            Vector3 targetPos = new Vector3(target.position.x, 0, target.position.z);

            if (!dead)
            {
                pathFinder.SetDestination(targetPos);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
