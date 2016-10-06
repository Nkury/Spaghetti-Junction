using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderWithValue : MonoBehaviour
{

	public Slider slider;
	public Text text;
	public string unit;
	public byte decimals = 2;
	public bool isTitle;

	void OnEnable()
	{
		slider.onValueChanged.AddListener(ChangeValue);
		ChangeValue(slider.value);
	}

	void OnDisable()
	{
		slider.onValueChanged.RemoveAllListeners();
	}

	void ChangeValue(float value)
	{
		if (isTitle)
			text.text = value.ToString("n" + decimals) + " x " + value.ToString("n" + decimals);
		else
			text.text = value.ToString("n" + decimals) + " " + unit;
	}


}