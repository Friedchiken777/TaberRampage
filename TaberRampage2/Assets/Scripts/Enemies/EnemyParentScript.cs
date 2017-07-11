using UnityEngine;
using System.Collections;

public class EnemyParentScript : MonoBehaviour
{
    protected const float CLEANUPDISTANCE = 60;
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

    protected bool statNumbers;

    public EnemySpawnTypes spawnType;

    bool hasSpriteRenderer;

    Vector3 startingValues;

    protected Animator animator;
    protected bool hasAnimator;

    // Use this for initialization
    protected void Awake ()
    {
        monster = GameObject.FindObjectOfType<MonsterController>();
        currentHealth = maxHealth;
        statNumbers = (StatisticsNumbers.instance != null);
        hasSpriteRenderer = GetComponent<SpriteRenderer>() != null;
        startingValues = transform.localScale;
        SetAnimator();
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
                    if (statNumbers)
                    {
                        StatisticsNumbers.instance.ModifyTotalPeopleEaten(1);
                        if (this.GetComponent<T0Civilian>() != null)
                        {
                            if (this.GetComponent<T0Civilian>().actualCivilian)
                            {
                                StatisticsNumbers.instance.ModifyCivilliansEaten(1);
                            }
                        }
                        statNumbers = false;
                    }
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

    protected bool GetDirectionToFace(string debug)
    {
        StartCoroutine(DirectionChangeWithDelay(debug));
        return switched;
    }

    IEnumerator DirectionChangeWithDelay(string debug)
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
            print(debug + " " + direction);
            if (direction.x != lastXDirection)
            {
                switched = true;

                lastXDirection = direction.x;
                //Face enemy in right direction
                yield return new WaitForSeconds(turnDelay);
                if (Mathf.Sign(direction.x) >= 0)
                {
                    if (hasSpriteRenderer)
                    {
                        GetComponent<SpriteRenderer>().flipX = true;
                    }
                    else
                    {
                        transform.localScale = new Vector3(startingValues.x, startingValues.y, startingValues.z);
                        print("positive "+transform.localScale);
                    }
                }
                else
                {
                    if (hasSpriteRenderer)
                    {
                        GetComponent<SpriteRenderer>().flipX = false;
                    }
                    else
                    {
                        transform.localScale = new Vector3(-startingValues.x, startingValues.y, startingValues.z);
                        print("negative " + transform.localScale);
                    }
                }
                print("START" + startingValues);
            }
        }
    }

    protected void SetAnimator()
    {
        if (this.GetComponentInChildren<Animator>() != null)
        {
            animator = this.gameObject.GetComponentInChildren<Animator>();
            hasAnimator = true;
        }
        else
        {
            hasAnimator = false;
            print(gameObject.name + ": No animator Found...");
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
