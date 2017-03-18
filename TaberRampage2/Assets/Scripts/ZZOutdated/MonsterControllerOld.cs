using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class MonsterControllerOld : MonoBehaviour
{ 
    const float GRAVITY = -10;
    const float ZLAYER = -3;

    CharacterController cc;
    Transform leftBottom, rightBottom, centerBottom;

    public float maxHealth, currentHealth;    

    public bool climbing, grounded, roofWalk, falling, jumping, jumpCooldown;
    public float baseWalkSpeed, BaseClimbSpeed, jumpHeight, jumpSpeed, swipeSpeed;

    public Vector3 jumpStartPos;

    float walkSpeed, climbSpeed, fallspeed;


    public bool isMousePressed, swipeMove;
    public List<Vector3> pointsList;
    Vector3 mousePos;
    public int moveLineIndex;

    // Use this for initialization
    void Start ()
    {
        cc = this.GetComponent<CharacterController>();
        leftBottom = transform.FindChild("LeftBottom");
        rightBottom = transform.FindChild("RightBottom");
        centerBottom = transform.FindChild("CenterBottom");
        currentHealth = maxHealth;

        isMousePressed = false;
        pointsList = new List<Vector3>();
        swipeMove = false;
        moveLineIndex = 0;
	}

	void FixedUpdate ()
    {
        if (!swipeMove)
        {
            Walk();
        }
        SwipeMove();

        /*if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitScene;
            if (Physics.Raycast(transform.position, Vector3.forward, out hitScene, 10))
            {
                if (hitScene.collider.gameObject.GetComponent<BuildingChunk>())
                {
                    hitScene.collider.gameObject.GetComponent<BuildingChunk>().TakeDamage();
                }
            }
        }*/
    }

    void Walk()
    {
        climbSpeed = BaseClimbSpeed;
        walkSpeed = baseWalkSpeed;

        float horizontalMove = Input.GetAxis("Horizontal") * walkSpeed * Time.deltaTime;
        float verticalMove = 0;        

        roofWalk = false;
        climbing = false;
        grounded = false;

        //grounded check
        RaycastHit hitGround;
        if (Physics.Raycast(centerBottom.position, Vector3.down, out hitGround, (cc.skinWidth + 0.001f)))
        {
            grounded = true;
            falling = false;
        }

        RaycastHit hitScene;
        if (Physics.Raycast(centerBottom.position, Vector3.forward, out hitScene, 10))
        {
            //print("Infront of " + hitScene.collider.gameObject.name);
            //infront of building
            if (hitScene.collider.gameObject.GetComponent<BuildingChunk>())
            {                
                //regular climb
                if (!falling && !hitScene.collider.gameObject.GetComponent<BuildingChunk>().isSky && !hitScene.collider.gameObject.GetComponent<BuildingChunk>().isBorder)
                {
                    //print(hit.collider.gameObject.name);
                    verticalMove = Input.GetAxis("Vertical") * climbSpeed * Time.deltaTime;
                    horizontalMove = Input.GetAxis("Horizontal") * climbSpeed * Time.deltaTime;
                    climbing = true;
                    jumpCooldown = false;
                }
                //must "grab" building if falling
                else
                {
                    if (Input.GetMouseButtonDown(0) || jumping)
                    {
                        falling = false;
                        verticalMove = Input.GetAxis("Vertical") * climbSpeed * Time.deltaTime;
                        horizontalMove = Input.GetAxis("Horizontal") * climbSpeed * Time.deltaTime;
                        climbing = true;
                    }
                }
                //on roof
                if (hitScene.collider.gameObject.GetComponent<BuildingChunk>().isSky)
                {                    
                    roofWalk = true;
                    falling = false;
                    jumpCooldown = false;
                    
                    //climb back down building
                    if (Input.GetAxis("Vertical") < 0)
                    {
                        verticalMove = Input.GetAxis("Vertical") * climbSpeed * Time.deltaTime;
                    }
                }

                if (climbing)
                {
                    //slow sliding off building
                    if (!Physics.Raycast(leftBottom.position, Vector3.forward, out hitScene, 10) || !Physics.Raycast(rightBottom.position, Vector3.forward, out hitScene, 10))
                    {
                        if (Input.GetAxis("Horizontal") < 0)
                        {
                            climbSpeed = climbSpeed / 4;
                        }
                    }
                }
            }           
        }

        //check for jump
        if (!jumping && !jumpCooldown)
        {            
            if (Input.GetButtonDown("Jump"))
            {
                jumping = true;
                jumpCooldown = true;
                jumpStartPos = transform.position;
            }            
        }
        //Jump
        if ((jumpStartPos.y + jumpHeight) > transform.position.y && jumping)
        {
            verticalMove += Time.deltaTime * jumpSpeed;
        }
        else
        {
            jumping = false;
        }

        if (grounded)
        {
            climbing = false;
            roofWalk = false;
            jumpCooldown = false;
            fallspeed = 0;
        }

        //gravity
        if (!grounded && !climbing && !roofWalk && !jumping)
        {
            falling = true;
            if (fallspeed > GRAVITY)
            {
                fallspeed += Time.deltaTime * GRAVITY;
            }
            verticalMove = fallspeed * Time.deltaTime;
        }

        Vector3 finalMove = new Vector3(horizontalMove, verticalMove, 0.0f);
        
        cc.Move(finalMove);
    }

    void SwipeMove()
    {
        if (swipeMove)
        {
            if (moveLineIndex < pointsList.Count)
            {
                transform.position = Vector3.MoveTowards(transform.position, pointsList[moveLineIndex], swipeSpeed * Time.deltaTime);
                RaycastHit hitScene;
                if (Physics.Raycast(transform.position, Vector3.forward, out hitScene, 10))
                {
                    if (hitScene.collider.gameObject.GetComponent<BuildingChunk>())
                    {
                        hitScene.collider.gameObject.GetComponent<BuildingChunk>().TakeDamage(false);
                    }
                }
                if (transform.position == pointsList[moveLineIndex])
                {
                    moveLineIndex++;
                }
            }
            else
            {
                swipeMove = false;                
            }
        }
        else {
            if (Input.GetMouseButtonDown(0))
            {
                isMousePressed = true;
                pointsList.RemoveRange(0, pointsList.Count);
            }
            if (Input.GetMouseButtonUp(0))
            {
                isMousePressed = false;
                moveLineIndex = 0;
                swipeMove = true;
            }
            // Get points for swipe line
            if (isMousePressed)
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = ZLAYER;
                if (!pointsList.Contains(mousePos))
                {
                    pointsList.Add(mousePos);
                }
            }
        }
    }

    public void TakeDamage()
    {
        currentHealth--;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            print("You're Dead Mate...");
        }
        GUIManager.instance.UpdateMosterHealthBar(currentHealth/maxHealth);
    }

    public void TakeDamage(float d)
    {
        currentHealth -= d;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            print("You're Dead Mate...");
        }
        GUIManager.instance.UpdateMosterHealthBar(currentHealth / maxHealth);
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
        GUIManager.instance.UpdateMosterHealthBar(currentHealth / maxHealth);
    }
}
