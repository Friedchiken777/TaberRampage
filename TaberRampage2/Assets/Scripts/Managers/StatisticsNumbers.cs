using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsNumbers : MonoBehaviour
{
    [SerializeField]
    [ReadOnly]
    float buildingChunksDestroyed,
          buildingsDestroyed,
          environmentObjectsDestroyed,
          totalPeopleEaten,
          civilliansEaten,
          totalHealthRecovered,
          totalHealthLost,
          totalDashesPerformed,
          totalHorizontalDistanceTraveled,
          totalVerticalDistanceTraveled,
          totalDistanceTraveledFromStart,
          totalBuildingsGenerated,
          highestMultiplier,
          averageMultiplier,
          gameTime,
          totalEnemiesSpawned,
          groundEnimiesSpawned,
          windowEnemiesSpawned,
          roofEnemiesSpawned,
          skyEnemiesSpawned;

    [SerializeField][ReadOnly]
    float[] multiplierTimes;

    int currentMultiplier;
    Vector3 playerPosition; 
    float lastMoveX, lastMoveY;

    public static StatisticsNumbers instance;

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
        gameTime = Mathf.Epsilon;
        multiplierTimes = new float[9];
    }

    private void Update()
    {
        multiplierTimes[currentMultiplier] += Time.deltaTime;
        gameTime += Time.deltaTime;
    }

    #region Getters
    public float GetBuildingChunksDestroyed()
    {
        return buildingChunksDestroyed;
    }

    public float GetBuildingsDestroyed()
    {
        return buildingsDestroyed;
    }

    public float GetEnvironmentObjectsDestroyed()
    {
        return environmentObjectsDestroyed;
    }

    public float GetTotalPeopleEaten()
    {
        return totalPeopleEaten;
    }

    public float GetCivilliansEaten()
    {
        return civilliansEaten;
    }

    public float GetTotalHealthRecovered()
    {
        return totalHealthRecovered;
    }

    public float GetTotalHealthLost()
    {
        return totalHealthLost;
    }

    public float GetTotalDashesPerformed()
    {
        return totalDashesPerformed;
    }

    public float GetTotalHorizontalDistanceTraveled()
    {
        return totalHorizontalDistanceTraveled;
    }

    public float GetTotalVerticalDistanceTraveled()
    {
        return totalVerticalDistanceTraveled;
    }

    public float GetTotalDistanceTraveledFromStart()
    {
        return totalDistanceTraveledFromStart;
    }

    public float GetTotalBuildingsGenerated()
    {
        return totalBuildingsGenerated;
    }

    public float GetHighestMultiplier()
    {
        return highestMultiplier;
    }

    public float GetAverageMultiplier()
    {
        return averageMultiplier;
    }
    #endregion

    #region Modifiers
    public void ModifyBuildingChunksDestroyed(float f)
    {
        buildingChunksDestroyed += f;
    }

    public void ModifyBuildingsDestroyed(float f)
    {
        buildingsDestroyed += f;
    }

    public void ModifyEnvironmentObjectsDestroyed(float f)
    {
        environmentObjectsDestroyed += f;
    }

    public void ModifyTotalPeopleEaten(float f)
    {
        totalPeopleEaten += f;
    }

    public void ModifyCivilliansEaten(float f)
    {
        civilliansEaten += f;
    }

    public void ModifyTotalHealthRecovered(float f)
    {
        totalHealthRecovered += f;
    }

    public void ModifyTotalHealthLost(float f)
    {
        totalHealthLost += f;
    }

    public void ModifyTotalDashesPerformed(float f = 1)
    {
        totalDashesPerformed += f;
    }

    public void ModifyTotalDistanceTraveled(Vector3 f)
    {
        totalHorizontalDistanceTraveled += Mathf.Abs(f.x - lastMoveX);

        if (f.y > lastMoveY)
        {
            totalVerticalDistanceTraveled += f.y - lastMoveY;
        }

        totalDistanceTraveledFromStart = playerPosition.x + f.x;

        lastMoveX = f.x;
        lastMoveY = f.y;
    }

    public void ModifyTotalBuildingsGenerated(float f = 1)
    {
        totalBuildingsGenerated += f;
    }

    public void ModifyHighestMultiplier(int f)
    {
        if (f > highestMultiplier)
        {
            highestMultiplier = f;
        }
        currentMultiplier = f;
    }

    public void ModifyAverageMultiplier(float f)
    {
        float tempSummer = 0;
        float gameTimeSnapShot = gameTime;
        for (int i = 1; i < multiplierTimes.Length; i++)
        {
            tempSummer += (i * multiplierTimes[i]);
        }
        averageMultiplier = tempSummer / gameTimeSnapShot;
    }
    #endregion

    public void SetPlayerPosition(Vector3 p)
    {
        playerPosition = p;
    }

    public void ModifyTotalEnemiesSpawned(float f = 1)
    {
        totalEnemiesSpawned += f;
    }

    public void ModifyTotalGroundEnemiesSpawned(float f = 1)
    {
        groundEnimiesSpawned += f;
    }

    public void ModifyTotalWindowEnemiesSpawned(float f = 1)
    {
        windowEnemiesSpawned += f;
    }

    public void ModifyTotalRoofEnemiesSpawned(float f = 1)
    {
        roofEnemiesSpawned += f;
    }

    public void ModifyTotalSkyEnemiesSpawned(float f = 1)
    {
        skyEnemiesSpawned += f;
    }
}

