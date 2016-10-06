using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SimManager : MonoBehaviour {

	// Managers
	public GameObject stateManager, dayCycleManager;
	public GameObject timeText, simTimeText;
	public GameObject[] clockObjects;

	// Objects
	public List<GameObject> spawners;
	public GameObject cars, peds;

	public float simTime = 0.0f;
	public float totalSimTime = 0.0f;

	// Game states
	public enum SimState{Normal, FF2X, FF3X, Frozen};
	public SimState simState = SimState.Frozen, prevState = SimState.Frozen;

	// Options
	public int maxCars = 80, maxPeds = 80;

	// Use this for initialization
	void Start () {
		stateManager = GameObject.Find ("StateManager");
		dayCycleManager = GameObject.Find ("DayCycle");

		spawners = new List<GameObject> ();

		cars = new GameObject ("Cars");
		peds = new GameObject ("Pedestrians");
	}

	// Update is called once per frame
	void Update () {
		if (simState != SimState.Frozen) {
			calculateTimeUI ();
		}
	}

	public SimState getState() {
		return simState;
	}

	public void setState(SimState newState) {
		if (newState == simState)
			return;

		prevState = simState;

		float speed;

		if (simState == SimState.Frozen && newState != SimState.Frozen) {
			simTime = Time.time;
		} else if (newState == SimState.Frozen && simState != SimState.Frozen) {
			totalSimTime += (Time.time - simTime);
		}

		switch (newState) {
		case SimState.Normal:
			speed = 1.0f;
			break;
		case SimState.FF2X:
			speed = 2.0f;
			break;
		case SimState.FF3X:
			speed = 3.0f;
			break;
		case SimState.Frozen:
			speed = 0.0f;
			break;
		default:
			speed = 1.0f;
			break;
		}

		dayCycleManager.GetComponent<DayNightController> ().daySpeedMultiplier = speed / 10;

		foreach (NavMeshAgent agent in peds.GetComponentsInChildren<NavMeshAgent>()) {
			agent.speed = 3.5f * speed;
		}
			
		simState = newState;
	}

	public void setState() {
		setState (prevState);
	}

	public void simModeEnabled(bool enabled) {
		if (enabled) {
			simState = SimState.Normal;

			foreach (GameObject obj in spawners) {
				if (obj.GetComponent<PedSpawner>())
					obj.GetComponent<PedSpawner>().spawnEnabled = true;

				if(obj.GetComponent<Spawner>())
					obj.GetComponent<Spawner> ().spawnEnabled = true;

			}

			simTime = Time.time;
		} else {
			Debug.Log ("simModeEnabled set to false! Destroying all cars and peds");
			simState = SimState.Frozen;

			Destroy (cars);
			cars = new GameObject("Cars");

			Destroy (peds);
			peds = new GameObject ("Pedestrians");

            if (simTimeText) {
                simTimeText.GetComponent<Text>().text = "0 s";
            }

			resetSpawners ();
            resetCounts();

			calculateTimeUI ();
		}
	}

	public void addSpawner(GameObject spawner) {
		spawners.Add (spawner);
	}

	public void removeSpawner(GameObject spawner) {
		spawners.Remove (spawner);
	}

	public void resetSpawners() {
		foreach (GameObject spawner in spawners) {
			spawner.GetComponent<Spawner> ().occupiedSpawnPoints.Clear ();
		}
	}

    public void resetCounts()
    {
        GameStats.totalCommutedCars = 0;
        GameStats.pedestriansCommuted = 0;
    }

	public Transform getRandomDestination(GameObject spawnException)
	{
		List<GameObject> available = new List<GameObject> (spawners);
		if (spawnException) {
			available.Remove (spawnException);
		}

		GameObject randDestination = available [Random.Range (0, available.Count)];

		return randDestination.transform;
	}

	public void calculateTimeUI() {
		if (timeText) {
			timeText.GetComponent<Text> ().text = dayCycleManager.GetComponent<DayNightController> ().timeString;
		}
		if (clockObjects.Length > 0) {
			float currentTime = dayCycleManager.GetComponent<DayNightController> ().currentTime;
			float currentHour = Mathf.Floor (currentTime) % 12;
			float currentMinute = ((currentTime) - (Mathf.Floor(currentTime)))*60.0f;

			float hourRatio = currentHour / 12.0f;
			float minuteRatio = currentMinute / 60.0f;

			// Set the hands
			clockObjects[1].GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.back * ((hourRatio * 360f) + (minuteRatio * 360f / 12f)));
			clockObjects[2].GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.back * (currentMinute / 60.0f * 360f));
		}
		if (simTimeText) {
			simTimeText.GetComponent<Text> ().text = Mathf.Floor(totalSimTime + Time.time - simTime) + " s";
		}
	}

	public bool checkMaxCarCount() {
		Car[] spawnedCars = cars.GetComponentsInChildren<Car> ();
		return spawnedCars.Length < maxCars;
	}

	public bool checkMaxPedCount() {
		Pedestrian[] spawnedPeds = peds.GetComponentsInChildren<Pedestrian> ();
		return spawnedPeds.Length < maxPeds;
	}
}
