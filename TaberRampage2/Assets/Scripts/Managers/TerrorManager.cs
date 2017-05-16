using UnityEngine;
using System.Collections;

public class TerrorManager : MonoBehaviour
{
    const float TERRORTIMERHOLD = 1;                                    //seconds before terror starts dropping
    const int TERRORLEVEL1 = 100;
    const int TERRORLEVEL2 = 250;
    const int TERRORLEVEL3 = 600;
    const int TERRORLEVEL4 = 1400;
    const int TERRORLEVEL5 = 6000;
    const int TERRORLEVEL6 = 10000;
    const int TERRORLEVEL7 = 25000;
    const int TERRORLEVEL8 = 40000;
    const int MAXTERRORLOSS = 5;

    public static TerrorManager instance = null;

    float currentTerrorValue, previousTerrorValue, nextTerrorValue;     //current terror gage level, previous terror level for gage, next terror gage level for gage
    int playerScore;                                                    //player's score and overall destruction
    float terrorColdownTimer;                                           //time since last terror increase
    int terrorMultiplier;                                               // always one greater than actual terror level

    public int currentTerrorLevel;                                      //current terror value
    int terrorLoss;
    bool statNumbers;
    public static TerrorLevel terrorLevel;

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
        
        previousTerrorValue = 0;
        nextTerrorValue = TERRORLEVEL1;
        terrorLevel = TerrorLevel.T0;
        terrorMultiplier = 1;
        terrorLoss = 1;
        UpdateTerrorLevel();
        statNumbers = (StatisticsNumbers.instance != null);
        UpdateTerrorValue();
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        terrorColdownTimer += Time.deltaTime;
        if (terrorColdownTimer > TERRORTIMERHOLD)
        {
            RemoveTerror();
            terrorColdownTimer = 0;
            if (terrorLoss < MAXTERRORLOSS)
            {
                terrorLoss++;
            }
            else
            {
                terrorLoss = MAXTERRORLOSS;
            }
        }
	}

    public void AddTerror(float t)
    {
        t *= terrorMultiplier;
        int tInt = (int)Mathf.Round(t);
        playerScore += tInt;
        currentTerrorValue += tInt;
        UpdateTerrorLevel();
        terrorLoss = 1;
        //print(currentTerrorValue);
    }

    public void AddTerror()
    {
        playerScore += terrorMultiplier;
        currentTerrorValue += terrorMultiplier;
        UpdateTerrorLevel();
        terrorLoss = 1;
        //print(currentTerrorValue);
    }

    public void RemoveTerror(float t)
    {
        if (currentTerrorValue > 0)
        {
            t *= terrorMultiplier;
            int tInt = (int)Mathf.Round(t);
            currentTerrorValue -= tInt * terrorLoss;
            UpdateTerrorLevel();
            //print(currentTerrorValue);
        }
        else
        {
            currentTerrorValue = 0;
        }
    }

    public void RemoveTerror()
    {
        if (currentTerrorValue > 0)
        {
            currentTerrorValue -= terrorMultiplier * terrorLoss;
            UpdateTerrorLevel();
            //print(currentTerrorValue);
        }
        else
        {
            currentTerrorValue = 0;
        }
    }

    void UpdateTerrorLevel()
    {
        if (currentTerrorValue >= 0 && currentTerrorValue < TERRORLEVEL1 && terrorLevel != TerrorLevel.T0)
        {
            terrorLevel = TerrorLevel.T0;
            currentTerrorLevel = 0;
            terrorMultiplier = 1;
            previousTerrorValue = 0;
            nextTerrorValue = TERRORLEVEL1;
            UpdateTerrorValue();
            //print("T0");
        }
        else if (currentTerrorValue >= TERRORLEVEL1 && currentTerrorValue < TERRORLEVEL2 && terrorLevel != TerrorLevel.T1)
        {
            terrorLevel = TerrorLevel.T1;
            currentTerrorLevel = 1;
            terrorMultiplier = 2;
            previousTerrorValue = TERRORLEVEL1;
            nextTerrorValue = TERRORLEVEL2;
            UpdateTerrorValue();
            //print("T1");
        }
        else if (currentTerrorValue >= TERRORLEVEL2 && currentTerrorValue < TERRORLEVEL3 && terrorLevel != TerrorLevel.T2)
        {
            terrorLevel = TerrorLevel.T2;
            currentTerrorLevel = 2;
            terrorMultiplier = 3;
            previousTerrorValue = TERRORLEVEL2;
            nextTerrorValue = TERRORLEVEL3;
            UpdateTerrorValue();
            //print("T2");
        }
        else if (currentTerrorValue >= TERRORLEVEL3 && currentTerrorValue < TERRORLEVEL4 && terrorLevel != TerrorLevel.T3)
        {
            terrorLevel = TerrorLevel.T3;
            currentTerrorLevel = 3;
            terrorMultiplier = 4;
            previousTerrorValue = TERRORLEVEL3;
            nextTerrorValue = TERRORLEVEL4;
            UpdateTerrorValue();
            //print("T3");
        }
        else if (currentTerrorValue >= TERRORLEVEL4 && currentTerrorValue < TERRORLEVEL5 && terrorLevel != TerrorLevel.T4)
        {
            terrorLevel = TerrorLevel.T4;
            currentTerrorLevel = 4;
            terrorMultiplier = 5;
            previousTerrorValue = TERRORLEVEL4;
            nextTerrorValue = TERRORLEVEL5;
            UpdateTerrorValue();
            //print("T4");
        }
        else if (currentTerrorValue >= TERRORLEVEL5 && currentTerrorValue < TERRORLEVEL6 && terrorLevel != TerrorLevel.T5)
        {
            terrorLevel = TerrorLevel.T5;
            currentTerrorLevel = 5;
            terrorMultiplier = 6;
            previousTerrorValue = TERRORLEVEL5;
            nextTerrorValue = TERRORLEVEL6;
            UpdateTerrorValue();
            //print("T5");
        }
        else if (currentTerrorValue >= TERRORLEVEL6 && currentTerrorValue < TERRORLEVEL7 && terrorLevel != TerrorLevel.T6)
        {
            terrorLevel = TerrorLevel.T6;
            currentTerrorLevel = 6;
            terrorMultiplier = 7;
            previousTerrorValue = TERRORLEVEL6;
            nextTerrorValue = TERRORLEVEL7;
            UpdateTerrorValue();
            //print("T6");
        }
        else if (currentTerrorValue >= TERRORLEVEL7 && currentTerrorValue < TERRORLEVEL8 && terrorLevel != TerrorLevel.T7)
        {
            terrorLevel = TerrorLevel.T7;
            currentTerrorLevel = 7;
            terrorMultiplier = 8;
            previousTerrorValue = TERRORLEVEL7;
            nextTerrorValue = TERRORLEVEL8;
            UpdateTerrorValue();
            //print("T7");
        }
        GUIManager.instance.UpdateScore(playerScore);        
        GUIManager.instance.UpdateTerrorMeterIntensityGuageArrow((currentTerrorValue - previousTerrorValue) / (nextTerrorValue - previousTerrorValue));
        if (statNumbers)
        {
            StatisticsNumbers.instance.ModifyAverageMultiplier(terrorMultiplier);
        }
    }

    void UpdateTerrorValue()
    {
        currentTerrorValue = (nextTerrorValue + previousTerrorValue) / 2;
        GUIManager.instance.UpdateTerrorMeterLevelIndicator(terrorMultiplier - 1);
        if (statNumbers)
        {
            StatisticsNumbers.instance.ModifyHighestMultiplier(terrorMultiplier);
        }
    }

    public enum TerrorLevel
    {
        T0,
        T1,
        T2,
        T3,
        T4,
        T5,
        T6,
        T7,
        T8
    }
}
