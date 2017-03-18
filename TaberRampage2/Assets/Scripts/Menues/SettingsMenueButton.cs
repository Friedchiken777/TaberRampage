using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public class SettingsMenueButton : TouchButtonParrent
{ 
    [SerializeField]
    GameObject touchControls;
    [SerializeField]
    Material pauseMat, playMat;
    [SerializeField]
    GameObject[] menueItems;

    bool paused = false;

    protected override void Functionality()
    {
        if (paused)
        {
            paused = false;
            Time.timeScale = 1;
            touchControls.gameObject.SetActive(true);
            GetComponent<Renderer>().material = pauseMat;
            foreach (GameObject g in menueItems)
            {
                g.SetActive(false);
            }
        }
        else
        {
            paused = true;
            Time.timeScale = 0;
            touchControls.gameObject.SetActive(false);
            GetComponent<Renderer>().material = playMat;
            foreach (GameObject g in menueItems)
            {
                g.SetActive(true);
            }
        }
    }
}
