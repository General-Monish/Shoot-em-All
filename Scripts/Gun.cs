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
    public float gunRecoilSettleMoveTime =.1f;
    
    public bool isReloading;
    public float reloadTime = .3f;

    public AudioClip shootAudio;


    public Transform shell;
    public Transform shellEjection;
    public Vector2 gunKickBackMinMax=new Vector2(.05f,.01f);

    MuzzelFlash muzzelFlash;
    bool triggerReleasedSinceLastShot;
    int shotsRemaingInBurst;
    Vector3 recoilSmoothDampVel;
    //float recoilAngle;
    //float recoilRotatSmothDampVel;

    float nextShotTime;
    // Start is called before the first frame update

    private void Update()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVel, gunRecoilSettleMoveTime);
        
        //recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotatSmothDampVel, .1f);
        //transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

    }

    private void Start()
    {
        muzzelFlash = GetComponent<MuzzelFlash>();
        shotsRemaingInBurst = burstCount;
    }
     void shoot()
    {
        if (  Time.time > nextShotTime )
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
            transform.localPosition -= Vector3.forward * Random.Range(gunKickBackMinMax.x,gunKickBackMinMax.y);
            AudioManager.instance.PlaySound(shootAudio, transform.position);
            //recoilAngle += 20;
            //recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
        }
    }

    public void Reload()
    {
        StartCoroutine(AnimReload());
    }

    IEnumerator AnimReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float percent = 0;
        float reloadSpeed = 1 / reloadTime;
        float maxReloadAngle = 30;
        while (percent < 0)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadangle = Mathf.Lerp(0, maxReloadAngle, interpolation);
        }

        isReloading = false;
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
