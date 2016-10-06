using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{

    public bool spawnEnabled = false;

    public List<Node> spawnpoints;
    public List<Node> occupiedSpawnPoints;

    public List<GameObject> cars;
    public int xVal, yVal, type;
    public float timeForCar;

    public GameObject trappedCarCounter;
    public GameObject dayCycle, stateManager, simManager;

    public bool isCity;

    private GameObject mainCamera;
	private List<GameObject> spawnedCars;

    public static int totalTrappedCars;
    private int trappedCars = 0;
    private float time = 0;

	public int xSize = 0, ySize = 0;

	public int maxCars = 40;

    // Use this for initialization
    void Start()
    {
        spawnpoints = new List<Node>();
		spawnedCars = new List<GameObject> ();

        dayCycle = GameObject.Find("DayCycle");
        mainCamera = GameObject.Find("Main Camera");
        stateManager = GameObject.Find("StateManager");
		simManager = GameObject.Find("SimManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera && trappedCarCounter)
        {
            trappedCarCounter.GetComponent<TextMesh>().text = trappedCars.ToString();
            trappedCarCounter.transform.LookAt(mainCamera.transform.position);
        }

        if (stateManager.GetComponent<StateManager>().getState() != StateManager.GameStates.Simulating)
        {
            trappedCars = 0;
            totalTrappedCars = 0;
            return;
        }

		if (!spawnEnabled || simManager.GetComponent<SimManager>().getState() == SimManager.SimState.Frozen)
            return;

        time += Time.deltaTime;

        calculateTime();

        if (timeForCar >= 1 && time > timeForCar + Random.Range(-0.5f, 0.5f))
        {
//			if (!checkSpawnedCarsCount() || !simManager.GetComponent<SimManager> ().checkMaxCarCount ()) {
//				return;
//			}

            time = 0;
            foreach (Node n in spawnpoints)
            {
                if (!n)
                    continue;

                if (!occupiedSpawnPoints.Contains(n))
                {
                    occupiedSpawnPoints.Add(n);
                    int rand = Random.Range(0, cars.Count);
                    GameObject car = (GameObject)Instantiate(cars[rand], n.transform.position, n.transform.rotation);
                    if (n.connectedNodes.Count != 0)
                    {
                        car.transform.LookAt(n.connectedNodes[0].gameObject.transform.position);
                    }
                    else if(n.connectedObjectives.Count != 0)
                    {
                        car.transform.LookAt(n.connectedObjectives[0].gameObject.transform.position);
                    }
					car.transform.parent = simManager.GetComponent<SimManager> ().cars.transform;
					spawnedCars.Add (car);
                    car.SendMessage("SetCar", n);
                    GameStats.totalCars++;
                    GameStats.carsOnScreen++;
                }
                else
                {
                    trappedCars++;
                    totalTrappedCars++;
                }
            }
        }

    }

    void calculateTime()
    {
        float daytime = dayCycle.GetComponent<DayNightController>().currentTime;
        if (!isCity && simManager.GetComponent<SimManager>().simState != SimManager.SimState.Frozen)
        {
                if (daytime < 8)
                {
                    timeForCar = 6 / ((float)simManager.GetComponent<SimManager>().simState + 1);
                }
                else if (daytime >= 8 && daytime < 11)
                {
                    timeForCar = 4 / ((float)simManager.GetComponent<SimManager>().simState + 1);
                }
                else if (daytime >= 11 && daytime < 13)
                {
                    timeForCar = 6 / ((float)simManager.GetComponent<SimManager>().simState + 1);
                }
                else if (daytime >= 13 && daytime <= 24)
                {
                    timeForCar = 10 / ((float)simManager.GetComponent<SimManager>().simState + 1);
                }
        }
        else if(simManager.GetComponent<SimManager>().simState != SimManager.SimState.Frozen)
        {
            if (daytime < 11)
            {
                timeForCar = 12 / ((float)simManager.GetComponent<SimManager>().simState + 1);
            }
            else if (daytime >= 13 && daytime < 20)
            {
                timeForCar = 4 / ((float)simManager.GetComponent<SimManager>().simState + 1);
            }
            else if (daytime >= 20 && daytime <= 24)
            {
                timeForCar = 8 / ((float)simManager.GetComponent<SimManager>().simState + 1);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Node" || other.gameObject.tag == "IntersectionNode")
        {
            Node node = other.gameObject.GetComponent<Node>();
            if (!spawnpoints.Contains(node) && !node.connectedObjectives.Contains(this.gameObject))
            {
                spawnpoints.Add(node);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Node" || other.gameObject.tag == "IntersectionNode")
        {
            Node node = other.gameObject.GetComponent<Node>();
            if (spawnpoints.Contains(node))
            {
                spawnpoints.Remove(node);
            }
        }
        else if (other.gameObject.tag == "Car")
        {
            if (occupiedSpawnPoints.Contains(other.gameObject.GetComponent<Car>().parent))
                occupiedSpawnPoints.Remove(other.gameObject.GetComponent<Car>().parent);
        }
    }

	bool checkSpawnedCarsCount() {
		List<GameObject> carsCopy = new List<GameObject> (spawnedCars);
		foreach (GameObject car in carsCopy) {
			if (car == null) {
				spawnedCars.Remove (car);
			}
		}

		return true;
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
