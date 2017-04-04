using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationSetter : MonoBehaviour
{
    const float ATTACKDURATION = 34f/60f;
    public CustomAnimations[] a;
    int currentIndex, lastIndex;

    bool moving;
    public MonsterState state;
    bool thisIsMonsterPlayer;

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
        if (this.GetComponent<MonsterController>() != null)
        {
            thisIsMonsterPlayer = true;
        }
        state = MonsterState.Nothing;
        for (int i = 1; i < a.Length; i++)
        {
            if (a[i].Default != null)
            {
                a[i].Default.SetActive(true);
            }
            if (a[i].Attack != null)
            {
                a[i].Attack.SetActive(false);
            }
            a[i].WholePrefab.SetActive(false);            
        }
	}

    void Update()
    {
        if (thisIsMonsterPlayer)
        {
            MonsterAnimationCheck();
        }
        SetAnimation();
    }

    void SetAnimation()
    {
        if (!a[currentIndex].WholePrefab.activeSelf)
        {
            a[lastIndex].WholePrefab.SetActive(false);
            a[currentIndex].WholePrefab.SetActive(true);
            if (a[currentIndex].Default != null)
            {
                a[currentIndex].Default.SetActive(true);
            }
            if (a[currentIndex].Attack != null)
            {
                a[currentIndex].Attack.SetActive(false);
            }
            ParentToBuilding();
            lastIndex = currentIndex;            
        }
    }

    void MonsterAnimationCheck()
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
        else
        {
            currentIndex = 0;
        }
    }

    public GameObject[] GetCurrentAnimationObject()
    {
        return new GameObject[] { a[currentIndex].WholePrefab };
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
        if (a[currentIndex].Attack != null)
        {
            StartCoroutine(AttackAnimation());
        }
    }

    IEnumerator AttackAnimation()
    {
        SetAnimation();
        if (a[currentIndex].Default != null)
        {
            a[currentIndex].Default.SetActive(false);
        }
        if (a[currentIndex].Attack != null)
        {
            a[currentIndex].Attack.SetActive(true);
        }
        yield return new WaitForSeconds(ATTACKDURATION);
        if (a[currentIndex].Default != null)
        {
            a[currentIndex].Default.SetActive(true);
        }
        if (a[currentIndex].Attack != null)
        {
            a[currentIndex].Attack.SetActive(false);
        }
        SetAnimation();
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
    public GameObject WholePrefab, Default, Attack;
}
