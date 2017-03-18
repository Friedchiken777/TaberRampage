using UnityEngine;
using System.Collections;

public class TempKillingFloor : MonoBehaviour
{
    public GameObject player;

    public Vector3 initialPosition;
    
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindObjectOfType<MonsterController>().gameObject;
        initialPosition = player.transform.position;
    }

    void OnTriggerEnter(Collider col)
    {
        col.gameObject.transform.position = initialPosition;
    }
}
