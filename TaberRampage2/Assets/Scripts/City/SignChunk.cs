using UnityEngine;
using System.Collections;

public class SignChunk : MonoBehaviour
{
    public signType spawnType;    
    public int spawnRate;           //number greater than 1, high end of random range for spawn. Larger the number less likely the spawn.
    public bool westSide;
    public bool stun;
    public float stunTime, knockback;
    bool statNumbers;

    private void Start()
    {
        statNumbers = (StatisticsNumbers.instance != null);
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
                if (statNumbers)
                {
                    StatisticsNumbers.instance.ModifyEnvironmentObjectsDestroyed(1);
                    statNumbers = false;
                }
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

}

public enum signType
{
    Center,
    EastNeighbor,
    NorthSouthNeighbor
}
