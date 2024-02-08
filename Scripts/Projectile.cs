using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    float lifeTime=1.5f;
    float enemySkinWidth = .1f; // when enemy is moving rapidly then the bullet can pass through the collision and it wont give damage so making this so it detects b4 only
    public LayerMask collisionMask;
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
        Collider[] initialCollision = Physics.OverlapSphere(transform.position, 0.1f,collisionMask);
        if (initialCollision.Length > 0)
        {
            OnHitCollider(initialCollision[0]);
        }

    }
    
    
    // Update is called once per frame
    void Update()
    {
        float moveDistance = Time.deltaTime * speed;
        CheckCollsion(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollsion(float movedistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, movedistance + enemySkinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }

    }
    void OnHitObject(RaycastHit hit)
    {
        IDamagable damagableObject = hit.collider.GetComponent<IDamagable>();
        if (damagableObject != null)
        {
            damagableObject.TakeHit(damage, hit);
        }
        //Debug.Log(hit.collider.gameObject.name);
        GameObject.Destroy(gameObject);
    }

    void OnHitCollider(Collider collider)
    {
        // when enemy gets too close then the raycast pass through them without damage so made this method to avoid this problem
        IDamagable damagableObject = collider.GetComponent<IDamagable>();
        if (damagableObject != null)
        {
            damagableObject.TakeDamage(damage);
        }
        //Debug.Log(hit.collider.gameObject.name);
        GameObject.Destroy(gameObject);
    }
}
