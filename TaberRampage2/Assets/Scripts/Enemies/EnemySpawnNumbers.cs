using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemySpawnNumbers : MonoBehaviour
{
    const int MAXTERRORLEVEL = 7;

    [SerializeField]
    EnemyPrefabs[] enemies = new EnemyPrefabs[MAXTERRORLEVEL +1];

    public static EnemySpawnNumbers instance = null; 

    // Use this for initialization
    void Start ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public GameObject PickEnemyToSpawn(int terrorLevel)
    {
        if (enemies.Length > 0)
        {
            //print("Enemy Picking Time!");
            List<EnemyPrefabs> spawnableEnemies = new List<EnemyPrefabs>();
            //Determine who can be spawned at given terror level
            foreach (EnemyPrefabs e in enemies)
            {
                if (e.terrorSpawnRate[terrorLevel] > 0)
                {
                    spawnableEnemies.Add(e);
                }
            }
            //Determine which group will be spawned
            //create probability list
            List<int> spawnPercents = new List<int>();
            spawnPercents.Add(0);
            foreach (EnemyPrefabs e in spawnableEnemies)
            {
                int range = spawnPercents[spawnPercents.Count - 1];
                range += e.terrorSpawnRate[terrorLevel];
                //print(range);
                spawnPercents.Add(range);
            }
            if (spawnPercents[spawnPercents.Count - 1] != 100)
            {
                Debug.LogError("Enemies at terror level " + terrorLevel + " do not have 100% spawn chance!");
                return null;
            }
            //choose group at random
            int groupChooser = UnityEngine.Random.Range(0, 100);
            int chosen;
            for (chosen = 0; chosen < spawnPercents.Count; chosen++)
            {
                if (chosen == spawnPercents.Count - 1)
                {
                    break;
                }
                if (groupChooser >= spawnPercents[chosen] && groupChooser < spawnPercents[chosen + 1])
                {
                    break;
                }
            }
            //Determine actual member of group to be spawned
            int enemyChooser = UnityEngine.Random.Range(0, spawnableEnemies[chosen].prefabs.Length);
            return spawnableEnemies[chosen].prefabs[enemyChooser];
        }
        else
        {
            Debug.LogError("No enemies to spawn...");
        }
        return null;
    }


    [Serializable]
    public class EnemyPrefabs
    {
        public string name;
        public int[] terrorSpawnRate = new int[MAXTERRORLEVEL + 1];
        public GameObject[] prefabs;
    }
}
