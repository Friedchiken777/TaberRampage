using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour
{
    const float SPAWNOFFSET = 7;                        //how far off screen enemy spawns
    const float ZLAYER = -3;
    const float SKYSPAWNHEIGHT = 8;                     //Y values of sky spawning enemys
    const float SPAWNRADIUS = 50;

    public float spawnRate, spawnTimer, spawnRand;      //max time between spawns, how long since last spawn, how long till next spawn

    Vector3 spawnPoint;
    public CityBorderMarker[] markers;

    public LayerMask ground;
    public List<BuildingChunk> testChunk;

    public static EnemySpawnManager instance = null;

    // Use this for initialization
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        markers = GameObject.FindObjectsOfType<CityBorderMarker>();
        spawnRand = Random.Range(0, spawnRate);
        spawnPoint = markers[0].transform.position;

        testChunk = new List<BuildingChunk>();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (Mathf.Abs(spawnTimer + spawnRand) >= spawnRate)
        {
            spawnRand = Random.Range(0, spawnRate);
            spawnTimer = 0;

            GameObject enemyToSpawn = EnemySpawnNumbers.instance.PickEnemyToSpawn(TerrorManager.instance.currentTerrorLevel);

            if (enemyToSpawn != null)
            {
                switch (enemyToSpawn.GetComponent<EnemyParentScript>().spawnType)
                {
                    case EnemyParentScript.EnemySpawnTypes.Ground:
                        SpawnGroundEnemy(enemyToSpawn, false);
                        break;
                    case EnemyParentScript.EnemySpawnTypes.Window:
                        SpawnWindowEnemy(enemyToSpawn, false);
                        break;
                    case EnemyParentScript.EnemySpawnTypes.Roof:
                        SpawnWindowEnemy(enemyToSpawn, true);
                        break;
                    case EnemyParentScript.EnemySpawnTypes.Sky:
                        SpawnGroundEnemy(enemyToSpawn, true);
                        break;
                }
            }
            else
            {
                Debug.LogError("Couldn't spawn enemy");
            }
        }
    }

    void SpawnGroundEnemy(GameObject enemy, bool sky)
    {
        //Pick side of screen to spawn enemy
        int coin = Random.Range(0, 2);
        if (coin == 1)
        {
            spawnPoint = markers[coin].transform.position + new Vector3(SPAWNOFFSET, 0, -3);
        }
        else
        {
            spawnPoint = markers[coin].transform.position - new Vector3(SPAWNOFFSET, 0, 3);
        }

        //spawn enemy
        GameObject temp = Instantiate(enemy, spawnPoint, transform.rotation) as GameObject;

        //Put his feet on the ground or set him flying
        float yPos;
        if (sky)
        {
            spawnPoint.y = SKYSPAWNHEIGHT;
        }
        else
        {
            RaycastHit hit;
            if (Physics.Linecast(spawnPoint, Vector3.down, out hit, ground))
            {
                //print(hit.collider.name);
                yPos = hit.collider.transform.position.y + hit.collider.bounds.extents.y + temp.GetComponent<Collider>().bounds.extents.y;
                spawnPoint.y = yPos;
            }
        }

        temp.transform.position = spawnPoint;

    }

    void SpawnWindowEnemy(GameObject enemy, bool roof)
    {
        testChunk.Clear();
        bool processing = true;
        Collider[] col = Physics.OverlapSphere(GameObject.Find("Player").transform.position, SPAWNRADIUS);
        for (int i = 0; i < col.Length; i++)
        {
            if (col[i].GetComponent<BuildingChunk>() != null)
            {
                if (roof && col[i].GetComponent<BuildingChunk>().isSky)
                {
                    testChunk.Add(col[i].GetComponent<BuildingChunk>());
                }
                else if (!roof && !col[i].GetComponent<BuildingChunk>().isBorder && !col[i].GetComponent<BuildingChunk>().isDoor && !col[i].GetComponent<BuildingChunk>().isSky)
                {
                    testChunk.Add(col[i].GetComponent<BuildingChunk>());
                }
            }
        }
        if (testChunk.Count > 0)
        {
            int pos = 0;
            while (processing)
            {
                pos = Random.Range(0, testChunk.Count);

                if (!testChunk[pos].hasWindowEnemy && testChunk[pos].currentHealth == testChunk[pos].maxHealth && !testChunk[pos].hasSign)
                {
                    processing = false;
                }
                //invalid window to spawn
                else
                {
                    testChunk.Remove(testChunk[pos]);
                }

                if (testChunk.Count == 0)
                {
                    //print("No Spawnable Window");
                    return;
                }
            }
            if (testChunk.Count > 0 && testChunk[pos] != null)
            {
                Vector3 zOffset = new Vector3(testChunk[pos].transform.position.x, testChunk[pos].transform.position.y, ZLAYER);
                GameObject enemyTemp = Instantiate(enemy, zOffset, testChunk[pos].transform.rotation) as GameObject;
                testChunk[pos].hasWindowEnemy = true;
                enemyTemp.transform.parent = testChunk[pos].transform;
            }
        }
        else
        {
            //print("No Spawn Locations");
        }
    }
}