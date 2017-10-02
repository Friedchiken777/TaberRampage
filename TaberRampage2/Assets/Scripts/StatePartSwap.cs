using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePartSwap : MonoBehaviour {

    public int state;
    public Transform Part1; //holding arm
    public Transform Part2; //top good
    public Transform Part3; //top bad
    public Transform Part4; //bottom good
    public Transform Part5; //bottom bad
    public Transform Part6; //shield middle
    public Transform Part7; //free arm


    [SerializeField]
    protected Shield shield;

    //    private Vector3 P1pos;
    //    private Vector3 P2pos;
    //    private Vector3 P3pos;
    //    private Vector3 P4pos;
    //    private Vector3 P5pos;
    //    private Vector3 P6pos;
    //    private Vector3 P7pos;

    void Start () 
    {
        state = 0;

//        P1pos = Part1.transform.position;
//        P2pos = Part2.transform.position;
//        P3pos = Part3.transform.position;
//        P4pos = Part4.transform.position;
//        P5pos = Part5.transform.position;
//        P6pos = Part6.transform.position;
//        P7pos = Part7.transform.position;
	}

    void Update () //testing purposes only
    {
        /*if (Input.anyKeyDown)
        {
            if (state < 3)
                TriggerSwap();
        }*/
    }
	
	public void TriggerSwap () 
    {
        state += 1;

        switch (state)
        {
            case 1: //top broken
                {
                    Part2.transform.Translate(new Vector3(0,0, 10000));
                    Part3.transform.Translate(new Vector3(0,0,-10000));
                    break;
                }
            case 2: //both broken
                {
                    Part4.transform.Translate(new Vector3(0,0,10000));
                    Part5.transform.Translate(new Vector3(0,0,-10000));

                    break;
                }
            case 3: //gone
                {
                    Part1.transform.Translate(new Vector3(0,0,10000));
                    Part6.transform.Translate(new Vector3(0,0,10000));
                    Part7.transform.Translate(new Vector3(0,0,-10000));
                    Destroy(shield.gameObject);
                    break;
                }
            default: //undamaged
                {
                    Part1.transform.Translate(new Vector3(0,0,-10000));
                    Part2.transform.Translate(new Vector3(0,0,-10000));
                    Part3.transform.Translate(new Vector3(0,0,10000));
                    Part4.transform.Translate(new Vector3(0,0,-10000));
                    Part5.transform.Translate(new Vector3(0,0,10000));
                    Part6.transform.Translate(new Vector3(0,0,-10000));
                    Part7.transform.Translate(new Vector3(0,0,10000));
                    break;
                }
        }

//        if (Part1 != null)
//        {
//            
//        }
	}
}
