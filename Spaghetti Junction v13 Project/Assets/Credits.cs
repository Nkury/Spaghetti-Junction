using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Credits : MonoBehaviour {

    List<string> credits = new List<string>();

    // Use this for initialization
    void Start () {
        credits.Add(" The Team");
        credits.Add("Gameplay and UI Programmer\nJordan Ross");
        credits.Add("AI and UI Programmer\nNizar Kury");
        credits.Add("2D and 3D Artists\nLeigh Russel Moratin\nDrew O'Connor");
        credits.Add("2D Artist and Musician\nOdin Unger");
        credits.Add("  Music");
        credits.Add("Title Music\nBensound- Memories");
        credits.Add("Editing Music\nBensound- Jazz Comedy\nBensound- The Elevator\nBensound- The Lounge");
        credits.Add("Simulation Music\nBensound- Ukelele\nBensound- Pop Dance");
        credits.Add("Special Thanks To");
        credits.Add("Yoshiharu Kobayashi");
        credits.Add("Simple World");
        credits.Add("iTween");
        credits.Add("The CPI 441 Class of 2016");
        credits.Add("Thank you for playing!");
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.up * Time.deltaTime * .2f);
	}

    public IEnumerator startCredits()
    {
        for(int i = 0; i < credits.Count; i++)
        {
            if (ButtonManager.backPressed)
                break;
            this.GetComponent<Text>().text = credits[i];
            yield return new WaitForSeconds(2);
            if (ButtonManager.backPressed)
                break;
            if(i != credits.Count-1)
                this.GetComponent<Text>().text = "";
            yield return new WaitForSeconds(.5f);
            if (ButtonManager.backPressed)
                break;
        }

    }
}
