using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    const float CHUNKSIZE = 2.048f;                     //size of standard building chunk
    const float PERCENTDAMAGEFORDEATH = .65f;
    const float TIMETODEATH = 30;
    const float TERRORBONUSMULTIPLIER = 2;

    public bool heroBuilding;

    public List<GameObject> groundKit, groundBorderKit, middleKit, middleBorderKit, roofKit, roofBorderKit, skyKit, signKit, rubbleKit, rubbleBorderKit;

    public float maxHealth, currentHealth;

    public GameObject rubblePile;

    Vector3 rubbleEndPoint;

    public Vector3 windowEnemyOffset;
    public float[] alternateXValuesforWindowEnemies;

    float fallRate;

    bool died;
    bool statNumbers;
    //Requires file structure bases on objects name
    //Each building should have and empty gameobject with this script in the foundations folder
    //There should also be a corresponding folder structure with the correct name that has all building chunk prefabs
	void Awake ()
    {
        transform.name = transform.name.Replace("(Clone)", "").Trim();
        rubblePile = new GameObject();
        rubblePile.name = gameObject.name + "RubblePile";
        rubblePile.transform.position = new Vector3(transform.position.x, transform.position.y - CHUNKSIZE, transform.position.z);
        rubbleEndPoint = new Vector3(rubblePile.transform.position.x, rubblePile.transform.position.y + CHUNKSIZE, rubblePile.transform.position.z);
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/GroundFloor", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            groundKit.Add(g);
        }
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/GroundBorder", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            groundBorderKit.Add(g);
        }
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/MiddleFloors", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            middleKit.Add(g);
        }
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/MiddleBorder", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            middleBorderKit.Add(g);
        }
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/Roof", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            roofKit.Add(g);
        }
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/RoofBorder", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            roofBorderKit.Add(g);
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
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/Rubble", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            rubbleKit.Add(g);
        }
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/" + gameObject.name + "/RubbleBorder", typeof(GameObject)))
        {
            //Debug.Log("prefab found: " + g.name);
            rubbleBorderKit.Add(g);
        }
        died = false;       
    }

    private void Start()
    {
        statNumbers = (StatisticsNumbers.instance != null);
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
        if (!died)
        {
            died = true;
            TerrorManager.instance.AddTerror(maxHealth * TERRORBONUSMULTIPLIER);
            Destroy(this.gameObject, TIMETODEATH);            
            if (statNumbers)
            {
                StatisticsNumbers.instance.ModifyBuildingsDestroyed(1);
                statNumbers = false;
            }
        }
        transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime, transform.position.z);
        
        rubblePile.transform.position = Vector3.Lerp(rubblePile.transform.position, rubbleEndPoint, Time.deltaTime / CHUNKSIZE);
    }
}
