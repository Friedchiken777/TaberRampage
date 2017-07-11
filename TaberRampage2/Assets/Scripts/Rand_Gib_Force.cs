using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rand_Gib_Force : MonoBehaviour {

    private Rigidbody2D rb;
    public float ForceVal;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        float RandAngle = Random.Range(100,250);

        rb.AddForce(new Vector2(RandAngle * -1, ForceVal));
        rb.AddTorque(RandAngle);
	}
	


}
