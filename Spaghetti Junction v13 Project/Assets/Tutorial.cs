using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {
    public GameObject tutorial;
    public GameObject stateManager;
    public GameObject pauseMenu;

    private string[] dialogue = {"Hi there! My name is Katherine and I'm the architect around these parts! It's very nice to meet you!",
                "Welcome to Spaghetti Junction, a playground for architect enthusiasts who want to have a little fun! Before you begin, let me give you some tips!",
                "The world is divided in square tiles where you can place streets, neighborhoods, cities, airports, you name it!",
                "Simply clicking on a tile will place your selected object for you. But don't forget, there is an bomb tool that will erase any mistakes you make.",
                "To move around, use WASD. To rotate, use Q and E. To ascend or descend, use the mouse wheel. To change perspective, press TAB",
                "To start the simulation, place down a \"car spawner\". These are neighborhoods (house icon) or cities.",
                "There are also \"pedestrian spawners\" such as entertainment districts (movie icon), train stations, or airports! Place those to populate your city!",
                "The main goal is to build a thriving city that survives as long as possible! That's right, I used the word \"survive\"!",
                "On the right is a \"satisfaction meter\". As long as you get cars out of your spawners, you will have a thriving city.",
                "However, if there's too much backup, your city's population will get unhappy. When more than twenty cars are trapped, you will lose.",
                "Trust me, though. As long as you build a great city, you won't have anything to worry about!",
                "If you want to talk with me again, just click the ESCAPE key. Okay, that's it! Have fun!"};

    private int index = 0;
    private int dialogueIndex = 0;
	// Use this for initialization
	void Start () {
        restartTutorial();
    }

    // Update is called once per frame
    void Update () {
        if (dialogueIndex >= dialogue.Length)
        {
            tutorial.SetActive(false);
            stateManager.GetComponent<StateManager>().setState();
        }
        else {
            if (index <= dialogue[dialogueIndex].Length)
                this.GetComponent<Text>().text = dialogue[dialogueIndex].Substring(0, index);
        }

        index++;
	}

    public void nextText()
    {
        index = 0;
        dialogueIndex++;
    }

    public void skip()
    {
        dialogueIndex = dialogue.Length;
    }

    public void restartTutorial()
    {
        tutorial.SetActive(true);
        stateManager.GetComponent<StateManager>().setState(StateManager.GameStates.Paused);
        pauseMenu.SetActive(false);
        dialogueIndex = 0;
        index = 0;
    }

}
