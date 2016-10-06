using UnityEngine;
using System.Collections;

public class ObjectFollowMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        while (Input.GetMouseButton(0))
        {
            Vector3 temp = Input.mousePosition;
            temp.z = this.gameObject.transform.position.z - Camera.main.transform.position.z;
            transform.position = Camera.main.ScreenToWorldPoint(temp);
        }
    }
}
