using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim_Bodyparts : MonoBehaviour 
{
    public Transform Part1;
    public float Factor1;
    public Transform Part2;
    public float Factor2;
    public Transform Part3;
    public float Factor3;
    public Transform Part4;
    public float Factor4;

    public float angleAim;
	
	void Update () 
    {

        //if the character is flipped, the z rotate isn't flipped so + flips from clockwise to counterclockwise...

        if (Part1 != null)
        {
            if (Part1.rotation.z != angleAim * Factor1)
            {
                float angle = Mathf.LerpAngle(Part1.rotation.z, angleAim * Factor1, Time.time);
                Part1.eulerAngles = new Vector3(0, 0, angle);
//            Debug.Log(Part1 + " rotating from " + Part1.rotation.z + " to " + angle);
            }
        }

        if (Part2 != null)
        {
            if (Part2.rotation.z != angleAim * Factor2)
            {
                float angle = Mathf.LerpAngle(Part2.rotation.z, angleAim * Factor2, Time.time);
                Part2.eulerAngles = new Vector3(0, 0, angle);
//            Debug.Log(Part2 + " rotating from " + Part2.rotation.z + " to " + angle);
            }
        }

        if (Part3 != null)
        {
            if (Part3.rotation.z != angleAim * Factor3)
            {
                float angle = Mathf.LerpAngle(Part3.rotation.z, angleAim * Factor3, Time.time);
                Part3.eulerAngles = new Vector3(0, 0, angle);
//            Debug.Log(Part3 + " rotating from " + Part3.rotation.z + " to " + angle);
            }
        }

        if (Part4 != null)
        {
            if (Part4.rotation.z != angleAim * Factor4)
            {
                float angle = Mathf.LerpAngle(Part3.rotation.z, angleAim * Factor4, Time.time);
                Part4.eulerAngles = new Vector3(0, 0, angle);
//            Debug.Log(Part3 + " rotating from " + Part3.rotation.z + " to " + angle);
            }
        }
	}
}
