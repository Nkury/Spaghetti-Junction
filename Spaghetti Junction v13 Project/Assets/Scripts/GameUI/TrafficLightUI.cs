using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrafficLightUI : MonoBehaviour {

    public GameObject stateManager;
    public Button closeButton;
    public Slider greenInterval;
    public Slider yellowInterval;
    public Slider redInterval;
    public InputField lightName;

    public GameObject trafficTile;

	// Use this for initialization
	void Start () {
    	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            clickCloseButton();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            setButton();
        }
	}

    public void clickCloseButton()
    {
        stateManager.GetComponent<StateManager>().setState(StateManager.GameStates.Editing);
        this.gameObject.SetActive(false);
    }

    public void setTrafficTile(GameObject tileObj)
    {
        trafficTile = tileObj;
        greenInterval.value = trafficTile.GetComponentInChildren<TrafficLightSystem>().greenLightTime;
        yellowInterval.value = trafficTile.GetComponentInChildren<TrafficLightSystem>().yellowLightTime;
        redInterval.value = trafficTile.GetComponentInChildren<TrafficLightSystem>().redLightTime;
        lightName.text = trafficTile.GetComponentInChildren<TrafficLightSystem>().trafficLightName;

    }

    public void setButton()
    {
        trafficTile.GetComponentInChildren<TrafficLightSystem>().greenLightTime = greenInterval.value;
        trafficTile.GetComponentInChildren<TrafficLightSystem>().yellowLightTime = yellowInterval.value;
        trafficTile.GetComponentInChildren<TrafficLightSystem>().redLightTime = redInterval.value;
        trafficTile.GetComponentInChildren<TrafficLightSystem>().setName(lightName.text);
        clickCloseButton();
    }
}
