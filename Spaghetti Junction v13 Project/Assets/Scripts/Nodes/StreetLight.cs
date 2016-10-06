using UnityEngine;
using System.Collections;

public class StreetLight : MonoBehaviour {

    public GameObject dayTime;
    public Light streetLight;

	// Use this for initialization
	void Start () {
        dayTime = GameObject.Find("DayCycle");
	}
	
	// Update is called once per frame
	void Update () {
		if(dayTime.GetComponent<DayNightController>().currentTime > 17 || dayTime.GetComponent<DayNightController>().currentTime < 8)
        {
            streetLight.enabled = true;
        }
        else
        {
            streetLight.enabled = false;
        }
	}
}
