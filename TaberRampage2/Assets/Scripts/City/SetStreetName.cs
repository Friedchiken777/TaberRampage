using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetStreetName : MonoBehaviour {

    public List<string> StreetA;
    public List<string> StreetB;
    public List<string> StreetC;

    private bool addingA;
    private bool addingB;

    public TextMesh signText;

	void Start () 
    {
        signText = GetComponentInChildren<TextMesh>();

        TextAsset nameText = Resources.Load<TextAsset>("streets");
        string [] Lines = nameText.text.Split("\n"[0]);

        for (int i = 0; i < Lines.Length; i++) //look through list and find headers to add female or male names to different lists
        {
            if (Lines[i].Contains("A:"))
            {
                addingA = true;
                //Debug.Log("Adding A");
                continue;
            }
            if (Lines[i].Contains("B:"))
            {
                addingA = false;
                addingB = true;
                //Debug.Log("Adding B");
                continue;
            }    
            if (Lines[i].Contains("C:"))
            {
                addingA = false;
                addingB = false;
                //Debug.Log("Adding C");
                continue;
            }

            if (Lines[i].Contains("---") != true)
            {
                if (addingA)
                {
                    StreetA.Add(Lines[i]);
                }
                else if (addingB)
                {
                    StreetB.Add(Lines[i]);
                }
                else
                {
                    StreetC.Add(Lines[i]);
                }
            }
        }

        signText.text = generateName();
	}
	
    string generateName ()
    {
        string stA;
        string stB;
        string stC;

        stA = StreetA[Random.Range(0, StreetA.Count)];
        stB = StreetB[Random.Range(0, StreetB.Count)];
        stC = StreetC[Random.Range(0, StreetC.Count)];

        string StreetName = (stA + stB + stC);

        return StreetName;
    }
}
