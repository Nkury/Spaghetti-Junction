using UnityEngine;
using System.Collections;

public class Pedestrian : MonoBehaviour {

	public Transform goal;
	public bool setGoal = false;
	public GameObject spawnObject, planeManager, simManager, dayCycleManager;
    public GameObject parent;
	public float origSpeed;

	// Use this for initialization
	void Start () {
		NavMeshAgent agent = GetComponent<NavMeshAgent>();

		origSpeed = agent.speed;

		if (goal) {
			agent.destination = goal.position; 
			setGoal = true;
		}

		if (!planeManager) {
			planeManager = GameObject.Find ("PlaneManager");
		}

		if (!simManager) {
			simManager = GameObject.Find ("SimManager");
		}

		if (!dayCycleManager) {
			dayCycleManager = GameObject.Find ("DayCycle");
		}

		agent.speed = origSpeed * dayCycleManager.GetComponent<DayNightController> ().daySpeedMultiplier * 10;
	}
	
	// Update is called once per frame
	void Update () {
		NavMeshAgent agent = GetComponent<NavMeshAgent>();

		if (!setGoal && goal) {
			agent.destination = goal.position; 

			setGoal = true;
		} else if (simManager) {
			goal = simManager.GetComponent<SimManager> ().getRandomDestination (spawnObject);
		}

		if (!agent.pathPending) {
			if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance < agent.stoppingDistance && !agent.hasPath) {
				Debug.Log ("Agent made it to destination, destroying instance");
				Destroy (gameObject);
			} else if (agent.pathStatus == NavMeshPathStatus.PathInvalid) {
				Debug.Log ("Agent has invalid path, finding another");
				goal = simManager.GetComponent<SimManager> ().getRandomDestination (spawnObject);
			}
		}
	}

    void OnTriggerEnter(Collider other)
    {
        if(parent == null && other.gameObject.tag == "Objective")
        {
            parent = other.gameObject;
        }
        else if(other.gameObject.tag == "Objective" && other.gameObject != parent)
        {
            GameStats.pedestriansCommuted++;
            Destroy(this.gameObject);
        }
    }


}
