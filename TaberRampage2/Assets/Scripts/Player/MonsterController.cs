using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

[RequireComponent(typeof(CharacterController))]

public class MonsterController : MonoBehaviour
{ 
    const float GRAVITY = -500;                                     //Gravity
    const float DASHTIME = 0.75f;                                   //How long a Dash lasts
    const float UNTOUCHABLETIME = 0.5f;
    const float NUMBEROFFLASHES = 8;
    const float FLASHTIME = UNTOUCHABLETIME / NUMBEROFFLASHES;
    const float STUNTIME = 0.35f;
    const float DASHPOWERMODIFIER = 3f;

    CharacterController cc;                                         //reference to CharacterController
    Transform leftBottom, rightBottom, centerBottom, centerBottomBuilding;      //Location references for the left, right, bottom and bottom aligned with buildings of the character

    [SerializeField]
    float maxHealth, currentHealth;                                 //Max and Current health;

    [SerializeField]
    float baseWalkSpeed, baseClimbSpeed, baseDashSpeed;             //Base Movement speeds

    public float walkSpeed, climbSpeed, fallspeed, dashSpeed;       //Final (if modified) movement speeds
    float dashModifier;
    float dashTimer, dashPower;                                     //Holds Dash time passed
    float dashTerminalV;                                            //Holds horizontal speed when dash ends
    public bool dashFall, stunFall;                                                  //keeps track of end of dash existance, keeps track of if fall was from stun

    Vector3 swipeDirection;                                         //Direction of a swipe

    bool monsterTapped;                                          //detects when monster tapped

    bool canClimb;                                                  //true when in front of buildings

    bool ghostTouch;

    public Vector2 movmentTouchInput;
    int faceDirection;                                              //1 if facing to the right, -1 if facing to the left
    bool turnAround, knockedBack;

    float horizontalMove = 0;
    float verticalMove = 0;

    float ccHeight;

    void Start ()
    {
        cc = this.GetComponent<CharacterController>();
        leftBottom = transform.FindChild("LeftBottom");
        rightBottom = transform.FindChild("RightBottom");
        centerBottom = transform.FindChild("CenterBottom");
        centerBottomBuilding = transform.FindChild("CenterBottomBuilding");
        climbSpeed = baseClimbSpeed;
        walkSpeed = baseWalkSpeed;
        dashSpeed = baseDashSpeed;
        currentHealth = maxHealth;
        movmentTouchInput = Vector2.zero;
        faceDirection = 1;
        turnAround = true;
        dashModifier = 1f;
        ccHeight = cc.height;
	}

	void FixedUpdate ()
    {
        SetState();    

        Movment();

        if (monsterTapped && !stunFall)
        {
            RaycastHit hitScene;
            if (Physics.Raycast(transform.position, Vector3.forward, out hitScene, 10))
            {
                if (hitScene.collider.gameObject.GetComponent<BuildingChunk>())
                {
                    hitScene.collider.gameObject.GetComponent<BuildingChunk>().TakeDamage(true);
                }
            }
        }

        monsterTapped = false;

        /*if (Input.GetKeyDown(KeyCode.M))
        {
            print(AnimationSetter.instance.GetCurrentAnimationObject().name);
        }*/
    }

