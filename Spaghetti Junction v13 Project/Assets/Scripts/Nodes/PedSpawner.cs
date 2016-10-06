using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PedSpawner : MonoBehaviour {

	public bool spawnEnabled = false;
	public List<GameObject> peds, spawnedPeds;
	public int xVal, yVal, type, xSize, ySize;
    public float timeForPed;

    public GameObject dayCycle, stateManager, simManager;

    private float time = 0;
	public int maxPeds = 20;

    // Use this for initialization
	void Start () {
		dayCycle = GameObject.Find("DayCycle");
		stateManager = GameObject.Find ("StateManager");
		simManager = GameObject.Find("SimManager");

		spawnedPeds = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (!spawnEnabled || stateManager.GetComponent<StateManager>().getState() != StateManager.GameStates.Simulating)
			return;

        time += Time.deltaTime;

        calculateTime();

        if (timeForPed >= 1 && time > timeForPed + Random.Range(-0.25f, 0.25f))
        {
			if (!checkSpawnedPedsCount() || !simManager.GetComponent<SimManager>().checkMaxPedCount()) {
				return;
			}

            time = 0; 
			int randNumPeds = Random.Range (1, 4);
			for (int i = 0; i < randNumPeds; i++) {
				int rand = Random.Range (0, peds.Count);
				GameObject ped = (GameObject)Instantiate (peds [rand], gameObject.transform.position, gameObject.transform.rotation);
				ped.GetComponent<Pedestrian> ().spawnObject = gameObject;

				ped.transform.parent = simManager.GetComponent<SimManager> ().peds.transform;
			}

			//GameStats.totalPeds++;
            //GameStats.pedsOnScreen++;
        }    

	}

    void calculateTime()
    {
        float daytime = dayCycle.GetComponent<DayNightController>().currentTime;

        if(daytime < 8)
        {
            timeForPed = 30 / Mathf.Sqrt(daytime);
        } else if (daytime >= 8 && daytime < 11)
        {
            timeForPed = Mathf.Sqrt(daytime) * 1.5f;
        } else if (daytime >= 11 && daytime < 13)
        {
            timeForPed = 12;
        } else if (daytime >= 13 && daytime < 17)
        {
            timeForPed = 20 / Mathf.Sqrt(daytime);
        } else if (daytime >= 17 && daytime <= 24)
        {
            timeForPed = 5f * Mathf.Sqrt(daytime) * daytime / 24;
        }
    }

	bool checkSpawnedPedsCount() {
		List<GameObject> pedsCopy = new List<GameObject> (spawnedPeds);
		foreach (GameObject ped in pedsCopy) {
			if (ped == null) {
				spawnedPeds.Remove (ped);
			}
		}

		return spawnedPeds.Count < maxPeds;
	}

	public Vector3 closestNodePosToPos(Vector3 position) {
		Vector3 closestPos = Vector3.zero;
		float closestDist = float.MaxValue;
		for (int i = -xSize; i <= xSize; i++) {
			for (int j = -ySize; j <= ySize; j++) {
				Vector3 checkPos = transform.position + new Vector3 (i * 20, 0, j * 20);
				if (Vector3.Distance (checkPos, position) < closestDist) {
					closestPos = checkPos;
					closestDist = Vector3.Distance (checkPos, position);
				}
			}
		}
		return closestPos;
	}
}
