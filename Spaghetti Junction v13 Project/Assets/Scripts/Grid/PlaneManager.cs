using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlaneManager : MonoBehaviour {
	public int gridWidth, gridHeight;
	public Text gameName;
	public GameObject gameButtonManager, simManager, stateManager, trafficLightUI;

	// Prefab types
	public GameObject floorPrefab;
	public GameObject[] tiles;
	public GameObject[] neighborhoods, cities, entertainments, trains, airports;

	// int enum for determining if we are placing street, neighborhood, city, etc...
	public enum Type{Street, Neighborhood, City, Entertainment, Train, Airport};
	public enum StreetTypes{Straight, Corner, TIntersection, Intersection};
	public Dictionary<Type, Vector2> typeSizes = new Dictionary<Type, Vector2>();
	private Type tileIndex = Type.Street;

	// Objects for tile placement
	private List<GameObject> coloredFloors, coloredErasingObjects;
	private GameObject lastFloor, currentGhost, currentForcedTile, currentForcedGhostTile;
	private Type forcedTileIndex;

	// Game states
	public enum State{Placing, ForcePlacing, Deleting};
	private State buildState = State.Placing, prevState = State.Placing;

	private GameObject[,] placedTiles;
	private bool onUI = false; // boolean for determining if mouse is hovering over UI and disabling being able to click a road down
	private bool justPlacedItem = false;
	private bool isErasingTiles = false;

	// Use this for initialization
	void Start () {

		// Player Prefs
		gridWidth = PlayerPrefs.GetInt("GridSize");
		gridHeight = PlayerPrefs.GetInt("GridSize");
		gameName.text = PlayerPrefs.GetString ("Name");

		// Managers Setup
		simManager = GameObject.Find ("SimManager");
		stateManager = GameObject.Find ("StateManager");
		gameButtonManager = GameObject.Find ("ButtonManager");

		// Grid Initialization
		float gridWidthF, gridHeightF;
		if (gridWidth == 0) {
			gridWidth = 10;
		}
		if (gridHeight == 0) {
			gridHeight = 10;
		}

		// Object Initialization
		placedTiles = new GameObject[gridWidth, gridHeight];
		GameObject gridTiles = new GameObject("Grid Tiles");
		coloredFloors = new List<GameObject> ();
		coloredErasingObjects = new List<GameObject> ();

		// Add sizes of each tile for reference in methods
		typeSizes.Add (Type.Street, new Vector2 (0, 0));
		typeSizes.Add (Type.Neighborhood, new Vector2 (1, 1));
		typeSizes.Add (Type.City, new Vector2 (1, 1));
		typeSizes.Add (Type.Entertainment, new Vector2 (1, 1));
		typeSizes.Add (Type.Train, new Vector2 (1, 1));
		typeSizes.Add (Type.Airport, new Vector2 (2, 2));

		// Create the grid and set values
		gridWidthF = (float)gridWidth;
		gridHeightF = (float)gridHeight;
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeight; j++) {
				float x = ((float)i - gridWidthF / 2.0f) * 20f + 10;
				float y = ((float)j - gridHeightF / 2.0f) * 20f;

				GameObject floor = (GameObject)Instantiate (floorPrefab, new Vector3 (x, 0, y), Quaternion.identity);
				floor.GetComponent<GridTile>().xVal = i;
				floor.GetComponent<GridTile>().yVal = j;
				floor.transform.parent = gridTiles.transform;

				placedTiles [i, j] = floor;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		// tiles is array of possible tiles to place
		if (tiles.Length == 0)
			return;

		// We're not editing right now
		if (stateManager.GetComponent<StateManager> ().getState () != StateManager.GameStates.Editing)
			return;

		// Make sure the mouse isn't on the UI
		if (!onUI)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				Transform objectHit = hit.transform;

				if (objectHit.gameObject.tag != "Floor")
					return;

				int xVal, yVal;

				if (!lastFloor) {
					lastFloor = objectHit.gameObject;
					xVal = lastFloor.GetComponent<GridTile> ().xVal;
					yVal = lastFloor.GetComponent<GridTile> ().yVal;
				}

				if (buildState == State.Deleting) {
					GameObject currentFloor = objectHit.gameObject;

					// if clicked and they have a floor highlighted we need to delete
					if (Input.GetMouseButtonDown (0)) {
						if (currentFloor && currentFloor.GetComponent<GridTile> ().blocked) {
							removeTile (currentFloor.GetComponent<GridTile> ().xVal, currentFloor.GetComponent<GridTile> ().yVal);

							foreach (GameObject obj in coloredErasingObjects) {
								if (obj) {
									iTween.ColorUpdate (obj, Color.white, 0.01f);
								}
							}
							coloredErasingObjects.Clear ();
						}
					} else if (!currentFloor.Equals(lastFloor)) {
						if (coloredErasingObjects.Count > 0) {
							foreach (GameObject obj in coloredErasingObjects) {
								if (obj) {
									iTween.ColorUpdate (obj, Color.white, 0.01f);
								}
							}
							coloredErasingObjects.Clear ();
						}
						lastFloor = currentFloor;
					} else {
						if (currentFloor.GetComponent<GridTile> ().blocked) {
							coloredErasingObjects.Clear ();
							coloredErasingObjects.Add (currentFloor.GetComponent<GridTile> ().tileObject);
							coloredErasingObjects.Add (currentFloor);

							foreach (GameObject obj in coloredErasingObjects) {
								if (obj) {
									iTween.ColorUpdate (obj, Color.red, 2.5f);
								}
							}
						}
					}
				}
				else if (buildState == State.Placing)
				{
					if (Input.GetMouseButtonDown (0) && lastFloor) {
						xVal = lastFloor.GetComponent<GridTile> ().xVal;
						yVal = lastFloor.GetComponent<GridTile> ().yVal;

						justPlacedItem = placeTile (xVal, yVal);

						if (justPlacedItem && tileIndex != Type.Street) {
							if (tileIndex != Type.Train && tileIndex != Type.Airport && numberOfConnectedTilesForTileType (xVal, yVal, tileIndex) < 1) {
								forcedTileIndex = tileIndex;
								currentForcedTile = lastFloor;
								buildState = State.ForcePlacing;

								if (gameButtonManager) {
									gameButtonManager.GetComponent<GameButtonManager> ().forceButton (0);
								}
								Debug.Log ("BuildState now at ForcePlacing");
							} else if (gameButtonManager) {
								gameButtonManager.GetComponent<GameButtonManager> ().selectButton (0, false);
							}

							tileIndex = Type.Street;
						} else {
							// Tile is blocked
							if (lastFloor.GetComponent<GridTile> ().blocked) {
								StreetTypes streetType = (StreetTypes)lastFloor.GetComponent<GridTile> ().streetType;

								if (streetType == StreetTypes.Intersection || streetType == StreetTypes.TIntersection) {
									// BRING UP STOPLIGHT GUI

									trafficLightUI.SetActive(true);
									trafficLightUI.GetComponent<TrafficLightUI>().setTrafficTile(lastFloor);
									stateManager.GetComponent<StateManager>().setState(StateManager.GameStates.Paused);

									GameObject intersection = lastFloor.GetComponent<GridTile> ().tileObject;
								}
							}
						}
					} else if (!objectHit.gameObject.Equals (lastFloor)) {
						lastFloor = objectHit.gameObject;

						destroyGhost ();

						if (!(lastFloor.GetComponent<GridTile> ().blocked && justPlacedItem)) {
							xVal = lastFloor.GetComponent<GridTile> ().xVal;
							yVal = lastFloor.GetComponent<GridTile> ().yVal;

							createGhost (xVal, yVal);
							justPlacedItem = false;
						}
					} else if (!isTileFreeAtPos(lastFloor.GetComponent<GridTile> ().xVal, lastFloor.GetComponent<GridTile> ().yVal)) {
						xVal = lastFloor.GetComponent<GridTile> ().xVal;
						yVal = lastFloor.GetComponent<GridTile> ().yVal;

						if (!justPlacedItem)
						{
							colorTiles (xVal, yVal, Color.red);
						}
					} else {
						xVal = lastFloor.GetComponent<GridTile> ().xVal;
						yVal = lastFloor.GetComponent<GridTile> ().yVal;

						colorTiles (xVal, yVal, Color.green);
						if (justPlacedItem)
						{
							justPlacedItem = false;
						}
					}

					xVal = lastFloor.GetComponent<GridTile> ().xVal;
					yVal = lastFloor.GetComponent<GridTile> ().yVal;

					if (!currentGhost && !objectHit.gameObject.GetComponent<GridTile>().blocked && !isErasingTiles)
					{
						createGhost (xVal, yVal);
					}
				}
				else if (buildState == State.ForcePlacing)
				{
					if (!currentForcedTile) {
						buildState = State.Placing;
						Debug.Log ("No currentForcedTile, BuildState now at Placing");
						return;
					}

					if (Input.GetMouseButtonDown (0) && currentForcedGhostTile) {
						justPlacedItem = placeTile (currentForcedGhostTile.GetComponent<GridTile> ().xVal, currentForcedGhostTile.GetComponent<GridTile> ().yVal);

						if (justPlacedItem) {
							buildState = State.Placing;
							Debug.Log ("ForcePlace successful, BuildState now at Placing");
							gameButtonManager.GetComponent<GameButtonManager> ().releaseButton ();
						}
					}  else if (!objectHit.gameObject.Equals (lastFloor) || !currentGhost) {
						lastFloor = objectHit.gameObject;

						xVal = lastFloor.GetComponent<GridTile> ().xVal;
						yVal = lastFloor.GetComponent<GridTile> ().yVal;

						int xValForced = -1, yValForced = -1;

						if (currentForcedGhostTile) {
							xValForced = currentForcedGhostTile.GetComponent<GridTile> ().xVal;
							yValForced = currentForcedGhostTile.GetComponent<GridTile> ().yVal;
						}

						Vector2 xyVal = closestForcedTileToPos (xVal, yVal);

						if (!currentForcedGhostTile || !Vector2.Equals(xyVal, new Vector2(xValForced, yValForced))) {
							destroyGhost ();

							createGhost ((int)xyVal.x,(int)xyVal.y);
							currentForcedGhostTile = placedTiles[(int)xyVal.x,(int)xyVal.y];
						}
					} else if (!isTileFreeAtPos(currentForcedGhostTile.GetComponent<GridTile> ().xVal, currentForcedGhostTile.GetComponent<GridTile> ().yVal)) {
						xVal = currentForcedGhostTile.GetComponent<GridTile> ().xVal;
						yVal = currentForcedGhostTile.GetComponent<GridTile> ().yVal;

						if (!justPlacedItem)
						{
							colorTiles (xVal, yVal, Color.red);
						}
					} else {
						xVal = currentForcedGhostTile.GetComponent<GridTile> ().xVal;
						yVal = currentForcedGhostTile.GetComponent<GridTile> ().yVal;

						colorTiles (xVal, yVal, Color.green);
						if (justPlacedItem)
						{
							justPlacedItem = false;
						}
					}
				}
			}
		}
	}

	void finishAnimation(string name) {
		iTween[] tweens = GetComponents<iTween>();
		foreach (iTween tween in tweens) {
			if (tween.name == name) {
				tween.time = 0;
				tween.SendMessage("Update");
			}
		}
	}

	public void mouseOnUI()
	{
		destroyGhost ();
		onUI = true;
	}

	public void mouseOffUI()
	{
		onUI = false;
	}

	public State getState()
	{
		return buildState;
	}

	public void simModeEnabled(bool enabled) {
		if (enabled) {
			destroyGhost ();
		}
	}

	public void enableEraserMode()
	{
		buildState = State.Deleting;
		Debug.Log ("BuildState now at Deleting");
	}

	//******************//
	//  Ghost methods   //
	//******************//

	public void setGhost(int index)
	{
		if (buildState == State.Placing || buildState == State.Deleting) {
			tileIndex = (Type)Mathf.Clamp (index, 0, 5);
			buildState = State.Placing;
			Debug.Log ("BuildState now at Placing");
		}
	}

	public void createGhost(int x, int y)
	{
		GameObject tile = placedTiles[x,y];

		GameObject ghostType;
		int tileType, tileRot;
		switch (tileIndex) {

		case Type.Street:
			tileType = tileTypeAtPos (tile.GetComponent<GridTile> ().xVal, tile.GetComponent<GridTile> ().yVal);
			tileRot = tileRotAtPos (tile.GetComponent<GridTile> ().xVal, tile.GetComponent<GridTile> ().yVal);
			ghostType = tiles [tileType + 4];
			break;
		case Type.Neighborhood:
			ghostType = neighborhoods [1];
			tileRot = 0;
			break;
		case Type.City:
			ghostType = cities [1];
			tileRot = 0;
			break;
		case Type.Entertainment:
			ghostType = entertainments [1];
			tileRot = 0;
			break;
		case Type.Train:
			ghostType = trains [1];
			tileRot = 2;
			break;
		case Type.Airport:
			ghostType = airports [1];
			tileRot = 2;
			break;
		default:
			ghostType = tiles [4];
			tileRot = 0;
			break;
		}

		currentGhost = (GameObject)Instantiate (ghostType, tile.transform.position + Vector3.up * 0.3f, Quaternion.Euler (Vector3.up * tileRot * 90));
		currentGhost.name = "Ghost Object";
		iTween.Init (currentGhost);
		iTween.FadeUpdate (currentGhost, 0.5f, 0.01f);
		iTween.MoveTo(currentGhost, iTween.Hash("name", "ghost", "position", tile.transform.position + Vector3.up * 2, "time", 0.5f));
	}

	public void destroyGhost()
	{
		if (!currentGhost)
			return;
		Destroy (currentGhost);

		foreach (GameObject floor in coloredFloors) {
			iTween.ColorUpdate(floor, floorPrefab.GetComponent<Renderer> ().sharedMaterial.color, 0.01f);
		}
		coloredFloors.Clear ();
	}

	public void resetGhost()
	{
		destroyGhost ();

		if (lastFloor && !lastFloor.GetComponent<GridTile> ().blocked) {
			int xVal = lastFloor.GetComponent<GridTile> ().xVal;
			int yVal = lastFloor.GetComponent<GridTile> ().yVal;

			createGhost (xVal, yVal);
		}
	}

	//**************//
	// Tile methods //
	//**************//

	public bool placeTile(int x, int y)
	{
		bool successful = false;
		switch (tileIndex) {
		case Type.Street:
			successful = placeStreet (x, y);
			break;
		case Type.City:
		case Type.Neighborhood:
		case Type.Entertainment:
		case Type.Train:
		case Type.Airport:
			successful = placeNonStreetTile (x, y);
			break;
		}

		if (successful) {
			if (spawnerWithRoadExists ()) {
				gameButtonManager.GetComponent<GameButtonManager> ().setSimButtonEnabled (true);
			}
		}

		return successful;
	}

	public bool removeTile(int x, int y)
	{
		if (x < 0 || x > gridWidth - 1 || y < 0 || y > gridHeight - 1)
			return false;

		GameObject tile = placedTiles[x,y];

		if (!tile.GetComponent<GridTile> ().blocked)
			return false;

		Type tileType = (Type)tile.GetComponent<GridTile> ().type;

		Vector2 xySize;
		int xSize = 0, ySize = 0;

		if (typeSizes.TryGetValue (tileType, out xySize)) {
			xSize = (int)xySize.x;
			ySize = (int)xySize.y;
		}

		switch (tileType) {
		case Type.City:
		case Type.Neighborhood:
		case Type.Entertainment:
		case Type.Train:
		case Type.Airport:
			simManager.GetComponent<SimManager> ().removeSpawner (tile.GetComponent<GridTile> ().tileObject);
			break;
		}

		Destroy (tile.GetComponent<GridTile> ().tileObject);

		int xCenter = tile.GetComponent<GridTile> ().xCenter;
		int yCenter = tile.GetComponent<GridTile> ().yCenter;
		for (int i = -xSize; i <= xSize; i++) {
			for (int j = -ySize; j <= ySize; j++) {
				GameObject newTile = placedTiles [xCenter + i, yCenter + j];
				newTile.GetComponent<GridTile> ().blocked = false;
				tile.GetComponent<GridTile>().rotVal = 0;
				tile.GetComponent<GridTile>().type = 0;
				tile.GetComponent<GridTile>().streetType = 0;
				tile.GetComponent<GridTile> ().xCenter = 0;
				tile.GetComponent<GridTile> ().yCenter = 0;
				tile.GetComponent<GridTile> ().xSize = 0;
				tile.GetComponent<GridTile> ().ySize = 0;
				tile.GetComponent<GridTile> ().tileObject = null;
			}
		}

		for (int i = -(xSize + 1); i <= (xSize + 1); i++) {
			for (int j = -(ySize + 1); j <= (ySize + 1); j++) {
				if (Mathf.Abs (i) == Mathf.Abs (j))
					continue;

				if (Mathf.Abs (i) != (xSize + 1) && Mathf.Abs (j) != (ySize + 1))
					continue;
				
				updateTile (xCenter + i, yCenter + j);
			}
		}

		if (!spawnerWithRoadExists ()) {
			gameButtonManager.GetComponent<GameButtonManager> ().setSimButtonEnabled (false);
		}

		return true;
	}

	public bool placeNonStreetTile(int x, int y)
	{
		if (x <= 0 || x >= gridWidth || y <= 0 || y >= gridHeight)
			return false;

		Vector2 xySize;
		int xSize = 0, ySize = 0, rotVal = 0;

		if (typeSizes.TryGetValue (tileIndex, out xySize)) {
			xSize = (int)xySize.x;
			ySize = (int)xySize.y;
		} else {
			Debug.Log ("Attempted to get size for value " + tileIndex + " but it did not exist!");
			return false;
		}

		GameObject centerTile = placedTiles[x,y];
		for (int i = -xSize; i <= xSize; i++) {
			for (int j = -ySize; j <= ySize; j++) {
				if (x + i <= gridWidth - 1 && x + i >= 0 && y + j <= gridHeight - 1 && y + j >= 0) {
					GameObject tile = placedTiles [x + i, y + j];
					if (tile.GetComponent<GridTile> ().blocked)
						return false;
				} else
					return false;
			}
		}
		if (centerTile.GetComponent<GridTile>().blocked)
			return false;

		GameObject largeObject;
		switch (tileIndex) {
		case Type.Neighborhood:
			largeObject = neighborhoods [0];
			placedNeighborhood (x, y);
			break;
		case Type.City:
			largeObject = cities [0];
			placedCity (x, y);
			break;
		case Type.Entertainment:
			largeObject = entertainments [0];
			placedEntertainment (x, y);
			break;
		case Type.Train:
			largeObject = trains [0];
			rotVal = 2;
			break;
		case Type.Airport:
			largeObject = airports [0];
			rotVal = 2;
			break;
		default:
			return false;
		}

		GameObject solid = (GameObject)Instantiate(largeObject, centerTile.transform.position + Vector3.up * 2, Quaternion.Euler (Vector3.up * rotVal * 90));
		solid.transform.parent = centerTile.transform;

		iTween.Init (solid);
		iTween.FadeUpdate(solid, 0.5f, 0.01f);
		iTween.FadeTo(solid, 1.0f, 1.0f);
		iTween.MoveTo(solid, iTween.Hash("EaseType", "easeInBack", "delay", 0.05f, "position", lastFloor.transform.position + Vector3.up * 0.3f, "time", 0.75f));

		for (int i = -xSize; i <= xSize; i++) {
			for (int j = -ySize; j <= ySize; j++) {
				GameObject tile = placedTiles[x + i,y + j];
				tile.GetComponent<GridTile>().blocked = true;
				tile.GetComponent<GridTile>().rotVal = 0;
				tile.GetComponent<GridTile>().type = (int)tileIndex;
				tile.GetComponent<GridTile> ().xCenter = x;
				tile.GetComponent<GridTile> ().yCenter = y;
				tile.GetComponent<GridTile> ().xSize = xSize;
				tile.GetComponent<GridTile> ().ySize = ySize;
				tile.GetComponent<GridTile> ().tileObject = solid;
			}
		}

		for (int i = -(xSize + 1); i <= (xSize + 1); i++) {
			for (int j = -(ySize + 1); j <= (ySize + 1); j++) {
				if (Mathf.Abs (i) == Mathf.Abs (j))
					continue;

				if (Mathf.Abs (i) != (xSize + 1) && Mathf.Abs (j) != (ySize + 1))
					continue;

				updateTile (x + i, y + j);
			}
		}
			
		if (tileIndex != Type.Train && tileIndex != Type.Entertainment && tileIndex != Type.Airport) {
			solid.GetComponent<Spawner> ().xVal = x;
			solid.GetComponent<Spawner> ().yVal = y;
			solid.GetComponent<Spawner> ().xSize = xSize;
			solid.GetComponent<Spawner> ().ySize = ySize;
			solid.GetComponent<Spawner> ().type = (int)tileIndex;

		} else {
			solid.GetComponent<PedSpawner> ().xVal = x;
			solid.GetComponent<PedSpawner> ().yVal = y;
			solid.GetComponent<Spawner> ().xSize = xSize;
			solid.GetComponent<Spawner> ().ySize = ySize;
			solid.GetComponent<PedSpawner> ().xSize = xSize;
			solid.GetComponent<PedSpawner> ().ySize = ySize;
			solid.GetComponent<PedSpawner> ().type = (int)tileIndex;
		}

		simManager.GetComponent<SimManager> ().addSpawner (solid);

		Destroy(currentGhost);
		return true;
	}

	public void placedNeighborhood(int x, int y)
	{
		// extra stuff for neighborhood
	}

	public void placedCity(int x, int y)
	{
		// extra stuff for city
	}

	public void placedEntertainment(int x, int y)
	{
		// extra stuff for entertainment
	}

	public bool placeStreet(int x, int y)
	{
		GameObject tile = placedTiles[x,y];
		if (tile.GetComponent<GridTile>().blocked)
			return false;

		tile.GetComponent<GridTile>().blocked = true;

		int tileType = tileTypeAtPos (tile.GetComponent<GridTile> ().xVal, tile.GetComponent<GridTile> ().yVal);
		int tileRot = tileRotAtPos (tile.GetComponent<GridTile> ().xVal, tile.GetComponent<GridTile> ().yVal);

		GameObject solid = (GameObject)Instantiate(tiles[tileType], tile.transform.position + Vector3.up * 2, Quaternion.Euler(new Vector3(0, tileRot * 90, 0)));
		solid.transform.parent = tile.transform;
		tile.GetComponent<GridTile> ().tileObject = solid;

		iTween.Init (solid);
		iTween.FadeUpdate(solid, 0.5f, 0.01f);
		iTween.FadeTo(solid, 1.0f, 1.0f);
		if (tileType == 0)
			iTween.MoveTo(solid, iTween.Hash("EaseType", "easeInBack", "delay", 0.05f, "position", tile.transform.position + Vector3.up * 0.3f, "time", 0.75f));
		else
			iTween.MoveTo(solid, iTween.Hash("EaseType", "easeInBack", "delay", 0.05f, "position", tile.transform.position + Vector3.up * 0.3f, "time", 0.75f));

		tile.GetComponent<GridTile>().rotVal = tileRot;
		tile.GetComponent<GridTile>().type = 0;
		tile.GetComponent<GridTile>().streetType = tileType;
		tile.GetComponent<GridTile> ().xCenter = x;
		tile.GetComponent<GridTile> ().yCenter = y;
		tile.GetComponent<GridTile> ().xSize = 0;
		tile.GetComponent<GridTile> ().ySize = 0;

		updateTile (x - 1, y);
		updateTile (x + 1, y);
		updateTile (x, y - 1);
		updateTile (x, y + 1);

		Destroy(currentGhost);

		return true;
	}

	public void updateTile(int x, int y)
	{
		if (x < 0 || x > gridWidth - 1 || y < 0 || y > gridHeight - 1)
			return;

		GameObject tile = placedTiles[x,y];

		if (!tile.GetComponent<GridTile> ().blocked || (Type)tile.GetComponent<GridTile> ().type != Type.Street)
			return;

		int tileType = tileTypeAtPos (tile.GetComponent<GridTile> ().xVal, tile.GetComponent<GridTile> ().yVal);
		int tileRot = tileRotAtPos (tile.GetComponent<GridTile> ().xVal, tile.GetComponent<GridTile> ().yVal);

		if (tile.GetComponent<GridTile>().streetType == tileType && tile.GetComponent<GridTile>().rotVal == tileRot)
			return;

		Destroy (tile.GetComponent<GridTile> ().tileObject);

		GameObject solid = (GameObject)Instantiate(tiles[tileType], tile.transform.position + Vector3.up * 0.3f, Quaternion.Euler(new Vector3(0, tileRot * 90, 0)));
		solid.transform.parent = tile.transform;

		Vector2 xySize;
		int xSize = 0, ySize = 0;

		if (typeSizes.TryGetValue ((Type)tileType, out xySize)) {
			xSize = (int)xySize.x;
			ySize = (int)xySize.y;
		} else {
			Debug.Log ("Attempted to get size for value " + tileType + " but it did not exist!");
			return;
		}

		tile.GetComponent<GridTile>().rotVal = tileRot;
		tile.GetComponent<GridTile>().type = 0;
		tile.GetComponent<GridTile>().streetType = tileType;
		tile.GetComponent<GridTile> ().xCenter = x;
		tile.GetComponent<GridTile> ().yCenter = y;
		tile.GetComponent<GridTile> ().xSize = xSize;
		tile.GetComponent<GridTile> ().ySize = ySize;

		tile.GetComponent<GridTile> ().tileObject = solid;
	}

	public void updateSurroundingTiles(int x, int y)
	{
		updateTile (x - 1, y);
		updateTile (x + 1, y);
		updateTile (x, y - 1);
		updateTile (x, y + 1);
	}

	public Vector2 closestForcedTileToPos(int x, int y)
	{
		if (!currentForcedTile)
			return Vector2.zero;

		int xVal = currentForcedTile.GetComponent<GridTile> ().xVal;
		int yVal = currentForcedTile.GetComponent<GridTile> ().yVal;

		Vector2 xySize;
		int xSize = 0, ySize = 0;

		if (typeSizes.TryGetValue (forcedTileIndex, out xySize)) {
			xSize = (int)xySize.x;
			ySize = (int)xySize.y;
		} else {
			Debug.Log ("Attempted to get size for value " + forcedTileIndex + " but it did not exist!");
			return Vector2.zero;
		}

		Vector2 xyVal = new Vector2 (x, y);

		float distance = float.MaxValue;
		Vector2 closestPair = Vector2.zero;

		switch (forcedTileIndex) {
		case Type.Neighborhood:
		case Type.City:
		case Type.Entertainment:
			for (int i = -(xSize + 1); i <= (xSize + 1); i++) {
				for (int j = -(ySize + 1); j <= (ySize + 1); j++) {
					if (Mathf.Abs (i) == Mathf.Abs (j))
						continue;

					if (Mathf.Abs (i) != (xSize + 1) && Mathf.Abs (j) != (ySize + 1))
						continue;

					Vector2 currPair = new Vector2 (xVal + i, yVal + j);
					float currDist = Vector2.Distance (xyVal, currPair);
					if (currDist < distance) {
						closestPair = currPair;
						distance = currDist;
					}
				}
			}
			break;
		}

		return closestPair;
	}

	//*********************//
	// Helper Tile Methods //
	//*********************//

	public int numberOfConnectedTiles(int x, int y)
	{
		int sum = 0;

		Vector2 xySize;
		int xSize = 0, ySize = 0;

		if (typeSizes.TryGetValue (Type.Street, out xySize)) {
			xSize = (int)xySize.x;
			ySize = (int)xySize.y;
		} else {
			Debug.Log ("Attempted to get size for value " + Type.Street + " but it did not exist!");
			return 0;
		}

		for (int i = -(xSize + 1); i <= (xSize + 1); i++) {
			for (int j = -(ySize + 1); j <= (ySize + 1); j++) {
				if (Mathf.Abs (i) == Mathf.Abs (j))
					continue;

				if (Mathf.Abs (i) != (xSize + 1) && Mathf.Abs (j) != (ySize + 1))
					continue;

				if ((x + i) > 0 && (x + i) < gridWidth - 1 && (y + j) > 0 && (y + j) < gridHeight - 1) {

					GameObject currTile = placedTiles [x + i, y + j];

					if (currTile.GetComponent<GridTile> ().blocked) {
						sum++;
					}
				}
			}
		}

		return sum;
	}

	public int numberOfConnectedTilesForTileType(int x, int y, Type indexType)
	{
		int sum = 0;

		Vector2 xySize;
		int xSize = 0, ySize = 0;

		if (typeSizes.TryGetValue (indexType, out xySize)) {
			xSize = (int)xySize.x;
			ySize = (int)xySize.y;
		} else {
			Debug.Log ("Attempted to get value " + indexType + " but value did not exist!");
			return 0;
		}

		for (int i = -(xSize + 1); i <= (xSize + 1); i++) {
			for (int j = -(ySize + 1); j <= (ySize + 1); j++) {
				if (Mathf.Abs (i) == Mathf.Abs (j))
					continue;

				if (Mathf.Abs (i) != (xSize + 1) && Mathf.Abs (j) != (ySize + 1))
					continue;

				if ((x + i) > 0 && (x + i) < gridWidth - 1 && (y + j) > 0 && (y + j) < gridHeight - 1) {

					GameObject currTile = placedTiles [x + i, y + j];

					if (currTile.GetComponent<GridTile> ().blocked && (Type)currTile.GetComponent<GridTile> ().type == Type.Street) {
						sum++;
					}
				}
			}
		}
		return sum;
	}

	// this will only be called if tile at x,y has 2 connecting tiles
	public int cornerTileCheck(int x, int y)
	{
		bool leftCheck = false, rightCheck = false, upCheck = false, downCheck = false;
		if (x > 0)
		{
			if (placedTiles [x - 1, y].GetComponent<GridTile> ().blocked)
				leftCheck = true;
		}
		if (x < gridWidth - 1)
		{
			if (placedTiles [x + 1, y].GetComponent<GridTile> ().blocked)
				rightCheck = true;
		}
		if (y > 0)
		{
			if (placedTiles [x, y - 1].GetComponent<GridTile> ().blocked)
				downCheck = true;
		}
		if (y < gridHeight - 1)
		{
			if (placedTiles [x, y + 1].GetComponent<GridTile> ().blocked)
				upCheck = true;
		}

		if (leftCheck)
		{
			if (upCheck)
				return 3;
			if (downCheck)
				return 2;
		}
		if (rightCheck)
		{
			if (upCheck)
				return 0;
			if (downCheck)
				return 1;
		}

		return -1;
	}

	// this will only be called if tile at x,y has 1 or 2 connecting tiles and isn't a corner
	public int straightTileCheck(int x, int y)
	{
		bool leftCheck = false, rightCheck = false;
		if (x > 0)
		{
			if (placedTiles [x - 1, y].GetComponent<GridTile> ().blocked)
				leftCheck = true;
		}
		if (x < gridWidth - 1)
		{
			if (placedTiles [x + 1, y].GetComponent<GridTile> ().blocked)
				rightCheck = true;
		}

		if (leftCheck || rightCheck)
		{
			return 0;
		}

		return 1;
	}

	// this will only be called if tile at x,y has 3 connecting tiles
	public int tTileCheck(int x, int y)
	{
		bool leftCheck = false, rightCheck = false, upCheck = false;
		if (x > 0)
		{
			if (placedTiles [x - 1, y].GetComponent<GridTile> ().blocked)
				leftCheck = true;
		}
		if (x < gridWidth - 1)
		{
			if (placedTiles [x + 1, y].GetComponent<GridTile> ().blocked)
				rightCheck = true;
		}
		if (y < gridHeight - 1)
		{
			if (placedTiles [x, y + 1].GetComponent<GridTile> ().blocked)
				upCheck = true;
		}

		if (leftCheck && rightCheck)
		{
			if (upCheck)
				return 2;
			else
				return 0;
		} 
		else {
			if (leftCheck)
				return 1;
			else
				return 3;
		}
	}

	// Calculates what the type of a street should be for moving ghost and updating streets
	public int tileTypeAtPos(int x, int y)
	{
		int numConnectedTiles = numberOfConnectedTiles (x, y);
		switch (numConnectedTiles)
		{

		case 0:
			return 0;
		case 1:
			return 0;
		case 2:
			int cornerCheck = cornerTileCheck (x, y);
			if (cornerCheck != -1)
				return 1;
			else
				return 0;
		case 3:
			return 2;
		case 4:
			return 3;
		default:
			return 0;
		}
	}

	// Calculates what the rotation of a street should be for moving ghost and updating streets
	public int tileRotAtPos(int x, int y)
	{
		int numConnectedTiles = numberOfConnectedTiles (x, y);
		switch (numConnectedTiles)
		{

		case 0:
			return 0;
		case 1:
			return straightTileCheck (x, y);
		case 2:
			int cornerCheck = cornerTileCheck (x, y);
			if (cornerCheck != -1)
				return cornerCheck;
			else
				return straightTileCheck (x, y);
		case 3:
			return tTileCheck (x, y);
		case 4:
			return 0;
		default:
			return 0;
		}
	}

	// Just checks if a tile is free at (x,y) factoring in the size of the set tileIndex
	public bool isTileFreeAtPos(int x, int y)
	{
		if (x < 0 || x > gridWidth - 1 || y < 0 || y > gridHeight - 1)
			return false;

		Vector2 xySize;
		int xSize = 1, ySize = 1;

		if (typeSizes.TryGetValue (tileIndex, out xySize)) {
			xSize = (int)xySize.x;
			ySize = (int)xySize.y;
		} else {
			Debug.Log ("Attempted to get size for value " + tileIndex + " but it did not exist!");
			return false;
		}

		for (int i = -xSize; i <= xSize; i++) {
			for (int j = -ySize; j <= ySize; j++) {
				if (x + i <= gridWidth - 1 && x + i >= 0 && y + j <= gridHeight - 1 && y + j >= 0) {
					GameObject tile = placedTiles [x + i, y + j];
					if (tile.GetComponent<GridTile> ().blocked)
						return false;
				} else
					return false;
			}
		}

		return true;
	}

	// This colors all tiles around (x,y) a specific color
	public void colorTiles(int x, int y, Color color)
	{
		if (x < 0 || x > gridWidth - 1 || y < 0 || y > gridHeight - 1)
			return;

		Vector2 xySize;
		int xSize = 0, ySize = 0;

		if (typeSizes.TryGetValue (tileIndex, out xySize)) {
			xSize = (int)xySize.x;
			ySize = (int)xySize.y;
		} else {
			Debug.Log ("Attempted to get size for value " + tileIndex + " but it did not exist!");
			return;
		}

		for (int i = -xSize; i <= xSize; i++) {
			for (int j = -ySize; j <= ySize; j++) {
				if (x + i <= gridWidth - 1 && x + i >= 0 && y + j <= gridHeight - 1 && y + j >= 0) {
					GameObject tile = placedTiles [x + i, y + j];
					if (!tile.GetComponent<GridTile> ().blocked) {
						coloredFloors.Add (tile);
						iTween.ColorUpdate (tile, color, 2.5f);
					}
				}
			}
		}
	}

	// Checks if a spawner with a connecting road exists
	public bool spawnerWithRoadExists()
	{
		List<GameObject> allSpawners = new List<GameObject> (simManager.GetComponent<SimManager> ().spawners);
		foreach (GameObject spawner in allSpawners) {
			int xVal, yVal, type;
			if (spawner.GetComponent<Spawner> ()) {
				xVal = spawner.GetComponent<Spawner> ().xVal;
				yVal = spawner.GetComponent<Spawner> ().yVal;
				type = spawner.GetComponent<Spawner> ().type;
			} else if (spawner.GetComponent<PedSpawner> ()) {
				xVal = spawner.GetComponent<PedSpawner> ().xVal;
				yVal = spawner.GetComponent<PedSpawner> ().yVal;
				type = spawner.GetComponent<PedSpawner> ().type;
			} else {
				Debug.Log ("Looping through allSpawners and element doesn't have Spawner or PedSpawner component.");
				return false;
			}
			if (numberOfConnectedTilesForTileType (xVal, yVal, (Type)type) > 0) {
				return true;
			}
		}

		return false;
	}
}
