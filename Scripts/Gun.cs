using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto,Burst,Single};
    public FireMode fireMode;
    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShots;
    public float ShootVelocity;
    public int burstCount;

    public Transform shell;
    public Transform shellEjection;

    MuzzelFlash muzzelFlash;
    bool triggerReleasedSinceLastShot;
    int shotsRemaingInBurst;

    float nextShotTime;
    // Start is called before the first frame update

    private void Start()
    {
        muzzelFlash = GetComponent<MuzzelFlash>();
        shotsRemaingInBurst = burstCount;
    }
     void shoot()
    {
        if (Time.time > nextShotTime)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemaingInBurst == 0)
                {
                    return;
                }
                shotsRemaingInBurst--;
            }else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }
            for(int i=0; i < projectileSpawn.Length; i++)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(ShootVelocity);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzelFlash.Activate();
        }
    }

    public void OntriggerHold()
    {
        shoot();
        triggerReleasedSinceLastShot = false;
    }  
    public void OntriggerReleased()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemaingInBurst = burstCount;

    }
}
