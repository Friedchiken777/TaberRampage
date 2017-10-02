using UnityEngine;
using System.Collections;

public class WindowEnemy : EnemyParentScript
{
    bool triggered;

    protected override void MonsterInteraction()
    {

        if (Vector3.Distance(monster.transform.position, transform.position) > boldness)
        {
            triggered = false;
            GetDirectionToFace("Forward");
            if (hasAnimator)
            {
                animator.SetBool("Fire", false);
            }
        }
        else
        {
            GetDirectionToFace("Forward");
            triggered = true;
        }

        aimPoint = monster.transform.position;

        //Rotate Arm
        if (aimArm != null)
        {
            Vector3 aimZ = new Vector3(aimPoint.x, aimPoint.y, armPosition.position.z);
            float sign;
            if (transform.localScale.x < 0)
            {
                sign = (aimPoint.y > armPosition.position.y) ? -1.0f : 1.0f;
                aimArm.angleAim = (Vector3.Angle(Vector3.right, (armPosition.position - aimZ)) * sign);
            }
            else
            {
                sign = (aimPoint.y < armPosition.position.y) ? -1.0f : 1.0f;
                aimArm.angleAim = (Vector3.Angle(Vector3.right, (aimZ - armPosition.position)) * sign);
            }


        }

        if (triggered)
        {
            shootTimer += Time.deltaTime;

            if (shootTimer >= fireRate)
            {
                if (hasAnimator)
                {
                    animator.SetBool("Fire", true);
                    //Fire called by animation
                }
                else
                {
                    Fire();
                }
            }
        }

    }
}
