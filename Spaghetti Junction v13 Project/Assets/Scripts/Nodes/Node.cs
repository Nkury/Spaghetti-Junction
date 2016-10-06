using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public bool occupied = false; // TRUE if car is colliding with it; FALSE otherwise
    public List<Node> connectedNodes;
    public List<GameObject> connectedObjectives;

    // Use this for initialization
    void Start()
    {
        connectedNodes = new List<Node>();
        connectedObjectives = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
		
    }


    void OnTriggerEnter(Collider other)
    {

        // for removing unneeded nodes
		List<Node> connectedNodesCopy = new List<Node> (connectedNodes);
		List<GameObject> connectedObjectivesCopy = new List<GameObject> (connectedObjectives);

		foreach (Node n in connectedNodesCopy)
        {
            if (!n)
                connectedNodes.Remove(n);
        }

		foreach (GameObject g in connectedObjectivesCopy)
        {
            if (!g)
                connectedObjectives.Remove(g);
        }

        // adds nodes if they are not objectives
        if (gameObject.tag != "Objective")
        {
            if (other.gameObject.tag == "Node" || other.gameObject.tag == "IntersectionNode")
            {
                Node node = other.gameObject.GetComponent<Node>();
                if (!connectedNodes.Contains(node))
                {
                    connectedNodes.Add(node);
                }
            }
            else if (other.gameObject.tag == "Objective")
            {
                if (!connectedObjectives.Contains(other.gameObject))
                {
                    connectedObjectives.Add(other.gameObject);
                }
            }   
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (gameObject.tag != "Objective")
        {
            if (other.gameObject.tag == "Node" || other.gameObject.tag == "IntersectionNode")
            {
                Node node = other.gameObject.GetComponent<Node>();
                if (connectedNodes.Contains(node))
                {
                    connectedNodes.Remove(node);
                }
            }
            else if (other.gameObject.tag == "Objective")
            {
                if (connectedObjectives.Contains(other.gameObject))
                {
                    connectedObjectives.Remove(other.gameObject);
                }
            }
        }
    }

}
