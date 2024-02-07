using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform shotPoint;
    public Projectile projectile;
    public float msBetweenShots;
    public float ShootVelocity;

    float nextShotTime;
    // Start is called before the first frame update
 public void shoot()
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, shotPoint.position, shotPoint.rotation) as Projectile;
            newProjectile.SetSpeed(ShootVelocity);
        }
    }
}
