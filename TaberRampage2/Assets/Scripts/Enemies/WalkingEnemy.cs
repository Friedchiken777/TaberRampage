using UnityEngine;
using System.Collections;

public class WalkingEnemy : T0Civilian
{   

    protected override void MonsterInteraction()
    {
        
        if (!triggered && Vector3.Distance(monster.transform.position, transform.position) <= boldness)
        {
            GetDirectionToFace("Stop");
            triggered = true;
            if (hasAnimator)
            {
                animator.SetBool("Triggered", true);
            }
            currentSpeed = 0;
        }
        if (triggered && Vector3.Distance(monster.transform.position, transform.position) > boldness)
        {
            GetDirectionToFace("Forward");
            triggered = false;
            if (hasAnimator)
            {
                animator.SetBool("Triggered", false);
                animator.SetBool("Fire", false);
            }            
            currentSpeed = walkSpeed;
        }
        if (triggered && Vector3.Distance(monster.transform.position, transform.position) <= boldness / 2)
        {
            GetDirectionToFace("Backward");
            currentSpeed = -walkSpeed;
        }

        aimPoint = monster.transform.position;

        //Rotate Arm
        if (triggered && (aimArm != null))
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
