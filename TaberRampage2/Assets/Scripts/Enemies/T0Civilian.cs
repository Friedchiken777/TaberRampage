using UnityEngine;
using System.Collections;

public class T0Civilian : EnemyParentScript
{
    protected bool triggered = false;        

    // Use this for initialization
    void Start ()
    {
        currentSpeed = walkSpeed;
        GetDirectionToFace();
    }

    protected override void Move()
    {
        gameObject.GetComponent<CharacterController>().Move(direction * currentSpeed * Time.deltaTime);
    }

    protected override void MonsterInteraction()
    {
        if (!triggered)
        {
            if (Vector3.Distance(transform.position, monster.transform.position) < boldness)
            {
                triggered = true;
                float oldDirection = direction.x;
                Vector3 heading = monster.transform.position - transform.position;
                heading = new Vector3(-Mathf.Round(heading.x), 0, 0);
                if (heading.x != 0)
                {
                    float distance = heading.magnitude;
                    distance = Mathf.Round(distance);
                    direction = heading / distance;
                }
                if (Mathf.Sign(direction.x) > 0)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                currentSpeed = runSpeed;
            }
        }
    }    
}
