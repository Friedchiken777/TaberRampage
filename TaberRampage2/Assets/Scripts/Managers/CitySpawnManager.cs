using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CitySpawnManager : MonoBehaviour
{
    const float CHUNKSIZE = 2.048f;                     //size of standard building chunk
    const int MINBUILDINGHEIGHT = 4;                    //min height of buildings
    const int MAXBUILDINGHEIGHT = 10;                    //maximum height of buildings
    const float BORDERCOLLIDERCENTERX = 0.75f;          //modified center for building border pieces collider x axis only
    const float BORDERCOLLIDERSIZEX = 0.55f;            //modified size for building border pieces collider x axis only
    const float MINALLEYDIST = 0.15f;                   //min distance between buildings
    const float MAXALLEYDIST = CHUNKSIZE * 0.5f;        //max space between buildings

    public bool debug = false;                          //toggles debug lines

    Camera cc;
    Transform rightEdge, leftEdge;
    CityBorderMarker[] markers;

    float distanceFromScreen;
    float leftBorder;
    float rightBorder;

    public GameObject currentGround;

    public LayerMask cityFoundation, buildingChunks;

    public List<GameObject> cityGrounds, buildings;

    GameObject newestChunk, lowerFloor;

    public static CitySpawnManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        cc = GameObject.FindObjectOfType<CameraController>().GetComponent<Camera>();
        markers = GameObject.FindObjectsOfType<CityBorderMarker>();
        //Find initial ground
        RaycastHit hitC;
        if (Physics.Linecast(cc.transform.position, transform.position, out hitC, cityFoundation))
        {
            //print(hitC.collider.gameObject.name + " C");
            if (currentGround != hitC.collider.gameObject)
            {
                currentGround = hitC.collider.gameObject;
            }
        }
        //Find City Border Markers
        foreach (CityBorderMarker c in markers)
        {
            if (c.right)
            {
                rightEdge = c.transform;
            }
            else
            {
                leftEdge = c.transform;
            }
        }
        //load all ground types
        foreach (GameObject g in Resources.LoadAll("TestStuff/Grounds", typeof(GameObject)))
        {
            cityGrounds.Add(g);
        }
        //load all building types
        foreach (GameObject g in Resources.LoadAll("TestStuff/Buildings/Foundations", typeof(GameObject)))
        {
            buildings.Add(g);
        }
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(cc.transform.position.x, transform.position.y, transform.position.z);

        distanceFromScreen = (transform.position - cc.transform.position).z;
        leftBorder = cc.ViewportToWorldPoint(new Vector3(0, 0, distanceFromScreen)).x;
        rightBorder = cc.ViewportToWorldPoint(new Vector3(1, 0, distanceFromScreen)).x;

        Vector3 hitPointR = new Vector3(rightBorder, transform.position.y, transform.position.z);
        Vector3 hitPointL = new Vector3(leftBorder, transform.position.y, transform.position.z);

        if (debug)
        {
            Debug.DrawLine(cc.transform.position, hitPointR);
            Debug.DrawLine(cc.transform.position, hitPointL);
            Debug.DrawLine(cc.transform.position, transform.position);
            Debug.DrawLine(rightEdge.transform.position, hitPointR);
            Debug.DrawLine(leftEdge.transform.position, hitPointL);
        }

        //Find Ground
        RaycastHit hitC;
        if (Physics.Linecast(cc.transform.position, transform.position, out hitC, cityFoundation))
        {
            //print(hitC.collider.gameObject.name + " C");
            if (currentGround != hitC.collider.gameObject)
            {
                currentGround = hitC.collider.gameObject;
            }
        }

        //Check for City to the right
        RaycastHit hitR2;
        if (Physics.Linecast(rightEdge.transform.position, hitPointR, out hitR2, cityFoundation))
        {
            //print(hitR2.collider.gameObject.name + " R2");
        }
        else
        {
            SpawnCitySection(1);
        }

        //Check for City to the Left
        RaycastHit hitL2;
        if (Physics.Linecast(leftEdge.transform.position, hitPointL, out hitL2, cityFoundation))
        {
            //print(hitL2.collider.gameObject.name + " L2");            
        }
        else
        {
            SpawnCitySection(-1);
        }
    }

    //Greates ground section at random from all possible round sections
    void SpawnCitySection(int sign)
    {
        int rand = Random.Range(0, cityGrounds.Count);
        //print(rand);
        GameObject spawnGround = Instantiate(cityGrounds[rand], (Vector3.forward * 30), currentGround.transform.rotation) as GameObject;

        float oldEdge = currentGround.GetComponent<Collider>().bounds.extents.x * sign;

        float newCenter = currentGround.transform.position.x + oldEdge + (spawnGround.gameObject.GetComponent<Collider>().bounds.extents.x * sign);

        Vector3 spawnPosition = new Vector3(newCenter, currentGround.GetComponent<Collider>().bounds.center.y, currentGround.GetComponent<Collider>().bounds.center.z);
        spawnGround.transform.position = (spawnPosition);

        SpawnBuildings(spawnGround);
    }

    //Spawns buildings on a ground section
    void SpawnBuildings(GameObject ground)
    {
        Vector3 groundmin = ground.transform.position - ground.GetComponent<Collider>().bounds.extents;
        Vector3 groundmax = ground.transform.position + ground.GetComponent<Collider>().bounds.extents;

        for (float x = groundmin.x; x < groundmax.x; x+= CHUNKSIZE)
        {
            Vector3 top = new Vector3(x, ground.transform.position.y + 10, ground.transform.position.z);
            Vector3 bottom = new Vector3(x, ground.transform.position.y, ground.transform.position.z);
            
            //check for double spawn at start
            if (Physics.Linecast(top, bottom, buildingChunks))
            {
                //print("hitted" + x);
                continue;
            }

            //Building width semi hardcoded for ratio based sizing
            int randWidth = Random.Range(0,10);
            int buildingWidth = 0;
            int doorPosition = 0;
            int coin = Random.Range(0, 2);
            //Determin door position
            switch (randWidth)
            {
                case 0:
                    buildingWidth = 4;
                    if (coin == 0)
                    {
                        doorPosition = 2;
                    }
                    else
                    {
                        doorPosition = 3;
                    }
                    break;
                case 1:
                case 2:
                case 3:
                    buildingWidth = 6;
                    if (coin == 0)
                    {
                        doorPosition = 3;
                    }
                    else
                    {
                        doorPosition = 4;
                    }
                    break;
                case 4:
                case 5:
                    buildingWidth = 7;
                    doorPosition = 4;
                    break;
                default:
                    buildingWidth = 5;
                    doorPosition = 3;
                    break;
            }

            int buildingHeight = Random.Range(MINBUILDINGHEIGHT, MAXBUILDINGHEIGHT);

            //select building to build
            float slightOffset = Random.Range(MINALLEYDIST, MAXALLEYDIST);
            //print(slightOffset);
            Vector3 foundationLocation = new Vector3(x - slightOffset, (groundmax.y + (CHUNKSIZE/2)), ground.transform.position.z);
            int floorplan = Random.Range(0, buildings.Count);
            GameObject foundation = Instantiate(buildings[floorplan], foundationLocation, ground.transform.rotation) as GameObject;

            for (int i = 1; i <= buildingWidth; i++)
            {
                //spawn left border
                if (i == 1)
                {
                    newestChunk = Instantiate(foundation.GetComponent<Building>().groundFloorKit[0], foundation.transform.position, foundation.transform.rotation) as GameObject;
                    FixBorderCollider(newestChunk, 1);
                    newestChunk.GetComponent<BuildingChunk>().westSideBorder = true;
                }
                //spawn right border
                else if (i == buildingWidth)
                {
                    Vector3 spawnSpot = new Vector3((newestChunk.transform.position.x + CHUNKSIZE), newestChunk.transform.position.y, newestChunk.transform.position.z);
                    newestChunk = Instantiate(foundation.GetComponent<Building>().groundFloorKit[1], spawnSpot, newestChunk.transform.rotation) as GameObject;
                    FixBorderCollider(newestChunk, -1);
                }
                //place door
                else if (i == doorPosition)
                {
                    Vector3 spawnSpot = new Vector3((newestChunk.transform.position.x + CHUNKSIZE), newestChunk.transform.position.y, newestChunk.transform.position.z);
                    newestChunk = Instantiate(foundation.GetComponent<Building>().groundFloorKit[2], spawnSpot, newestChunk.transform.rotation) as GameObject;
                }
                else
                {
                    Vector3 spawnSpot = new Vector3((newestChunk.transform.position.x + CHUNKSIZE), newestChunk.transform.position.y, newestChunk.transform.position.z);
                    int groundPicker = Random.Range(3, foundation.GetComponent<Building>().groundFloorKit.Count);
                    newestChunk = Instantiate(foundation.GetComponent<Building>().groundFloorKit[groundPicker], spawnSpot, newestChunk.transform.rotation) as GameObject;                    
                }
                newestChunk.transform.parent = foundation.transform;
                newestChunk.transform.parent.GetComponent<Building>().maxHealth += newestChunk.GetComponent<BuildingChunk>().maxHealth;
                for (int j = 1; j <= buildingHeight; j++)
                {                    
                    if (j == 1)
                    {
                        lowerFloor = newestChunk;
                    }
                    Vector3 nextfloor = new Vector3(lowerFloor.transform.position.x, lowerFloor.transform.position.y + CHUNKSIZE, lowerFloor.transform.position.z);
                    //left border
                    if (i == 1)
                    {
                        if (j != buildingHeight)
                        {
                            if (j == buildingHeight - 1)
                            {
                                lowerFloor = Instantiate(foundation.GetComponent<Building>().roofKit[0], nextfloor, lowerFloor.transform.rotation) as GameObject;
                            }
                            else
                            {
                                lowerFloor = Instantiate(foundation.GetComponent<Building>().middleFloorKit[0], nextfloor, lowerFloor.transform.rotation) as GameObject;
                            }
                            FixBorderCollider(lowerFloor, 1);
                            lowerFloor.GetComponent<BuildingChunk>().westSideBorder = true;
                        }
                    }
                    //right border
                    else if (i == buildingWidth)
                    {
                        if (j != buildingHeight)
                        {
                            if (j == buildingHeight - 1)
                            {
                                lowerFloor = Instantiate(foundation.GetComponent<Building>().roofKit[1], nextfloor, lowerFloor.transform.rotation) as GameObject;
                            }
                            else
                            {
                                lowerFloor = Instantiate(foundation.GetComponent<Building>().middleFloorKit[1], nextfloor, lowerFloor.transform.rotation) as GameObject;
                            }
                            FixBorderCollider(lowerFloor, -1);
                        }
                    }
                    //center parts
                    else
                    {
                        int randChunk = Random.Range(2, foundation.GetComponent<Building>().roofKit.Count);
                        if (j == buildingHeight)
                        {
                            randChunk = Random.Range(0, foundation.GetComponent<Building>().skyKit.Count);
                            lowerFloor = Instantiate(foundation.GetComponent<Building>().skyKit[randChunk], nextfloor, lowerFloor.transform.rotation) as GameObject;
                        }
                        else if (j == buildingHeight - 1)
                        {
                            lowerFloor = Instantiate(foundation.GetComponent<Building>().roofKit[randChunk], nextfloor, lowerFloor.transform.rotation) as GameObject;
                        }
                        else
                        {
                            lowerFloor = Instantiate(foundation.GetComponent<Building>().middleFloorKit[randChunk], nextfloor, lowerFloor.transform.rotation) as GameObject;
                        }
                    }
                    lowerFloor.transform.parent = foundation.transform;
                    lowerFloor.GetComponent<BuildingChunk>().floorLevel = j;
                    if (!lowerFloor.GetComponent<BuildingChunk>().isBorder && !lowerFloor.GetComponent<BuildingChunk>().isSky)
                    {
                        lowerFloor.transform.parent.GetComponent<Building>().maxHealth += lowerFloor.GetComponent<BuildingChunk>().maxHealth;
                    }
                }
            }
            foundation.GetComponent<Building>().currentHealth = foundation.GetComponent<Building>().maxHealth;
            SetNeghboors(foundation);
            //Generate Signs
            foreach (GameObject s in foundation.GetComponent<Building>().signKit)
            {
                List<BuildingChunk> buildingChunksInQuestion = new List<BuildingChunk>();
                foreach (BuildingChunk b in foundation.transform.GetComponentsInChildren<BuildingChunk>())
                {
                    if(!b.isBorder && b.floorLevel != 0 && !b.hasSign && !b.isDoor && !b.hasWindowEnemy)
                    {
                        buildingChunksInQuestion.Add(b);
                    }
                } 

                switch (s.GetComponent<SignChunk>().spawnType)
                {
                    case signType.Center:
                    {
                        foreach (BuildingChunk b in buildingChunksInQuestion)
                        {
                            int rand = Random.Range(0, s.GetComponent<SignChunk>().spawnRate);
                            if (!b.isSky && rand == 1 && !b.hasSign)
                            {
                                GameObject tempSign = Instantiate(s, b.transform.position, b.transform.rotation) as GameObject;
                                tempSign.transform.parent = foundation.transform;
                                b.hasSign = true;
                            }
                        }

                        break;
                    }
                    case signType.EastNeighbor:
                    {
                            foreach (BuildingChunk b in buildingChunksInQuestion)
                            {
                                int rand = Random.Range(0, s.GetComponent<SignChunk>().spawnRate);
                                //check for east neighbor
                                if (b.GetNeighbor(0) != null && !b.hasSign && b.floorLevel > 4 && rand == 1)
                                {
                                    //check for neighboor conflicts
                                    if (!b.GetNeighbor(0).hasSign)
                                    {
                                        GameObject tempSign = Instantiate(s, b.transform.position, b.transform.rotation) as GameObject;
                                        tempSign.transform.parent = foundation.transform;
                                        b.hasSign = true;
                                        b.GetNeighbor(0).hasSign = true;
                                    }
                                }
                            }
                            break;
                    }
                    case signType.NorthSouthNeighbor:
                    {
                            foreach (BuildingChunk b in buildingChunksInQuestion)
                            {
                                int rand = Random.Range(0, s.GetComponent<SignChunk>().spawnRate);
                                //check for border chunk
                                if (b.transform.childCount > 0)
                                {
                                    //print(s.GetComponent<SignChunk>().westSide + " " + " " + b.transform.GetChild(0).name + b.transform.GetComponentInChildren<BuildingChunk>().westSideBorder + " " + b.transform.GetChild(0).GetComponent<BuildingChunk>().westSideBorder);
                                    //check for border on correct side
                                    if ((s.GetComponent<SignChunk>().westSide && b.transform.GetChild(0).GetComponent<BuildingChunk>().westSideBorder) || (!s.GetComponent<SignChunk>().westSide && !b.transform.GetChild(0).GetComponent<BuildingChunk>().westSideBorder))
                                    {
                                        //check positioning
                                        if (b.GetNeighbor(2) != null && b.GetNeighbor(3) != null && !b.hasSign && b.floorLevel > 3 && rand == 1)
                                        {
                                            //check neighbor conflicts
                                            if (!b.GetNeighbor(2).hasSign && !b.GetNeighbor(3).hasSign)
                                            {
                                                GameObject tempSign = Instantiate(s, b.transform.position, b.transform.rotation) as GameObject;
                                                tempSign.transform.parent = foundation.transform;
                                                b.hasSign = true;
                                                b.GetNeighbor(2).hasSign = true;
                                                b.GetNeighbor(3).hasSign = true;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                    default:
                    {
                        break;
                    }

                }
            }
        }
    }

    public List<BuildingChunk> GetBuildingChunksOnScreen()
    {
        BuildingChunk[] chunks = GameObject.FindObjectsOfType<BuildingChunk>();
        List<BuildingChunk> returnChunks = new List<BuildingChunk>();

        foreach (BuildingChunk bc in chunks)
        {
            if (bc.GetComponent<SpriteRenderer>().isVisible)
            {
                if (!bc.isBorder && !bc.isSky && !bc.isDoor)
                {
                    returnChunks.Add(bc);
                }
            }
        }

        return returnChunks;
    }

    void FixBorderCollider(GameObject o, int sign)
    {
        BoxCollider oldC = o.GetComponent<BoxCollider>();
        BoxCollider newC = o.AddComponent<BoxCollider>();
        newC.center = new Vector3(BORDERCOLLIDERCENTERX * sign, newC.center.y, newC.center.z);
        newC.size = new Vector3(BORDERCOLLIDERSIZEX, newC.size.y, newC.size.z);
        Destroy(oldC, 0.5f);
    }

    void SetNeghboors(GameObject foundation)
    {
        foreach(BuildingChunk b in foundation.GetComponentsInChildren<BuildingChunk>())
        {
            if (!b.isBorder && !b.isSky)
            {
                RaycastHit hitR;
                if (Physics.Linecast(b.GetComponent<Collider>().bounds.center, 
                        new Vector3(b.GetComponent<Collider>().bounds.center.x + b.GetComponent<Collider>().bounds.extents.x * 1.1f, b.GetComponent<Collider>().bounds.center.y, b.GetComponent<Collider>().bounds.center.z), out hitR, buildingChunks))
                {
                    if (b.transform.parent == hitR.transform.parent)
                    {
                        if (!hitR.collider.gameObject.GetComponent<BuildingChunk>().isBorder)
                        {
                            b.GetComponent<BuildingChunk>().SetBuildingNeighbor(hitR.collider.gameObject.GetComponent<BuildingChunk>(), 0);
                        }
                        if (!b.isBorder && hitR.collider.GetComponent<BuildingChunk>().isBorder)
                        {
                            hitR.collider.transform.parent = b.transform;
                        }
                    }
                }
                RaycastHit hitL;
                if (Physics.Linecast(b.GetComponent<Collider>().bounds.center, 
                        new Vector3(b.GetComponent<Collider>().bounds.center.x - b.GetComponent<Collider>().bounds.extents.x * 1.1f, b.GetComponent<Collider>().bounds.center.y, b.GetComponent<Collider>().bounds.center.z), out hitL, buildingChunks))
                {
                    if (b.transform.parent == hitL.transform.parent)
                    {
                        if (!hitL.collider.gameObject.GetComponent<BuildingChunk>().isBorder)
                        {
                            b.GetComponent<BuildingChunk>().SetBuildingNeighbor(hitL.collider.gameObject.GetComponent<BuildingChunk>(), 1);
                        }
                        if (!b.isBorder && hitL.collider.GetComponent<BuildingChunk>().isBorder)
                        {
                            hitL.collider.transform.parent = b.transform;
                        }
                    }
                }
                RaycastHit hitU;
                if (Physics.Linecast(b.GetComponent<Collider>().bounds.center, 
                        new Vector3(b.GetComponent<Collider>().bounds.center.x, b.GetComponent<Collider>().bounds.center.y + b.GetComponent<Collider>().bounds.extents.y * 1.1f, b.GetComponent<Collider>().bounds.center.z), out hitU, buildingChunks))
                {
                    if (b.transform.parent == hitU.transform.parent && !hitU.collider.gameObject.GetComponent<BuildingChunk>().isSky)
                    {
                        b.GetComponent<BuildingChunk>().SetBuildingNeighbor(hitU.collider.gameObject.GetComponent<BuildingChunk>(), 2);
                    }
                }
                RaycastHit hitD;
                if (Physics.Linecast(b.GetComponent<Collider>().bounds.center, 
                        new Vector3(b.GetComponent<Collider>().bounds.center.x, b.GetComponent<Collider>().bounds.center.y - b.GetComponent<Collider>().bounds.extents.y * 1.1f, b.GetComponent<Collider>().bounds.center.z), out hitD, buildingChunks))
                {
                    if (b.transform.parent == hitD.transform.parent)
                    {
                        b.GetComponent<BuildingChunk>().SetBuildingNeighbor(hitD.collider.gameObject.GetComponent<BuildingChunk>(), 3);
                    }
                }
            }
        }
    }
}
