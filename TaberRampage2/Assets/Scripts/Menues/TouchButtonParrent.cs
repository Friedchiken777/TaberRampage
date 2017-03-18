using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

[RequireComponent(typeof(PressGesture))]
public class TouchButtonParrent : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<PressGesture>().Pressed += PressHandler;
    }

    private void OnDisable()
    {
        GetComponent<PressGesture>().Pressed -= PressHandler;
    }

    void PressHandler(object sender, System.EventArgs e)
    {
        Functionality();
    }

    protected virtual void Functionality()
    {
        //What the button does. Defined by Children Scripts
    }
}
