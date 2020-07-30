using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    //mouse sensivity variable
    public float mouseSensitivity = 100f;

    //variable that helps when rotating player when turning horizontally (!must be the whole player as opposed to just the player body mesh!)
    public Transform player;

    float xRotation = 0f;

	// Use this for initialization
	void Start () 
    {
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () 
    {
        //define mouse x and y movements taking mouse sensityvity and time into account
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //get rotation and limit it
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //apply our rotation
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //rotate player body on y axis
        player.Rotate(Vector3.up * mouseX);
    }
}