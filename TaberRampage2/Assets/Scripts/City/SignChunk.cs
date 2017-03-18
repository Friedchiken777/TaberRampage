using UnityEngine;
using System.Collections;

public class SignChunk : MonoBehaviour
{
    public signType spawnType;    
    public int spawnRate;           //number greater than 1, high end of random range for spawn. Larger the number less likely the spawn.
    public bool westSide;
    public bool stun;
    public float stunTime, knockback;


    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<MonsterController>() != null)
        {
            //Stun Signs
            if (stun)
            {
                col.GetComponent<MonsterController>().StunPlayerH(stunTime, 0);
                Destroy(this.gameObject, stunTime / 2);
            }
            //AC
            else
            {
                if (AnimationSetter.instance.state != MonsterState.Climb)
                {
                    RaycastHit hitScene;
                    if (Physics.Raycast(col.transform.position, Vector3.down, out hitScene, 3))
                    {
                        //Set character to climb if lands directly on top of AC unit
                        if (hitScene.collider.transform == transform)
                        {
                            col.GetComponent<MonsterController>().ClimbIfCan();
                        }
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        //never called...
        print("he hitted me...");
    }
}

public enum signType
{
    Center,
    EastNeighbor,
    NorthSouthNeighbor
}
