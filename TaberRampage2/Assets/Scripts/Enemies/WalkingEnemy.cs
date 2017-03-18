using UnityEngine;
using System.Collections;

public class WalkingEnemy : T0Civilian
{

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

        if (triggered && !hasShield)
        {
            shootTimer += Time.deltaTime;

            if (shootTimer >= fireRate)
            {
                shootTimer = 0;
                Vector3 aimPoint = monster.transform.position;
                GameObject bullet = Instantiate(bulletPrfab, transform.position, transform.rotation) as GameObject;
                bullet.GetComponent<Projectile>().SetTarget(aimPoint);
                if (hasShotgun)
                {
                    GameObject bulletu = Instantiate(bulletPrfab, transform.position, transform.rotation) as GameObject;
                    aimPoint.y += SHOTSPREAD;
                    bulletu.GetComponent<Projectile>().SetTarget(aimPoint);
                    GameObject bulletd = Instantiate(bulletPrfab, transform.position, transform.rotation) as GameObject;
                    aimPoint.y -= SHOTSPREAD * 2;
                    bulletd.GetComponent<Projectile>().SetTarget(aimPoint);
                }                
            }
        }

    }
}
