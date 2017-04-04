using UnityEngine;
using System.Collections;
using System;

public class BuildingChunk : MonoBehaviour
{
    const float DAMAGECOOLDOWN = 0.175f;
    const float DAMAGEEXPLODEOUT = 0.5f;
    const int MAXNEIGBORRADIUS = 2;

    public Material clean, damaged, broken;
    public Sprite cleanS, damagedS, brokenS;
    public bool isSky, isBorder, isDoor, hasWindowEnemy, hasSign, westSideBorder;
    public float maxHealth, floorLevel;
    public float currentHealth;
    float damageCooldownCounter, damageExplosionCounter;
    bool explodeDamage;
    [SerializeField]
    NeighborList[] neighbors = new NeighborList[4];
    int timesNeigborsHit;
    bool statNumbers;

    // Use this for initialization
    void Start ()
    {
        gameObject.layer = LayerMask.NameToLayer("BuildingChunk");
        currentHealth = maxHealth;
        if (clean != null)
        {
            GetComponent<Renderer>().material = clean;
        }
        if (cleanS != null)
        {
            GetComponent<SpriteRenderer>().sprite = cleanS;
        }
        neighbors[0].name = "East";
        neighbors[1].name = "West";
        neighbors[2].name = "North";
        neighbors[3].name = "South";
        statNumbers = (StatisticsNumbers.instance != null);
    }
	
	// Update is called once per frame
	void Update ()
    {
        damageCooldownCounter += Time.deltaTime;
        if (explodeDamage)
        {
            damageExplosionCounter += Time.deltaTime;
        }
        if (damageExplosionCounter > DAMAGEEXPLODEOUT)
        {
            explodeDamage = false;
            damageExplosionCounter = 0;
            timesNeigborsHit = 0;
        }
	}

    public void TakeDamage(bool canExplode)
    {
        if (damageCooldownCounter > DAMAGECOOLDOWN && !isBorder && !isSky)
        {
            if (currentHealth > 0)
            {
                currentHealth--;
                transform.parent.GetComponent<Building>().currentHealth--;
                if (currentHealth / maxHealth <= 0.5f && currentHealth > 0)
                {
                    if (damaged != null)
                    {
                        GetComponent<Renderer>().material = damaged;
                        if (transform.childCount > 0 && transform.GetChild(0).GetComponent<BuildingChunk>() != null)
                        {

                            transform.GetChild(0).GetComponent<Renderer>().material = transform.GetChild(0).GetComponent<BuildingChunk>().damaged;
                        }
                    }
                    if (damagedS != null)
                    {
                        GetComponent<SpriteRenderer>().sprite = damagedS;
                        if (transform.childCount > 0 && transform.GetChild(0).GetComponent<BuildingChunk>() != null)
                        {
                            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<BuildingChunk>().damagedS;
                        }
                    }
                }
                if (currentHealth <= 0)
                {
                    if (broken != null)
                    {
                        GetComponent<Renderer>().material = broken;
                        if (transform.childCount > 0 && transform.GetChild(0).GetComponent<BuildingChunk>() != null)
                        {
                            transform.GetChild(0).GetComponent<Renderer>().material = transform.GetChild(0).GetComponent<BuildingChunk>().broken;
                        }
                    }
                    if (brokenS != null)
                    {
                        GetComponent<SpriteRenderer>().sprite = brokenS;
                        if (transform.childCount > 0 && transform.GetChild(0).GetComponent<BuildingChunk>() != null)
                        {
                            transform.GetComponentInChildren<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<BuildingChunk>().brokenS;
                        }
                    }
                    if (statNumbers)
                    {
                        StatisticsNumbers.instance.ModifyBuildingChunksDestroyed(1);
                        statNumbers = false;
                    }
                }

                TerrorManager.instance.AddTerror();
            }
            if (canExplode && timesNeigborsHit < MAXNEIGBORRADIUS)
            {
                damageCooldownCounter = 0;
                if (explodeDamage)
                {
                    DamageNeigbors(this.gameObject.GetComponent<BuildingChunk>());
                }
                explodeDamage = true;
                damageExplosionCounter = 0;
            }
        }
    }

    void DamageNeigbors(BuildingChunk b)
    {
        for(int i = 0; i < b.neighbors.Length; i++)
        {
            if (b.neighbors[i].neighbor != null)
            {
                b.neighbors[i].neighbor.GetComponent<BuildingChunk>().TakeDamage(true);
            }
        }
        timesNeigborsHit++;
    }

    public void SetBuildingNeighbor(BuildingChunk b, int i)
    {
        neighbors[i].neighbor = b;
    }

    public BuildingChunk GetNeighbor(int i)
    {
        if (i < neighbors.Length)
        {
            return neighbors[i].neighbor;
        }
        else
        {
            return null;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (isSky && col.GetComponent<MonsterController>() != null)
        {
            col.GetComponent<MonsterController>().SetState();
            col.GetComponent<MonsterController>().Movment();
        }
    }

    [Serializable]
    public class NeighborList
    {
        public string name;
        public BuildingChunk neighbor;
    }
}
