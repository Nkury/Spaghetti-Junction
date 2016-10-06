using UnityEngine;
using System;
using System.Collections;

//[ExecuteInEditMode]
public class TimeOfDay : MonoBehaviour
{
	public bool timeChanges;
	public bool lightChanges;
	public float dayLength; // How long a day is in seconds
	public float currentHour; // Current hour of the day (currentTime % dayLength)

	private float totalHours = 24.0f; // How many hours are in a day
	//private int dawnOffset = 6;// Move the clock forward to adjust rotation of the sun
	private float currentTime; // Current time of day in seconds
	private float dayRatio; // Represents (from 0 to 1) the current time of day
	private Light Sun; // Directional light component added to the parent game object;

	void Start()
	{
		// Make sure currentHour isn't creater than the totalHours
		Mathf.Clamp(currentHour, 0.0f, totalHours);

		// Set initial dayRatio
		dayRatio = currentHour / totalHours;

		// Set the current time in seconds;
		currentTime = dayRatio * dayLength;

		// Set the Sun's rotation
		Sun = GetComponent<Light>();
		transform.eulerAngles = new Vector3((dayRatio * 360.0f) - 90.0f, 30.0f, 0.0f);
	}

	void Update()
	{
		if (timeChanges) 
		{
			currentTime += Time.deltaTime;
			if (currentTime >= dayLength)
			{
				currentTime = 0.0f;
			}
			currentHour = (int)(Mathf.Ceil((currentTime / dayLength) * 24.0f) % 24.0f);
			Mathf.Clamp(currentHour, 0.0f, 24.0f);
			dayRatio = currentTime / dayLength;  // Changes to seconds to be more precise
			Mathf.Clamp(dayRatio, 0.0f, 1.0f);

			if (lightChanges) 
			{
				transform.eulerAngles = new Vector3((dayRatio * 360.0f) - 90.0f, 30.0f, 0.0f);

				if (currentHour >= 7.0f && currentHour <= 18.0f) 
				{
					Sun.intensity = Mathf.Lerp(0.0f, 1.0f, Time.time);
					RenderSettings.fog = true;
					RenderSettings.ambientIntensity = Mathf.Lerp(0.5f, 1.0f, Time.time);
				}
				else
				{
					Sun.intensity = Mathf.Lerp(1.0f, 0.0f, Time.time);
					RenderSettings.fog = false;
					RenderSettings.ambientIntensity = Mathf.Lerp(1.0f, 0.5f, Time.time);
				}
			}
		}
	}
}