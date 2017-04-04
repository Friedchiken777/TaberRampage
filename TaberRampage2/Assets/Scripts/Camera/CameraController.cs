using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    const float YADJUST = 3;
    const float DEFAULTORTHSIZE = 5;
    const float CAMERAZOOMSPEEDTHRESH = 0.15f;

    List<ViewEffector> highNooners;
    bool noPlayers;
    public float centerX, centerY;
    public float upperY, upPanFactor;
    float adjustedYAdjust;

	// Use this for initialization
	void Start ()
    {
        //create list of all players
        highNooners = new List<ViewEffector>();
        ViewEffector[] ve = GameObject.FindObjectsOfType<ViewEffector>();
        foreach (ViewEffector v in ve)
        {
            AddViewEffector(v);
        }

        //chect that players are in the scene
        if (highNooners.Count == 0)
        {
            print("No Players to track...");
            noPlayers = true;
        }

        adjustedYAdjust = YADJUST;
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if (!noPlayers)
        {
            //find center of all players
            float lowestX = Mathf.Infinity;
            float highestX = Mathf.NegativeInfinity;
            float lowestY = Mathf.Infinity;
            float highestY = Mathf.NegativeInfinity;
            foreach (ViewEffector v in highNooners)
            {
                if (lowestX > v.transform.position.x)
                {
                    lowestX = v.transform.position.x;
                }
                if (highestX < v.transform.position.x)
                {
                    highestX = v.transform.position.x;
                }
                if (lowestY > v.transform.position.y)
                {
                    lowestY = v.transform.position.y;
                }
                if (highestY < v.transform.position.y)
                {
                    highestY = v.transform.position.y;
                }
            }

            if (highestY > upperY)
            {
                if (highestY - upperY > 0)
                {
                    adjustedYAdjust = YADJUST - ((highestY - upperY)/upPanFactor);
                }
            }
            else
            {
                adjustedYAdjust = YADJUST;
            }

            centerX = (lowestX + highestX) / 2.0f;
            centerY = ((lowestY + highestY) / 2.0f) + adjustedYAdjust;

            Vector3 modifiedPos = new Vector3(centerX, centerY, transform.position.z);

            transform.position = modifiedPos;            
        }
    }

    public void AddViewEffector(ViewEffector v)
    {
        highNooners.Add(v);
        noPlayers = false;
    }

    public void RemoveViewEffectors(ViewEffector v)
    {
        highNooners.Remove(v);
        if (highNooners.Count == 0)
        {
            noPlayers = true;
            print("Last Player Removed");
        }
    }

    public float GetYAdjustment()
    {
        return adjustedYAdjust;
    }

    public void ModifyOrthagraphicSize(float f)
    {
        this.GetComponent<Camera>().orthographicSize = DEFAULTORTHSIZE + f;
    }
}
