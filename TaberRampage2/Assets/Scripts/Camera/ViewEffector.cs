using UnityEngine;
using System.Collections;

public class ViewEffector : MonoBehaviour
{
    Camera cc;

    float distanceFromScreen;
    float leftBorder;
    float rightBorder;

    // Use this for initialization
    void Start()
    {
        cc = GameObject.FindObjectOfType<CameraController>().GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceFromScreen = (transform.position - cc.transform.position).z;
        leftBorder = cc.ViewportToWorldPoint(new Vector3(0,0,distanceFromScreen)).x;
        rightBorder = cc.ViewportToWorldPoint(new Vector3(1, 0, distanceFromScreen)).x;

        //keeps player from walking off screen
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, leftBorder, rightBorder), transform.position.y, transform.position.z);
    }

    public Vector3 GetObjectPosition()
    {
        return transform.position;
    }
}
