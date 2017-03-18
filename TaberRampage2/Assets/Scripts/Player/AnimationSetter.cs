using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationSetter : MonoBehaviour
{
    public GameObject[] animations;
    public CustomAnimations[] a;
    int currentIndex, lastIndex;

    bool moving, attacking;
    public MonsterState state;

    public static AnimationSetter instance = null;

    // Use this for initialization
    void Start ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        currentIndex = 0;
        state = MonsterState.Nothing;
        for (int i = 1; i < a.Length; i++)
        {
            //animations[i].SetActive(false);
            a[i].TopHalf.SetActive(false);
            if (a[i].BottomHalf != null)
            {
                a[i].BottomHalf.SetActive(false);
            }
        }
	}

    void Update()
    {
        if (!attacking)
        {
            if (moving && (state == MonsterState.GroundWalk || state == MonsterState.RoofWalk))
            {
                currentIndex = 1;
            }
            else if (state == MonsterState.Climb)
            {
                if (moving)
                {
                    currentIndex = 2;
                }
                else
                {
                    currentIndex = 3;
                }
            }
            else if (state == MonsterState.Fall)
            {
                currentIndex = 4;
            }
            else if (state == MonsterState.Dash)
            {
                currentIndex = 5;
            }
            else if (state == MonsterState.Stun)
            {
                currentIndex = 6;
            }
            else if (state == MonsterState.Attack)
            {
                currentIndex = 7;
            }
            else
            {
                currentIndex = 0;
            }
            SetAnimation();
        }
    }

    void SetAnimationOld()
    {
        if (!animations[currentIndex].activeSelf)
        {
            animations[lastIndex].SetActive(false);
            animations[currentIndex].SetActive(true);
            ParentToBuilding();
            lastIndex = currentIndex;            
        }
    }

    void SetAnimation()
    {
        if ((a[currentIndex].BottomHalf == null && a[lastIndex].BottomHalf == null) || a[currentIndex].SwapTop || a[lastIndex].SwapTop)
        {

        }
        else {
            if (a[currentIndex].BottomHalf != null && !a[currentIndex].BottomHalf.activeSelf)
            {
                if (a[lastIndex].BottomHalf != null)
                {
                    a[lastIndex].BottomHalf.SetActive(false);
                    a[currentIndex].BottomHalf.SetActive(true);
                }
            }
            if (a[currentIndex].BottomHalf == null && a[lastIndex].BottomHalf.activeSelf)
            {
                a[lastIndex].BottomHalf.SetActive(false);
            }
        }
        if (!a[currentIndex].TopHalf.activeSelf)
        {
            a[lastIndex].TopHalf.SetActive(false);
            a[currentIndex].TopHalf.SetActive(true);
            ParentToBuilding();
            lastIndex = currentIndex;

        }
    }

    public GameObject[] GetCurrentAnimationObject()
    {
        if (a[currentIndex].BottomHalf != null)
        {
            return new GameObject[] { a[currentIndex].TopHalf, a[currentIndex].BottomHalf };
        }
        return new GameObject[] { a[currentIndex].TopHalf };
    }

    void ParentToBuilding()
    {
        //when climbing, holding onto, or standing on top a building
        if (state == MonsterState.Climb || state == MonsterState.RoofWalk || currentIndex != 4) //Remove currentindex != 4 once roof assets complete
        {
            RaycastHit hitScene;
            if (Physics.Raycast(transform.position, Vector3.forward, out hitScene, 10))
            {
                if (hitScene.collider.gameObject.GetComponent<BuildingChunk>())
                {
                    this.gameObject.transform.parent = hitScene.collider.transform;
                }
            }
            
        }
        else
        {
            this.gameObject.transform.parent = null;
        }
    }

    public void SetMovement(bool b)
    {
        moving = b;
    }

    public void SetAttackState()
    {
        StartCoroutine(AttackAnimation());
    }

    IEnumerator AttackAnimation()
    {
        attacking = true;
        currentIndex = 7;        
        SetAnimation();
        //a[currentIndex].TopHalf.GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(34f / 60f);//a[currentIndex].TopHalf.GetComponent<Animation>().clip.length);
        //a[currentIndex].TopHalf.GetComponent<Animator>().SetBool("Exit", false);
        attacking = false;
    }
        
}

public enum MonsterState
{
    GroundWalk,
    RoofWalk,
    Climb,
    Fall,
    Dash,
    Stun,
    Attack,
    Nothing
}

[System.Serializable]
public class CustomAnimations
{
    public string name;
    public bool SwapTop;
    public GameObject TopHalf, BottomHalf;
}
