using UnityEngine;
using System.Collections;

public class ChangeTileColor : MonoBehaviour {

    public GameObject[] newTiles;
    private int tileSelection = 0;
    private bool set;



	// Use this for initialization
	void Start () {
        set = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            tileSelection++;
        }

        if (tileSelection >= newTiles.Length)
        {
            tileSelection = 0;
        }
    }

    void OnMouseEnter()
    {
        if(!set)
            this.gameObject.GetComponent<Renderer>().material.color = Color.green;
    }

    void OnMouseExit()
    {
        if (!set)
            this.gameObject.GetComponent<Renderer>().material.color = Color.white;
    }

    void OnMouseDown()
    {
        if (!set)
        {
            switch (tileSelection)
            {
                case 0: Instantiate(newTiles[tileSelection], new Vector3(transform.position.x + .5f, transform.position.y + .05f, transform.position.z + .5f), newTiles[tileSelection].transform.rotation);
                    break;
                case 1: Instantiate(newTiles[tileSelection], new Vector3(transform.position.x + .5f, transform.position.y + .05f, transform.position.z + .5f), newTiles[tileSelection].transform.rotation);
                    break;
                case 2: Instantiate(newTiles[tileSelection], new Vector3(transform.position.x, transform.position.y + .05f, transform.position.z + .5f), newTiles[tileSelection].transform.rotation);
                    break;
                case 3: Instantiate(newTiles[tileSelection], new Vector3(transform.position.x + .52f, transform.position.y + .05f, transform.position.z + .5f), newTiles[tileSelection].transform.rotation);
                    break;
                default: Instantiate(newTiles[tileSelection], new Vector3(transform.position.x, transform.position.y + .05f, transform.position.z + .5f), newTiles[tileSelection].transform.rotation);
                    break;
            }
        }
        set = true;
    }
}
