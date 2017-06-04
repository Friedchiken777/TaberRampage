using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartNewGameButton : TouchButtonParrent
{

    protected override void Functionality()
    {
        SceneManager.LoadSceneAsync("BlockoutScene", LoadSceneMode.Single);
    }
}
