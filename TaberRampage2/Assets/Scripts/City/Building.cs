using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    const float PERCENTDAMAGEFORDEATH = .65f;
    const float TIMETODEATH = 30;
    const float TERRORBONUSMULTIPLIER = 2;

    public bool heroBuilding;

    public List<GameObject> groundFloorKit, middleFloorKit, roofKit, skyKit, signKit;

    public float maxHealth, currentHealth;

    float fallRate;

    int died;
    //Requires file structure bases on objects name
    //Each building should have and empty gameobject with this script in the foundations folder
    //There should also be a corresponding folder structure with the correct name that has all building chunk prefabs
	void Awake ()
    {
        transform.name = transform.name.Replace("(Clone)", "").Trim();
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/GroundFloor", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            groundFloorKit.Add(g);
        }
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/MiddleFloors", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            middleFloorKit.Add(g);
        }
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/Roof", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            roofKit.Add(g);
        }
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/Sky", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            skyKit.Add(g);
        }
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/Signs", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            signKit.Add(g);
        }
        died = 0;
    }

    void Update()
    {
        if (currentHealth < maxHealth - (maxHealth * PERCENTDAMAGEFORDEATH))
        {
            BuildingDeath();            
        }
    }

    void BuildingDeath()
    {
        if (died < 2)
        {
            died++;
        }        
        if (died == 1)
        {
            TerrorManager.instance.AddTerror(maxHealth * TERRORBONUSMULTIPLIER);
            Destroy(this.gameObject, TIMETODEATH);
        }
        transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime, transform.position.z);
        
    }
}
