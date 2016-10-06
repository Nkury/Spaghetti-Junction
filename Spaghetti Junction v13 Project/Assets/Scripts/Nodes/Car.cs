using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Car : MonoBehaviour
{

    public bool go; // TRUE if moving, FALSE if stopped

    public float speed, origSpeed;

    public Node target;
    public Node parent;

	public bool isTurning = false;

	public NodeDetection nodeDetection;
	private GameObject planeManager, stateManager, dayCycleManager;

    public void SetCar(Node tParent)
    {
        parent = tParent;
        go = true;
        target = null;
    }

    // Use this for initialization
    void Start()
    {
		speed = origSpeed = 10;

		planeManager = GameObject.Find ("PlaneManager");
		stateManager = GameObject.Find ("StateManager");

		if (!dayCycleManager) {
			dayCycleManager = GameObject.Find ("DayCycle");
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (stateManager.GetComponent<StateManager>().getState () != StateManager.GameStates.Simulating)
			return;

        // POSSIBLE CONFLICT WITH GAMESTATS
        if(parent != null && parent.gameObject.tag == "Objective")
        {
            GameStats.totalCommutedCars++;
            GameStats.carsOnScreen--;
            Destroy(this.gameObject);
        }

		float velocity = speed * Time.deltaTime * dayCycleManager.GetComponent<DayNightController>().daySpeedMultiplier * 10;

        if (target == null) // if we don't have a target, pick one
        {
            target = nodeDetection.GetNextNode();

			if (target && gameObject) {
				Vector3 targetPosition = Vector3.zero;

				if (target.gameObject.tag == "Objective") {
					targetPosition = target.GetComponent<Spawner> ().closestNodePosToPos (transform.position);
				} else {
					targetPosition = target.gameObject.transform.position;
				}

				Vector3 targetVector = targetPosition - transform.position;

				Vector3 cross = Vector3.Cross (targetVector.normalized, gameObject.transform.forward);
				//Debug.Log ("targetVector: " + targetVector + ", forward: " + gameObject.transform.forward + ", Cross: " + cross);

				if (cross.magnitude > 0.5f) {
					isTurning = true;
					iTween.LookTo (gameObject, iTween.Hash ("looktarget", targetPosition, "time", 0.5f, "oncomplete", "OnFinishTurning", "oncompletetarget", gameObject));
				}
			}
        }

		if (target != null && go && !blockedByCar() && !isTurning) // if there are no targets, don't move
        {
			Vector3 targetPos = target.transform.position;
			if (target.gameObject.tag == "Objective") {
				targetPos = target.GetComponent<Spawner> ().closestNodePosToPos (transform.position);
			}
			transform.position = Vector3.MoveTowards(transform.position, targetPos, velocity);
            transform.LookAt(targetPos);
        }
    }

    void OnTriggerEnter(Collider other)
    {
		if (!target)
			return;
		
        if(other.gameObject.tag == "Red Light" && target.gameObject.tag == "IntersectionNode")
        {
//            Debug.Log("I got here");
            go = false;
        }
        else if(other.gameObject.tag == "Green Light")
        {
            go = true;
        } 

        if (other.gameObject.tag == "Node")
        {
            other.GetComponent<Node>().occupied = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Node")
        {
            other.GetComponent<Node>().occupied = false;
        }
    }

	bool blockedByCar() {
		RaycastHit hit;

		float rayDistance = 5.5f;

        Vector3 startingPoint = transform.position + Vector3.up;
		Vector3 leftPoint = startingPoint + transform.right;
		Vector3 rightPoint = startingPoint - transform.right;

		if (Physics.Raycast (leftPoint, transform.forward, out hit, rayDistance, Physics.AllLayers, QueryTriggerInteraction.UseGlobal)) {
			Debug.DrawLine (leftPoint, leftPoint + transform.forward * rayDistance, Color.cyan);
			Transform objectHit = hit.transform;
			if (objectHit.tag == "Car" || objectHit.tag == "Pedestrian") {
				Debug.DrawLine (leftPoint, leftPoint + transform.forward * rayDistance, Color.red);
				return true;
			}
		} else {
			Debug.DrawLine (leftPoint, leftPoint + transform.forward * rayDistance, Color.green);
		}

		if (Physics.Raycast (rightPoint, transform.forward, out hit, rayDistance, Physics.AllLayers, QueryTriggerInteraction.UseGlobal)) {
			Debug.DrawLine (rightPoint, rightPoint + transform.forward * rayDistance, Color.cyan);
			Transform objectHit = hit.transform;
			if (objectHit.tag == "Car" || objectHit.tag == "Pedestrian") {
				Debug.DrawLine (rightPoint, rightPoint + transform.forward * rayDistance, Color.red);
				return true;
			}
		} else {
			Debug.DrawLine (rightPoint, rightPoint + transform.forward * rayDistance, Color.green);
		}
		return false;
	}

	void OnFinishTurning() {
		isTurning = false;
	}

}
