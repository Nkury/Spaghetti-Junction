using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StateManager : MonoBehaviour {

	// Game states
	public enum GameStates{Simulating, Paused, Editing};
	private GameStates gameState = GameStates.Editing, prevState = GameStates.Editing;

	public GameObject planeManager, simManager, gameButtonManager, dayCycleManager;
	public GameObject miniMenu;

	// Use this for initialization
	void Start () {
		planeManager = GameObject.Find ("PlaneManager");
		simManager = GameObject.Find ("SimManager");
		gameButtonManager = GameObject.Find ("ButtonManager");
		dayCycleManager = GameObject.Find ("DayCycle");
	}

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameState == GameStates.Paused)
            {
                miniMenu.SetActive(false);
                setState();
            }
            else {
                miniMenu.SetActive(true);
                setState(GameStates.Paused);
            }
            Debug.Log("GameState now at " + gameState);
        }
    }

	public GameStates getState() {
		return gameState;
	}

	public void setState(GameStates newState) {
		Debug.Log ("StateManager setState called for " + newState + " and gameState is " + gameState);

		if (newState == gameState)
			return;

		switch (newState) {
		case GameStates.Simulating:
			if (planeManager) {
				planeManager.GetComponent<PlaneManager> ().simModeEnabled (true);
			}
			if (simManager) {
				simManager.GetComponent<SimManager> ().simModeEnabled (true);
			}
			if (dayCycleManager) {
				dayCycleManager.GetComponent<DayNightController> ().daySpeedMultiplier = 0.1f;
				dayCycleManager.GetComponent<DayNightController> ().setTime (8.0f);
			}

			break;
		case GameStates.Paused:

			if (prevState == GameStates.Simulating) {
				// pause?

			} else if (prevState == GameStates.Editing) {
				// pause?
			}
			break;
		case GameStates.Editing:
			if (gameState == GameStates.Simulating) {
				if (planeManager) {
					planeManager.GetComponent<PlaneManager> ().simModeEnabled (false);
				}

				if (simManager) {
					simManager.GetComponent<SimManager> ().simModeEnabled (false);
					simManager.GetComponent<SimManager> ().resetSpawners ();
				}

				if (gameButtonManager) {
					gameButtonManager.GetComponent<GameButtonManager> ().adjustUIForSimStop ();
				}

				if (dayCycleManager) {
					dayCycleManager.GetComponent<DayNightController> ().setTime (10.0f);
					Debug.Log ("Just set time back to " + dayCycleManager.GetComponent<DayNightController> ().currentTime);
					dayCycleManager.GetComponent<DayNightController> ().resetLights ();
				}

				Camera.main.GetComponent<MusicManager> ().ToggleMusic ();
			} 
			break;
		}

		prevState = gameState;
		gameState = newState;
	}

	public void setState() {
		setState (prevState);
	}

	public void resetState() {
		if (gameState == GameStates.Paused) {
			setState ();
		}
		setState (GameStates.Editing);
	}
}
