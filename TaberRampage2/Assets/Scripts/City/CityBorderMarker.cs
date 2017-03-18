using UnityEngine;
using System.Collections;

public class CityBorderMarker : MonoBehaviour
{
    public bool right;

    float distanceFromScreen;
    float leftBorder;
    float rightBorder;
    CitySpawnManager csm;
    Camera cc;

	// Use this for initialization
	void Start ()
    {
        csm = GameObject.FindObjectOfType<CitySpawnManager>();
        cc = GameObject.FindObjectOfType<CameraController>().GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = new Vector3(cc.transform.position.x, csm.transform.position.y, csm.transform.position.z);

        distanceFromScreen = (csm.transform.position - cc.transform.position).z;

        leftBorder = cc.ViewportToWorldPoint(new Vector3(0, 0, distanceFromScreen)).x;
        rightBorder = cc.ViewportToWorldPoint(new Vector3(1, 0, distanceFromScreen)).x;

        if (right)
        {
            transform.position = new Vector3(rightBorder, transform.position.y + 10, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(leftBorder, transform.position.y + 10, transform.position.z);
        }
    }
}
