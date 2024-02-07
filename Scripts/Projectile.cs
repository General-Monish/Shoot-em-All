using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    public LayerMask collisionMask;
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    // Start is called before the first frame update
    void Start()
    {
        
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

        if (Physics.Raycast(ray, out hit, movedistance, collisionMask, QueryTriggerInteraction.Collide))
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
}