    public void Movment()
    {
        horizontalMove = 0;
        verticalMove = 0;

        //Not Dashing or stuned
        if (AnimationSetter.instance.state != MonsterState.Dash && AnimationSetter.instance.state != MonsterState.Stun)
        {
            transform.rotation = Quaternion.identity;
            horizontalMove = movmentTouchInput.x * walkSpeed * Time.deltaTime;

            //second half of dash followthrough
            if (dashFall)
            {
                horizontalMove = dashTerminalV + movmentTouchInput.x * walkSpeed * Time.deltaTime;
                RaycastHit hitScene;
                if (Physics.Raycast(centerBottom.position, Vector3.forward, out hitScene, 10))
                {
                    //infront of building
                    if (hitScene.collider.gameObject.GetComponent<BuildingChunk>())
                    {
                        /*
                        //damage buildings while falling
                        if (!ghostTouch && !stunFall)
                        {
                            hitScene.collider.gameObject.GetComponent<BuildingChunk>().TakeDamage(false);
                        }
                        */

                        //Check if infront of building top
                        if (!hitScene.collider.gameObject.GetComponent<BuildingChunk>().isSky)
                        {
                            if (monsterTapped)
                            {
                                ClimbIfCan();
                            }
                        }
                    }
                }
            }

            //climb Buildings
            if (canClimb)
            {
                verticalMove = movmentTouchInput.y * walkSpeed * Time.deltaTime;
                if (verticalMove > 0 && AnimationSetter.instance.state == MonsterState.GroundWalk)
                {
                    ClimbIfCan();
                }                
            }

            //reset variables when on ground
            if (AnimationSetter.instance.state == MonsterState.GroundWalk)
            {
                fallspeed = 0;
                dashFall = false;
                stunFall = false;
            }

            //climb down from roof
            if (AnimationSetter.instance.state == MonsterState.RoofWalk)
            {
                if (movmentTouchInput.y < 0)
                {
                    verticalMove = movmentTouchInput.y * walkSpeed * Time.deltaTime;
                    canClimb = true;
                    AnimationSetter.instance.state = MonsterState.Climb;
                }
                dashFall = false;
                stunFall = false;
            }            
        }
        //Dashing
        else
        {
            dashTimer += (Time.deltaTime / dashPower) * DASHPOWERMODIFIER;
            float zRot = (Mathf.Atan2(swipeDirection.x, -swipeDirection.y) * Mathf.Rad2Deg) - 90;
            this.transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0,0, zRot), (dashTimer/DASHTIME) * 100);
            cc.height = 1;
            if (dashTimer <= DASHTIME)
            {
                horizontalMove = dashModifier * swipeDirection.x * (dashSpeed - ((dashSpeed/2) * dashTimer)) * Time.deltaTime;
                verticalMove = dashModifier * swipeDirection.y * (dashSpeed - ((dashSpeed/2) * dashTimer)) * Time.deltaTime;            
                
                dashTerminalV = horizontalMove;
                RaycastHit hitScene;
                if (Physics.Raycast(centerBottom.position, Vector3.forward, out hitScene, 10))
                {
                    //infront of building
                    if (hitScene.collider.gameObject.GetComponent<BuildingChunk>())
                    {
                        if (!stunFall)
                        {
                            hitScene.collider.gameObject.GetComponent<BuildingChunk>().TakeDamage(false);
                        }

                        //Check if infront of building
                        if (!hitScene.collider.gameObject.GetComponent<BuildingChunk>().isSky)
                        {
                            if (monsterTapped)
                            {
                                ClimbIfCan();
                            }
                        }
                    }
                }
                //dash canncel in air              
                if (monsterTapped)
                {
                    if (!Physics.Raycast(centerBottom.position, Vector3.forward, out hitScene, 10))
                    {
                        dashTimer = DASHTIME;
                        //dashTerminalV = swipeDirection.x;
                    }
                }
            }
            //dash finished
            else
            {
                AnimationSetter.instance.state = MonsterState.Fall;
                dashTimer = 0;
                transform.rotation = Quaternion.identity;                
                dashFall = true;
            }
        }

        //gravity
        if (AnimationSetter.instance.state == MonsterState.Fall || AnimationSetter.instance.state == MonsterState.Stun)
        {
            cc.height = ccHeight;
            transform.rotation = Quaternion.identity;
            FlipCharacter();
            if (fallspeed > GRAVITY)
            {
                fallspeed += Time.deltaTime * GRAVITY;
            }
            verticalMove = fallspeed * Time.deltaTime;

            if (monsterTapped)
            {
                ClimbIfCan();
            }
        }

        FlipCharacter();

        if (knockedBack)
        {
            horizontalMove = faceDirection * walkSpeed * Time.deltaTime;
        }

        Vector3 finalMove = new Vector3(horizontalMove, verticalMove, 0.0f);
        
        cc.Move(finalMove * Time.deltaTime);
    }   

    public void SetState()
    {
        canClimb = false;
        if (AnimationSetter.instance.state != MonsterState.Dash && AnimationSetter.instance.state != MonsterState.Stun && AnimationSetter.instance.state != MonsterState.Attack)
        {
            //grounded
            RaycastHit hitGround;
            if (Physics.Raycast(centerBottom.position, Vector3.down, out hitGround, (cc.skinWidth + 0.001f)))
            {
                if (hitGround.collider.gameObject.layer == LayerMask.NameToLayer("FoundationGround"))
                {
                    AnimationSetter.instance.state = MonsterState.GroundWalk;
                }
            }

            RaycastHit hitScene;
            if (Physics.Raycast(centerBottom.position, Vector3.forward, out hitScene, 10))
            {
                //infront of building
                if (hitScene.collider.gameObject.GetComponent<BuildingChunk>())
                {
                    //Check if on Roof
                    if (hitScene.collider.gameObject.GetComponent<BuildingChunk>().isSky)
                    {
                        AnimationSetter.instance.state = MonsterState.RoofWalk;
                        FixRoofDistance(hitScene.collider);
                    }
                    //regular climb
                    else
                    {
                        canClimb = true;
                        if (AnimationSetter.instance.state != MonsterState.GroundWalk && AnimationSetter.instance.state != MonsterState.Fall)
                        {
                            AnimationSetter.instance.state = MonsterState.Climb;
                        }
                    }
                }
            }
            else if (AnimationSetter.instance.state != MonsterState.GroundWalk)
            {
                AnimationSetter.instance.state = MonsterState.Fall;
            }
            //Falling
            if (AnimationSetter.instance.state != MonsterState.GroundWalk && AnimationSetter.instance.state != MonsterState.RoofWalk && AnimationSetter.instance.state != MonsterState.Climb)
            {
                AnimationSetter.instance.state= MonsterState.Fall;
            }
        }
        if (AnimationSetter.instance.state == MonsterState.Dash && swipeDirection.y < 0)
        {
            RaycastHit hitGround;
            if (Physics.Raycast(centerBottom.position, Vector3.down, out hitGround, 0.5f))
            {
                if (hitGround.collider.gameObject.layer == LayerMask.NameToLayer("FoundationGround"))
                {
                    AnimationSetter.instance.state = MonsterState.Fall;
                }
            }
        }

    }

    public void TakeDamage(bool stun)
    {
        if (!ghostTouch)
        {
            currentHealth--;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                print("You're Dead Mate...");
            }
            GUIManager.instance.UpdateMosterHealthBar(currentHealth / maxHealth);
            if (stun)
            {
                if (AnimationSetter.instance.state == MonsterState.Dash)
                {
                    StunPlayerH(STUNTIME, 0);
                }
                StartCoroutine(Invincibility());
            }
        }
    }

    public void TakeDamage(float d, bool stun)
    {
        if (!ghostTouch)
        {
            currentHealth -= d;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                print("You're Dead Mate...");
            }
            GUIManager.instance.UpdateMosterHealthBar(currentHealth / maxHealth);
            if (stun)
            {
                if (AnimationSetter.instance.state == MonsterState.Dash)
                {
                    StunPlayerH(STUNTIME, 0);
                }
                StartCoroutine(Invincibility());
            }
        }
    }

    IEnumerator Invincibility()
    {
        ghostTouch = true;
        //this.GetComponent<CapsuleCollider>().enabled = false;
        //this.GetComponent<CharacterController>().detectCollisions = false;
        //print("invincibility active");        
        for (int i = 0; i < NUMBEROFFLASHES; i++)
        {
            GameObject[] currentAnimation = AnimationSetter.instance.GetCurrentAnimationObject();
            foreach (GameObject a in currentAnimation)
            {
                Renderer[] pieces = a.transform.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in pieces)
                {
                    r.enabled = false;
                }
                yield return new WaitForSeconds(FLASHTIME);
                foreach (Renderer r in pieces)
                {
                    r.enabled = true;
                }
                yield return new WaitForSeconds(FLASHTIME);
            }
        }
        ghostTouch = false;
        //this.GetComponent<CapsuleCollider>().enabled = true;
        //this.GetComponent<CharacterController>().detectCollisions = true;
        //print("invincibility deactive");
    }

    public void HealDamage()
    {
        currentHealth++;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        GUIManager.instance.UpdateMosterHealthBar(currentHealth / maxHealth);
    }

    public void HealDamage(float d)
    {
        currentHealth += d;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        GUIManager.instance.UpdateMosterHealthBar(currentHealth/maxHealth);
    }

    public void SetMonsterTapped(bool b)
    {
        monsterTapped = b;
    }

    public void RecieveMovmentImput(Vector2 i)
    {
        movmentTouchInput = i;
    }

    public void SetSwipeDirection(Vector3 d)
    {
        if (d.y < 0 && AnimationSetter.instance.state == MonsterState.GroundWalk)
        {
            d.y = 0;
        }

        swipeDirection = d.normalized;
    }

    public void SetDashPower(float p)
    {
        dashPower = p;
    }

    void FixRoofDistance(Collider col)
    {
        if(GetComponent<Collider>().bounds.min.y > col.bounds.min.y)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (GetComponent<Collider>().bounds.min.y - col.bounds.min.y) + 0.01f, transform.position.z);
        }
    }

    public void ClimbIfCan()
    {
        RaycastHit hitScene;
        if (Physics.Raycast(centerBottom.position, Vector3.forward, out hitScene, 10))
        {
            //infront of building
            if (hitScene.collider.gameObject.GetComponent<BuildingChunk>() && !hitScene.collider.gameObject.GetComponent<BuildingChunk>().isSky)
            {
                canClimb = true;
                AnimationSetter.instance.state = MonsterState.Climb;
                dashFall = false;
                stunFall = false;
            }
        }
    }

    void FlipCharacter()
    {
        if (turnAround)
        {
            if (horizontalMove < 0)
            {
                if (AnimationSetter.instance.state == MonsterState.Dash)
                {
                    transform.localScale = new Vector3(1, -1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                faceDirection = -1;
            }
            if (horizontalMove > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                faceDirection = 1;
            }
            if (verticalMove < 0 && AnimationSetter.instance.state != MonsterState.Dash)
            {
                transform.localScale = new Vector3(transform.localScale.x, 1, 1);
            }
        }
    }

    public void SlowPlayer(float slowPercent, float slowDuration)
    {
        StartCoroutine(SlowSpeed(slowPercent, slowDuration));
    }

    IEnumerator SlowSpeed(float slowPercent, float slowDuration)
    {
        if (walkSpeed > baseWalkSpeed * .25f)
        {
            walkSpeed -= baseWalkSpeed * slowPercent;
        }
        if (dashSpeed > baseDashSpeed * .25f)
        {
            dashSpeed -= baseDashSpeed * slowPercent;
        }
        yield return new WaitForSeconds(slowDuration);
        walkSpeed += baseWalkSpeed * slowPercent;
        dashSpeed += baseDashSpeed * slowPercent;
        if (walkSpeed > baseWalkSpeed)
        {
            walkSpeed = baseWalkSpeed;
        }
        if (dashSpeed > baseDashSpeed)
        {
            dashSpeed = baseDashSpeed;
        }
    }

    public void StunPlayerH(float stunTime, float stunForce)
    {
        StartCoroutine(Stun(stunTime, stunForce));
        stunFall = true;        
    }

    IEnumerator Stun(float s, float f)
    {
        AnimationSetter.instance.state = MonsterState.Stun;
        FlipCharacter();
        if (f > 0)
        {
            knockedBack = true;
            walkSpeed = -f;
        }
        turnAround = false;        
        dashSpeed = 0;
        
        yield return new WaitForSeconds(s);
        walkSpeed = baseWalkSpeed;
        dashSpeed = baseDashSpeed;
        turnAround = true;
        knockedBack = false;
        AnimationSetter.instance.state = MonsterState.Fall;
    }
}
