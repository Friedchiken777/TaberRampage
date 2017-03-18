using UnityEngine;
using System.Collections;

public class QuiteButton : TouchButtonParrent
{
    protected override void Functionality()
    {
        Application.Quit();
    }

}
