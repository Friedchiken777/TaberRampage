using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootHelper : MonoBehaviour
{
    [SerializeField]
    EnemyParentScript parent;

    public void FireAssist()
    {
        parent.Fire();
    }
}
