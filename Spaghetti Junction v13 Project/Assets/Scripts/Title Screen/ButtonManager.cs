using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

    public GameObject[] startButtons = new GameObject[3];
    public GameObject backButton;
    public Button loadGameButton;
    public GameObject[] sandBoxDetails = new GameObject[3];
    public Slider slider;
    public Button makeGrid;
    public GameObject creditsPanel;
    public Text credits;
  
    public InputField input;

    public static bool backPressed;
    
    void Start()
    {
        credits.enabled = false;
    }

    // Update is called once per frame
    void Update () {
        if (SceneManager.GetActiveScene().name == "title")
        {
            if (input.text.Length >= 1)
            {
                makeGrid.enabled = true;
            }
            else
            {
                makeGrid.enabled = false;
            }
        }
    }

    public void creditsPressed()
    {
        backPressed = false;
        foreach (GameObject ob in startButtons)
            ob.SetActive(false);

        credits.enabled = true;
        creditsPanel.SetActive(true);
        StartCoroutine(credits.GetComponentInChildren<Credits>().startCredits());
        backButton.SetActive(true);
    }

    public void backButtonPressed()
    {
       backPressed = true;

       foreach (GameObject ob in startButtons)
            ob.SetActive(true);

       foreach (GameObject ob in sandBoxDetails)
            ob.SetActive(false);

        credits.enabled = false;
        creditsPanel.SetActive(false);

        backButton.SetActive(false);
     }

    public void quitButtonPressed()
    {
        Application.Quit();
    }

    public void newGamePressed()
    {
        backPressed = false;
        foreach (GameObject ob in sandBoxDetails)
            ob.SetActive(true);

        foreach (GameObject ob in startButtons)
            ob.SetActive(false);

        backButton.SetActive(true);
    }

    public void makeGridPressed()
    {
        Game.current = new Game();
        PlayerPrefs.SetInt("GridSize", (int)slider.value);
        PlayerPrefs.SetString("Name", input.text);
		SceneManager.LoadScene("NavMeshTesting");
    }
}
