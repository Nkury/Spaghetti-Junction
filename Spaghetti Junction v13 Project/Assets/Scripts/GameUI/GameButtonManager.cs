using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameButtonManager : MonoBehaviour {

    public Button[] roadButtons;
	public Button[] timeButtons;
	public Sprite[] playPauseButtons;
	public GameObject[] UISwapItems;

    public GameObject planeManager, simManager, stateManager;
    public Text description;
    public Text totalCars;
    public Text commutedCars;
    public Text pedestrianCount;
	public bool forcedButton;
	public int selectedButton = 0, prevButton = 0;

	// Use this for initialization
	void Start () {
		stateManager = GameObject.Find ("StateManager");
		planeManager = GameObject.Find ("PlaneManager");
		simManager = GameObject.Find ("SimManager");

        Button b = roadButtons[0].GetComponent<Button>();
        ColorBlock cb = b.colors;
        cb.normalColor = Color.white;
        b.colors = cb;

		clearDescription ();

        for (int i = 1; i < roadButtons.Length; i++)
        {
            Button b2 = roadButtons[i].GetComponent<Button>();
            ColorBlock cbb = b2.colors;
            cbb.normalColor = Color.gray;
            b2.colors = cbb;
        }
	}
	
	// Update is called once per frame
	void Update () {
        totalCars.text = GameStats.totalCars.ToString();
        commutedCars.text = GameStats.totalCommutedCars.ToString();
        pedestrianCount.text = GameStats.pedestriansCommuted.ToString();
	}

	public void buttonClicked(int i)
	{
		if (!forcedButton) {
			//Debug.Log ("Just clicked button " + i);

			if (i != selectedButton)
				prevButton = selectedButton;
			selectedButton = i;

			switch (i) {
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				roadClicked (i);
				break;
			case 6:
				eraserModeClicked ();
				break;
			}

			// for changing the normal color of a button
			Button b = roadButtons [i].GetComponent<Button> ();
			ColorBlock cb = b.colors;
			cb.normalColor = Color.white;
			b.colors = cb;

			for (int j = 0; j < roadButtons.Length; j++) {
				if (j != i) {
					Button b2 = roadButtons [j].GetComponent<Button> ();
					ColorBlock cbb = b2.colors;
					cbb.normalColor = Color.gray;
					b2.colors = cbb;
				}
			}
		} else {
			// for changing the normal color of a button
			Button b = roadButtons [selectedButton].GetComponent<Button> ();
			ColorBlock cb = b.colors;
			cb.normalColor = Color.white;
			b.colors = cb;
			b.interactable = true;

			for (int j = 0; j < roadButtons.Length; j++) {
				if (j != selectedButton) {
					Button b2 = roadButtons [j].GetComponent<Button> ();
					ColorBlock cbb = b2.colors;
					cbb.normalColor = Color.gray;
					b2.colors = cbb;
					b2.interactable = false;
				}
			}
		}
	}

	public void selectButton(int i, bool forced)
	{
		if (i != selectedButton)
			prevButton = selectedButton;
		selectedButton = i;

		forcedButton = forced;

		// for changing the normal color of a button
		if (i >= 0 && i < roadButtons.Length) {
			Button b = roadButtons [i].GetComponent<Button> ();
			ColorBlock cb = b.colors;
			cb.normalColor = Color.white;
			b.colors = cb;
			b.interactable = true;
		}

		for (int j = 0; j < roadButtons.Length; j++)
		{
			if (j != i)
			{
				Button b2 = roadButtons[j].GetComponent<Button>();
				b2.interactable = !forced;
				ColorBlock cbb = b2.colors;
				cbb.normalColor = Color.gray;
				b2.colors = cbb;
			}
		}
	}

	public void forceButton(int i)
	{
		//Debug.Log ("Force Button for button " + i);
		selectButton (i, true);
	}

	public void releaseButton()
	{
		//Debug.Log ("Releasing all buttons");
		forcedButton = false;

		selectButton (0, false);
	}

    public void roadClicked(int i)
    {
		// setting the tile type
		planeManager.GetComponent<PlaneManager>().setGhost(i);
    }

	public void eraserModeClicked()
	{
		planeManager.GetComponent<PlaneManager> ().enableEraserMode ();
	}

	public void setSimButtonEnabled(bool enabled)
	{
		timeButtons [0].interactable = enabled;
		UISwapItems [0].SetActive (!enabled);
		UISwapItems [1].SetActive (enabled);
	}

	public void simButtonClicked(int i)
	{
		switch (i) {
		case 0:
			Camera.main.GetComponent<MusicManager> ().setMusicSpeed (1);

			if (stateManager.GetComponent<StateManager>().getState() != StateManager.GameStates.Simulating) {
				Camera.main.GetComponent<MusicManager>().ToggleMusic();

				timeButtons [0].GetComponent<Image> ().sprite = playPauseButtons [1];
				UISwapItems [2].SetActive (true);
				UISwapItems [3].SetActive (true);
				forceButton (-1);

				for(int j = 0; j < timeButtons.Length; j++) {
					timeButtons[j].interactable = true;
				}

				stateManager.GetComponent<StateManager> ().setState (StateManager.GameStates.Simulating);
			} else {
				if (simManager) {
					if (simManager.GetComponent<SimManager> ().getState () != SimManager.SimState.Frozen) {
						simManager.GetComponent<SimManager> ().setState (SimManager.SimState.Frozen);
						timeButtons [0].GetComponent<Image> ().sprite = playPauseButtons [0];
					} else {
						simManager.GetComponent<SimManager> ().setState (SimManager.SimState.Normal);
						timeButtons [0].GetComponent<Image> ().sprite = playPauseButtons [1];
					}
				}
			}
			break;
		case 1:
			if (simManager) {
				if (simManager.GetComponent<SimManager> ().getState () == SimManager.SimState.FF2X) {
					Debug.Log ("Setting simManager state to normal");
					simManager.GetComponent<SimManager> ().setState (SimManager.SimState.Normal);
					Camera.main.GetComponent<MusicManager> ().setMusicSpeed (1f);
				} else {
					Debug.Log ("Setting simManager state to 2x");
					simManager.GetComponent<SimManager> ().setState (SimManager.SimState.FF2X);
					Camera.main.GetComponent<MusicManager> ().setMusicSpeed (1.5f);
				}
				timeButtons [0].GetComponent<Image> ().sprite = playPauseButtons [1];
			}
			break;
		case 2:
			if (simManager) {
				if (simManager.GetComponent<SimManager> ().getState () == SimManager.SimState.FF3X) {
					Debug.Log ("Setting simManager state to normal");
					simManager.GetComponent<SimManager> ().setState (SimManager.SimState.Normal);
					Camera.main.GetComponent<MusicManager> ().setMusicSpeed (1f);
				} else {
					Debug.Log ("Setting simManager state to 3x");
					simManager.GetComponent<SimManager> ().setState (SimManager.SimState.FF3X);
					Camera.main.GetComponent<MusicManager> ().setMusicSpeed (2f);
				}
				timeButtons [0].GetComponent<Image> ().sprite = playPauseButtons [1];
			}
			break;
		default:
			break;
		}
	}

    public void setDescription(int i)
    {
        switch (i)
        {
            case 0:
                description.GetComponent<Text>().text = "Streets- single tiles that let cars drive throughout the city.";
                break;
            case 1:
				description.GetComponent<Text>().text = "Neighborhood- a 3x3 tile where cars spawn in the morning and head back in the evening.";
                break;
            case 2:
				description.GetComponent<Text>().text = "City- a 3x3 tile where cars head to in the morning and leave in the evening.";
                break;
			case 3:
				description.GetComponent<Text>().text = "Eraser- this lets you destroy tiles that you have already placed.";
				break;
			case 4:
				description.GetComponent<Text> ().text = "Entertainment- a 3x3 tile where cars head to near the evening.";
				break;
			default:
				description.GetComponent<Text>().text = "TAB- Switch camera perspective\nWASD- Move camera\nQ/E- Strafe camera";
				break;
        }
    }

	public void adjustUIForSimStop() {
		UISwapItems [2].SetActive (false);
		UISwapItems [3].SetActive (false);
		timeButtons [0].GetComponent<Image> ().sprite = playPauseButtons [0];
		timeButtons [1].interactable = false;
		timeButtons [2].interactable = false;

		selectButton (0, false);
		roadClicked (0);
	}

    public void clearDescription()
    {
        description.GetComponent<Text>().text = "TAB- Switch camera perspective\nWASD- Move camera\nQ/E- Strafe camera";
    }

    public void mouseEnterUI()
    {
		planeManager.GetComponent<PlaneManager>().mouseOnUI();
    }

    public void mouseExitUI()
    {
		planeManager.GetComponent<PlaneManager>().mouseOffUI();
    }
}
