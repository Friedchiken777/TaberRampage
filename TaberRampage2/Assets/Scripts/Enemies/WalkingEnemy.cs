using UnityEngine;
using System.Collections;

public class WalkingEnemy : T0Civilian
{

    [SerializeField]
    Transform bulletSpawn, arm;
    Vector3 aimPoint;

    protected override void MonsterInteraction()
    {
        if (Vector3.Distance(monster.transform.position, transform.position) <= boldness)
        {
            triggered = true;
            currentSpeed = 0;
        }
        if (triggered && Vector3.Distance(monster.transform.position, transform.position) > boldness)
        {
            triggered = false;
            GetDirectionToFace();
            currentSpeed = walkSpeed;
        }
        if (triggered && Vector3.Distance(monster.transform.position, transform.position) <= boldness / 2)
        {
            GetDirectionToFace();
            currentSpeed = -walkSpeed;
        }

        aimPoint = monster.transform.position;

        //Rotate Arm
        if (triggered && arm != null)
        {
            Vector3 aimZ = new Vector3(transform.position.x, transform.position.y, aimPoint.z);
            Quaternion q = Quaternion.LookRotation(aimZ - transform.position);
            float s = Mathf.Min(0.5f * Time.deltaTime, 1);
            arm.rotation = Quaternion.Lerp(arm.rotation, q, s);
        }

        if (triggered && !hasShield)
        {
            shootTimer += Time.deltaTime;

            if (shootTimer >= fireRate)
            {
                shootTimer = 0;
                //RotateArm(aimPoint);
                GameObject bullet = null;
                if (bulletSpawn != null)
                {
                    bullet = Instantiate(bulletPrfab, bulletSpawn.position, bulletSpawn.rotation) as GameObject;
                }
                else
                {
                    bullet = Instantiate(bulletPrfab, transform.position, transform.rotation) as GameObject;
                }                
                bullet.GetComponent<Projectile>().SetTarget(new Vector3(Mathf.Sign(direction.x), 0, 0));
                if (hasShotgun)
                {
                    GameObject bulletu = null;
                    if (bulletSpawn != null)
                    {
                        bulletu = Instantiate(bulletPrfab, bulletSpawn.position, bulletSpawn.rotation) as GameObject;
                    }
                    else
                    {
                        bulletu = Instantiate(bulletPrfab, transform.position, transform.rotation) as GameObject;
                    }
                    aimPoint.y += SHOTSPREAD;
                    bulletu.GetComponent<Projectile>().SetTarget(new Vector3(Mathf.Sign(direction.x), 0, 0));
                    GameObject bulletd = null;
                    if (bulletSpawn != null)
                    {
                        bulletd = Instantiate(bulletPrfab, bulletSpawn.position, bulletSpawn.rotation) as GameObject;
                    }
                    else
                    {
                        bulletd = Instantiate(bulletPrfab, transform.position, transform.rotation) as GameObject;
                    }
                    aimPoint.y -= SHOTSPREAD * 2;
                    bulletd.GetComponent<Projectile>().SetTarget(new Vector3(Mathf.Sign(direction.x), 0, 0));
                }                
            }
        }

    }

    void RotateArm(Vector3 target)
    {
        arm.eulerAngles = (transform.position - target).normalized;        
    }
}
