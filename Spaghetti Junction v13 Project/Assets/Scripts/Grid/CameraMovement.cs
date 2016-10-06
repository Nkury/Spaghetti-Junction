using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	public bool topDown;
	public GameObject stateManager;

	private float rotateSpeed = 1.0f;

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update()
	{
		if (stateManager.GetComponent<StateManager>().getState() != StateManager.GameStates.Paused)
		{
			// move camera left, right, up, down
			if (Input.GetAxis("CameraForward") != 0)
			{

				Vector3 forward;
				if (topDown)
				{
					forward = transform.up * Input.GetAxis("CameraForward");
				}
				else {
					forward = transform.forward * Input.GetAxis("CameraForward");
					forward.y = 0;
				}
				this.gameObject.transform.position += forward;
			}

			// move camera backward
			if ((Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKey(KeyCode.Space)) && this.gameObject.transform.position.y < 200)
			{
				this.gameObject.transform.position += new Vector3(0, 1, 0);
			}

			if (Input.GetKey(KeyCode.Q))
			{
				this.transform.RotateAround(this.transform.position, Vector3.down, rotateSpeed);
			}
			if (Input.GetKey(KeyCode.E))
			{
				this.transform.RotateAround(this.transform.position, Vector3.up, rotateSpeed);
			}
			if (Input.GetKey(KeyCode.A))
			{
				this.transform.position -= this.transform.right * rotateSpeed;
			}
			if (Input.GetKey(KeyCode.D))
			{
				this.transform.position += this.transform.right * rotateSpeed;
			}

			// move camera forward
			if ((Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKey(KeyCode.LeftShift)) && this.gameObject.transform.position.y > 15)
			{
				this.gameObject.transform.position -= new Vector3(0, 1, 0);
			}

			if (Input.GetKeyDown(KeyCode.Tab))
			{
				topDown = !topDown;
				if (topDown)
				{
					iTween.RotateTo(gameObject, iTween.Hash("x", 90, "EaseType", "easeInQuad", "time", 0.25f));
				}
				else {
					iTween.RotateTo(gameObject, iTween.Hash("x", 35, "EaseType", "easeInQuad", "time", 0.25f));
				}
			}
		}
	}
}
