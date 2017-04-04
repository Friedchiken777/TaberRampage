using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsNumbers : MonoBehaviour
{
    [SerializeField][ReadOnly]
    float buildingChunksDestroyed, 
          buildingsDestroyed, 
          environmentObjectsDestroyed,
          totalPeopleEaten, 
          civilliansEaten,           
          totalHealthRecovered, 
          totalHealthLost,
          totalDashesPerformed;


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

    public void ModifyTotalDashesPerformed(float f)
    {
        totalDashesPerformed += f;
    }
    #endregion
}

