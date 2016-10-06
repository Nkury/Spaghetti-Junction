using UnityEngine;
using System.Collections;

public class TrafficLightSystem : MonoBehaviour {

	public GameObject stateManager, dayCycleManager;

	public string trafficLightName;

    public GameObject[] horzLights;
    public GameObject[] vertLights;

    public Material redLight;
    public Material yellowLight;
    public Material greenLight;

    public float greenLightTime;
    public float yellowLightTime;
	public float redLightTime;

    private float time = 0;
	private bool lastLight; // true is horz, false is vert

	// Use this for initialization
	void Start () {
		stateManager = GameObject.Find ("StateManager");
		dayCycleManager = GameObject.Find ("DayCycle");

        horzLights[0].GetComponent<Renderer>().material = greenLight;
        horzLights[1].GetComponent<Renderer>().material = greenLight;
        vertLights[0].GetComponent<Renderer>().material = redLight;
        vertLights[1].GetComponent<Renderer>().material = redLight;
    }
	
	// Update is called once per frame
	void Update () {
		if (stateManager.GetComponent<StateManager> ().getState() != StateManager.GameStates.Simulating)
			return;
		
		time += Time.deltaTime * dayCycleManager.GetComponent<DayNightController>().daySpeedMultiplier * 10;
		if (time > greenLightTime + yellowLightTime + redLightTime) {
			time = 0;
			if (lastLight) {
				vertLights [0].GetComponent<Collider> ().enabled = true;
				vertLights [1].GetComponent<Collider> ().enabled = true;
				vertLights [0].GetComponent<Renderer> ().material = greenLight;
				vertLights [1].GetComponent<Renderer> ().material = greenLight;
				vertLights [0].tag = "Green Light";
				vertLights [1].tag = "Green Light";

				gameObject.GetComponent<NavMeshObstacle> ().size = new Vector3 (15.0f,2.0f,21.0f);
			} else {
				horzLights[0].GetComponent<Renderer>().material = greenLight;
				horzLights[1].GetComponent<Renderer>().material = greenLight;
				horzLights[0].tag = "Green Light";
				horzLights[1].tag = "Green Light";
				horzLights[0].GetComponent<Collider>().enabled = true;
				horzLights[1].GetComponent<Collider>().enabled = true;
				gameObject.GetComponent<NavMeshObstacle> ().size = new Vector3 (21.0f,2.0f,15.0f);
			}
		}
        else if(time > greenLightTime + yellowLightTime)
        {
            
            if (horzLights[0].tag == "Yellow Light")
            {
                horzLights[0].GetComponent<Renderer>().material = redLight;
                horzLights[1].GetComponent<Renderer>().material = redLight;
                horzLights[0].tag = "Red Light";
                horzLights[1].tag = "Red Light";
				lastLight = true;
               
            }
            else if (vertLights[0].tag == "Yellow Light")
            {
                vertLights[0].GetComponent<Renderer>().material = redLight;
                vertLights[1].GetComponent<Renderer>().material = redLight;
                vertLights[0].tag = "Red Light";
                vertLights[1].tag = "Red Light";
				lastLight = false;
               
            }

        } else if (time > greenLightTime)
        {
            if(horzLights[0].tag == "Green Light")
            {
                horzLights[0].GetComponent<Renderer>().material = yellowLight;
                horzLights[1].GetComponent<Renderer>().material = yellowLight;
                horzLights[0].tag = "Yellow Light";
                horzLights[1].tag = "Yellow Light";
                vertLights[0].GetComponent<Collider>().enabled = false;
                vertLights[1].GetComponent<Collider>().enabled = false;
            }
            else if(vertLights[0].tag == "Green Light"){
                vertLights[0].GetComponent<Renderer>().material = yellowLight;
                vertLights[1].GetComponent<Renderer>().material = yellowLight;
                vertLights[0].tag = "Yellow Light";
                vertLights[1].tag = "Yellow Light";
                horzLights[0].GetComponent<Collider>().enabled = false;
                horzLights[1].GetComponent<Collider>().enabled = false;
            }
        }
	}

	public void setName(string name)
	{
		trafficLightName = name;
	}
}
