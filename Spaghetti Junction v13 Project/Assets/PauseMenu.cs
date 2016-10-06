using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour {

    public GameObject stateManager;
    public GameObject tutorial;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void setTutorial()
    {
        tutorial.SetActive(true);
        tutorial.GetComponentInChildren<Tutorial>().restartTutorial();
    }

    public void toTitle()
    {
        SceneManager.LoadScene("title");
    }

    public void quit()
    {
        Application.Quit();
    }

    public void exitScreen()
    {
        this.gameObject.SetActive(false);
        stateManager.GetComponent<StateManager>().setState();
    }
}
