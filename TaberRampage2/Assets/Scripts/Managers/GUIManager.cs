using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour
{
    const float MAGICSCORETRANSFORMNUMBER = 10.63f;
    const float SCOREZEROYPOSITION = -285;

    const float ARROWHIGHDEGREES = 0;
    const float ARROWLOWDEGREES = -70;

    public static GUIManager instance = null;

    [SerializeField]
    Image monsterHealthBar;
    [SerializeField]
    Image monsterHealthHeart;
    [SerializeField]
    Image[] terrorMeterLevelIndicator, scoreNumberTracks;
    [SerializeField]
    Image terrorMeterIntensityGaugeArrow;

    int[] scoreArray = new int[10] {0,0,0,0,0,0,0,0,0,0};
    int[] previousScore = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    float resolutionOffset;

    // Use this for initialization
    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        /*if (Screen.width != 1024 && Screen.height != 600)
        {
            resolutionOffset = ((1024f - (float)Screen.width) / (600f - (float)Screen.height)) - (((float)Screen.width / (float)Screen.height) - (1024f / 600f));
        }
        else
        {
            resolutionOffset = 1;
        }
        
        print((1024f - (float)Screen.width) + " / " + (600f - (float)Screen.height) + " = " + ((1024f - (float)Screen.width) / (600f - (float)Screen.height)) + " - " + (((float)Screen.width / (float)Screen.height) - (1024f / 600f)) + " *=* " + resolutionOffset);*/

        resolutionOffset = (float)Screen.height / MAGICSCORETRANSFORMNUMBER;
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void UpdateScore(int s)
    {
        int storeScore = s;
        for (int i = scoreArray.Length - 1; s != 0; s /= 10)
        {
            scoreArray[i] = s % 10;            
            i--;
        }
        //print("(" + scoreArray[0] + ", " + scoreArray[1] + ", " + scoreArray[2] + ", " + scoreArray[3] + ", " + scoreArray[4] + ", " + scoreArray[5] + ", " + scoreArray[6] + ", " + scoreArray[7] + ", " + scoreArray[8] + ", " + scoreArray[9] + ")" + "(" + previousScore[0] + ", " + previousScore[1] + ", " + previousScore[2] + ", " + previousScore[3] + ", " + previousScore[4] + ", " + previousScore[5] + ", " + previousScore[6] + ", " + previousScore[7] + ", " + previousScore[8] + ", " + previousScore[9] + ")");
        for (int i = 0; i < scoreArray.Length; i++)
        {
            if (scoreArray[i] != previousScore[i])
            {
                scoreNumberTracks[i].rectTransform.transform.Translate(new Vector3(0, (scoreArray[i] - previousScore[i]) * resolutionOffset, 0));
            }
        }
        for (int i = scoreArray.Length - 1; storeScore != 0; storeScore /= 10)
        {
            previousScore[i] = storeScore % 10;
            i--;
        }
    }
    
    public void UpdateTerrorMeterLevelIndicator(int m)
    {
        int index = m;
        for (int i = 0; i < terrorMeterLevelIndicator.Length; i++)
        {
            if (i <= index)
            {
                terrorMeterLevelIndicator[i].enabled = true;
            }
            else
            {
                terrorMeterLevelIndicator[i].enabled = false;
            }
        }
    }

    public void UpdateTerrorMeterIntensityGuageArrow(float i)
    {
        //print("i = " + i + "result: " + i * ARROWLOWDEGREES);
        float zRot = ARROWLOWDEGREES - (i * ARROWLOWDEGREES);
        if (zRot <= ARROWHIGHDEGREES)
        {
            terrorMeterIntensityGaugeArrow.transform.localEulerAngles = new Vector3(0, 0, zRot);
        }
    }

    public void UpdateMosterHealthBar(float h)
    {
        monsterHealthBar.fillAmount = h;
        //monsterHealthHeart.fillAmount = h;
    }

}
