using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour
{
    const float SPAWNOFFSET = 7;                        //how far off screen enemy spawns
    const float ZLAYER = -3;
    const float SKYSPAWNHEIGHT = 8;                     //Y values of sky spawning enemys
    const float SPAWNRADIUS = 50;
    const float SPEEDSPAWNMODIFYER = 2.5f;
    const float TERRORSPAWNMODIFYER = 0.2f;
    const float LONGESTSPAWNTIME = 3.0f;
    const float SHORTESTSPAWNTIME = 0.5f;

    [SerializeField][ReadOnly]                             //max time between spawns
    float spawnTimer, spawnRand, modifyedSpawnRate;        //how long since last spawn, how long till next spawn, time between spawns based on terror

    Vector3 spawnPoint;
    public CityBorderMarker[] markers;

    public LayerMask ground;
    public List<BuildingChunk> testChunk;

    public static EnemySpawnManager instance = null;

    Transform player;
    Vector3 playerSpeedOffset;
    bool playerMonsterController;
    int lastTerrorLevel;

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
        spawnRand = Random.Range(0, LONGESTSPAWNTIME);
        spawnPoint = markers[0].transform.position;

        testChunk = new List<BuildingChunk>();

        player = GameObject.Find("Player").transform;
        playerSpeedOffset = Vector3.zero;
        playerMonsterController = player.GetComponent<MonsterController>() != null;
        modifyedSpawnRate = LONGESTSPAWNTIME;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (Mathf.Abs(spawnTimer + spawnRand) >= modifyedSpawnRate)
        {
            if (playerMonsterController)
            {
                playerSpeedOffset = new Vector3(player.gameObject.GetComponent<MonsterController>().GetCurrentMoveSpeed() * SPEEDSPAWNMODIFYER, 0, 0);
            }

            ModifySpawnRate();
            spawnRand = Random.Range(0, modifyedSpawnRate);
            spawnTimer = 0;

            GameObject enemyToSpawn = EnemySpawnNumbers.instance.PickEnemyToSpawn(TerrorManager.instance.GetTerrorValue());

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
            spawnPoint = markers[coin].transform.position + new Vector3(SPAWNOFFSET + playerSpeedOffset.x, 0, ZLAYER);
        }
        else
        {
            spawnPoint = markers[coin].transform.position - new Vector3(SPAWNOFFSET - playerSpeedOffset.x, 0, -ZLAYER);
        }

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
                yPos = hit.collider.transform.position.y + hit.collider.bounds.extents.y + enemy.GetComponent<Collider>().bounds.extents.y;
                spawnPoint.y = yPos;
            }
        }

        Instantiate(enemy, spawnPoint, transform.rotation);

    }

    void SpawnWindowEnemy(GameObject enemy, bool roof)
    {
        testChunk.Clear();
        bool processing = true;
        Collider[] col = Physics.OverlapSphere(player.position + playerSpeedOffset, SPAWNRADIUS);

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
                if (testChunk[pos].windowOpenS != null)
                {
                    testChunk[pos].gameObject.GetComponent<SpriteRenderer>().sprite = testChunk[pos].windowOpenS;
                }
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

    void ModifySpawnRate()
    {
        if (TerrorManager.instance.GetTerrorValue() != lastTerrorLevel)
        {
            modifyedSpawnRate = LONGESTSPAWNTIME + (TerrorManager.instance.GetTerrorValue() * TERRORSPAWNMODIFYER);
            if (modifyedSpawnRate < SHORTESTSPAWNTIME)
            {
                modifyedSpawnRate = SHORTESTSPAWNTIME;
            }
            lastTerrorLevel = TerrorManager.instance.GetTerrorValue();
        }
    }
}