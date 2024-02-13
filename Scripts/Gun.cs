using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform shotPoint;
    public Projectile projectile;
    public float msBetweenShots;
    public float ShootVelocity;

 public   Transform shell;
 public   Transform shellEjection;

    MuzzelFlash muzzelFlash;


    float nextShotTime;
    // Start is called before the first frame update

    private void Start()
    {
        muzzelFlash = GetComponent<MuzzelFlash>();
    }
    public void shoot()
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, shotPoint.position, shotPoint.rotation) as Projectile;
            newProjectile.SetSpeed(ShootVelocity);

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzelFlash.Activate();
        }
    }
}
