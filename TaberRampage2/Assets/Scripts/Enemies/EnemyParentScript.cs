using UnityEngine;
using System.Collections;

public class EnemyParentScript : MonoBehaviour
{
    protected const float CLEANUPDISTANCE = 30;
    protected const float SHOTSPREAD = 1f;

    [SerializeField]
    protected float maxHealth, walkSpeed, runSpeed, boldness, foodValue, stunDuration, stunForce, turnDelay, terrorValue;

    [SerializeField]
    protected float fireRate;
    protected float shootTimer;

    [SerializeField]
    protected bool hasShield, hasShotgun;

    [SerializeField]
    protected GameObject bulletPrfab;

    protected MonsterController monster;
    protected float currentHealth, currentSpeed;
    protected Vector3 direction;

    protected float lastXDirection;
    protected bool switched;

    protected bool attacked;
    protected float attackTimer;

    public EnemySpawnTypes spawnType;

    // Use this for initialization
    protected void Awake ()
    {
        monster = GameObject.FindObjectOfType<MonsterController>();
        currentHealth = maxHealth;
    }
	
	// Update is called once per frame
	protected void Update ()
    {
        Move();

        MonsterInteraction();

        SelfDestruct();

        //delays time between attacks
        if (attacked)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > 0.1f)
            {
                attacked = false;
                attackTimer = 0;
            }
        }
        
    }

    protected void OnTriggerEnter(Collider col)
    {
        if (!attacked)
        {
            //print(col.gameObject.name);
            if (col.gameObject.GetComponent<MonsterController>() != null)
            {
                AnimationSetter.instance.SetAttackState();
                currentHealth--;
                if (hasShield && currentHealth > 0)
                {
                    col.GetComponent<MonsterController>().StunPlayerH(stunDuration, stunForce);
                }
                else if (currentHealth <= 0)
                {
                    col.GetComponent<MonsterController>().HealDamage(foodValue);
                    TerrorManager.instance.AddTerror(terrorValue);
                    Destroy(this.gameObject);
                }
            }
            attacked = true;
        }
    }

    protected void SelfDestruct()
    {
        if (transform.position.x < monster.transform.position.x - CLEANUPDISTANCE || transform.position.x > monster.transform.position.x + CLEANUPDISTANCE)
        {
            Destroy(this.gameObject);
        }
    }

    protected bool GetDirectionToFace()
    {
        StartCoroutine(DirectionChangeWithDelay());
        return switched;
    }

    IEnumerator DirectionChangeWithDelay()
    {
        switched = false;
        //determine which way to walk to get to monster
        Vector3 heading = monster.transform.position - transform.position;
        heading = new Vector3(Mathf.Round(heading.x), 0, 0);
        float distance = heading.magnitude;
        distance = Mathf.Round(distance);
        if (distance != 0)
        {
            direction = heading / distance;
            if (direction.x != lastXDirection)
            {
                switched = true;
            }
            lastXDirection = direction.x;
            //Face enemy in right direction
            yield return new WaitForSeconds(turnDelay);
            if (Mathf.Sign(direction.x) >= 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
    }

    protected virtual void Move()
    {
        //Overload in Children
    }

    protected virtual void MonsterInteraction()
    {
        //Overload in Children
    }

    public enum EnemySpawnTypes
    {
        Ground,
        Window,
        Roof,
        Sky
    }
}
