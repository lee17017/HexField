using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public class CameraBehavior : MonoBehaviour {

    public float tiltRange = 45f;

    new Camera camera;
    Vector3 oldMousePosition;
	// Use this for initialization

      
	void Awake () {
        camera = gameObject.GetComponent<Camera>();
        oldMousePosition = Input.mousePosition;
    }
	
	// Update is called once per frame
	void Update () {
        if ((Input.GetAxis("Mouse ScrollWheel") > 0 && camera.fieldOfView > 10) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            camera.fieldOfView -= (Input.GetAxis("Mouse ScrollWheel")*10);
        }

        if (Input.GetMouseButton(1))//Right Click
        {
            if (Input.GetMouseButtonDown(1))
            {
                oldMousePosition = Input.mousePosition;
            }

            Vector3 diff = Input.mousePosition - oldMousePosition;
            oldMousePosition = Input.mousePosition;
            transform.Translate(-diff);
        }
        else if (Input.GetMouseButton(2))//Middle Click
        {
            if (Input.GetMouseButtonDown(2))
            {
                oldMousePosition = Input.mousePosition;
            }

            Vector3 diff = Input.mousePosition - oldMousePosition;
            oldMousePosition = Input.mousePosition;
            float y = -diff.y;
            float x = 0;//diff.x;
            if (transform.rotation.eulerAngles.x < 90 - tiltRange)
            {
                if ((y < 0 && transform.rotation.eulerAngles.y != 0) || (y > 0 && transform.rotation.eulerAngles.y == 0))
                    transform.Rotate(new Vector3(y, 0, x));
            }
            else {
                    transform.Rotate(new Vector3(y, 0, x));
            }
        
        }

    }
}