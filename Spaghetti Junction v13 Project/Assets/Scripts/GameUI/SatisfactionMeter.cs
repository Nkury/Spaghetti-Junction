using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SatisfactionMeter : MonoBehaviour {
    public Sprite[] satisfactionFaces;
    public Image pedestrianFace;
    public GameObject gameOver;

    private float maxNumberOfTrappedCars = 20;
    private bool isBlink1 = true;
    private bool isBlink2 = true;
    private bool isBlink3 = true;
    private bool isBlink4 = true;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        this.GetComponentInChildren<Text>().text = Spawner.totalTrappedCars.ToString();
        float sliderValue = Spawner.totalTrappedCars / maxNumberOfTrappedCars;
        if (sliderValue > 1)
            sliderValue = 1;
        this.GetComponentInChildren<Slider>().value = sliderValue;

  
        if (this.GetComponentInChildren<Slider>().value >= 0 && this.GetComponentInChildren<Slider>().value < .25f)
        {
            isBlink1 = true;
            isBlink2 = true;
            isBlink3 = true;
            isBlink4 = true;
            pedestrianFace.sprite = satisfactionFaces[0];
        } else if (this.GetComponentInChildren<Slider>().value >= .25f && this.GetComponentInChildren<Slider>().value < .5f && isBlink1)
        {
            isBlink1 = false;
            pedestrianFace.sprite = satisfactionFaces[1];
            StartCoroutine(blink());
        } else if(this.GetComponentInChildren<Slider>().value >= .5f && this.GetComponentInChildren<Slider>().value < .75f && isBlink2)
        {
            isBlink2 = false;
            pedestrianFace.sprite = satisfactionFaces[2];
            StartCoroutine(blink());
        } else if(this.GetComponentInChildren<Slider>().value >= .75f && this.GetComponentInChildren<Slider>().value < 1 && isBlink3)
        {
            isBlink3 = false;
            pedestrianFace.sprite = satisfactionFaces[3];
            StartCoroutine(blink());
        }
        else if(this.GetComponent<Slider>().value == 1 && isBlink4)
        {
            isBlink4 = false;
            Camera.main.GetComponent<MusicManager>().GameOver();
            pedestrianFace.sprite = satisfactionFaces[4];
            StartCoroutine(blink());
        }        

        if(Spawner.totalTrappedCars >= 20)
        {         
            gameOver.SetActive(true);
        }
        else
        {
            gameOver.SetActive(false);
        }
	}

    IEnumerator blink()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(.25f);
            pedestrianFace.gameObject.SetActive(!pedestrianFace.IsActive());
        }
    }


}
