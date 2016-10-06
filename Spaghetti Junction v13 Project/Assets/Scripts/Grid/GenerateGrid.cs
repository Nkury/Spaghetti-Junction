using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GenerateGrid : MonoBehaviour {

    public GameObject tileObject;
    public GameObject markers;
    public Text gameName;
    public int gridWidth;
    public int gridHeight;

	// Use this for initialization
	void Start () {
        gridWidth = PlayerPrefs.GetInt("GridSize");
        gridHeight = PlayerPrefs.GetInt("GridSize");
        gameName.text = PlayerPrefs.GetString("Name");

        // construct grid using tile Object and two loops
        if(gridWidth > 0 && gridHeight > 0){
	        for(int i = 0; i < gridWidth; i++){
                for (int j = 0; j < gridHeight; j++)
                {
                    Instantiate(tileObject, new Vector3(i, 0, j), tileObject.transform.rotation);
                }
            }
        }

        this.gameObject.transform.position = new Vector3(gridWidth / 2, 5, gridHeight / 2); // start the camera in the center of the grid
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
