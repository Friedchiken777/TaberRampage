using UnityEngine;
using System.Collections;

public class WindowEnemy : EnemyParentScript
{

    protected override void MonsterInteraction()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= fireRate)
        {
            GetDirectionToFace();
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
