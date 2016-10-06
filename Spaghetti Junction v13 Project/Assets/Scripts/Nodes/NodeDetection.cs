using UnityEngine;
using System.Collections;

public class NodeDetection : MonoBehaviour {

	public Car car;

    public int probabilityToTurn = 100; // probability that the car will turn-- default is 100%
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if (car.target != null && other.transform.parent != null && other.transform.parent.gameObject == car.target.gameObject)
		{
			car.parent = car.target;
			car.target = null;
		}
	}


	// returns the next node that the car will travel to by 
	// selecting nodes that are 20 units away from parent
	public Node GetNextNode()
	{
		if (car.parent != null)
		{
			if(car.parent.connectedObjectives.Count > 0)
			{
				car.go = true;
				if (car.parent.connectedObjectives.Count > 1)
				{
					int rand = Random.Range(0, car.parent.connectedObjectives.Count);
					return car.parent.connectedObjectives[rand].GetComponent<Node>();
				} else if (car.parent.connectedObjectives.Count == 1)
				{
                    return car.parent.connectedObjectives[0].GetComponent<Node>();
				}                     
			} else if (car.parent.connectedNodes.Count > 0)
			{
				car.go = true;
				if (car.parent.connectedNodes.Count > 1)
				{
                    if (Random.Range(0, 101) <= probabilityToTurn)
                    {
                        probabilityToTurn -= 60;
                        int rand = Random.Range(0, car.parent.connectedNodes.Count);
                        return car.parent.connectedNodes[rand];
                    }
                    else
                    {
                        return GetSingleNode();
                    }
				}
				else
				{
                    if(car.parent.connectedNodes[0].tag != "IntersectionNode")
                        probabilityToTurn = 100;
					return car.parent.connectedNodes[0];
				}
			}

		}

		return null;
	}

    // checks if target node has a connected node 
    public Node GetSingleNode()
    {
        Node n = car.parent.connectedNodes[0];
        if(n.connectedNodes.Count == 1 && n.connectedNodes[0].tag != "IntersectionNode"){
            return n;
        } else {
            return car.parent.connectedNodes[1];
        }
    }
}
