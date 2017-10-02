using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField]
    EnemyParentScript holder;

    [SerializeField]
    float stunDuration, stunForce;

    protected void OnTriggerEnter(Collider col)
    {
        if (!holder.GetAttacked())
        {
            if (col.gameObject.GetComponent<MonsterController>() != null)
            {
                AnimationSetter.instance.SetAttackState();
                col.GetComponent<MonsterController>().StunPlayerH(stunDuration, stunForce);
                holder.gameObject.GetComponent<StatePartSwap>().TriggerSwap();
                holder.SetAttacked(true);
            }
        }
    }
}
