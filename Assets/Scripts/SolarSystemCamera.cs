using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolarSystemCamera : MonoBehaviour {

    private TravelManager travelManager;

    public float rotationSpeed = 3.0f;
    public float moveSpeed = 15.0f;

    private void Start()
    {
        travelManager = GameObject.FindObjectOfType<TravelManager>();
    }

    void Update () {
		if(travelManager.travelMode == TravelManager.TravelMode.Solar)
        {
            GameObject.Find("SpeedMax").GetComponent<Text>().text = (moveSpeed * 0.1f).ToString("F2") + " AU/sec";

            // Do something with key input
            // move camera
            if (Input.GetAxis("Vertical") != 0f)
            {
                if (Input.GetAxis("Horizontal") != 0f)
                    transform.Translate(((Vector3.forward * Input.GetAxis("Vertical")) + (Vector3.right * Input.GetAxis("Horizontal"))).normalized * Time.deltaTime * moveSpeed);
                else
                    transform.Translate((Vector3.forward * Input.GetAxis("Vertical")) * Time.deltaTime * moveSpeed);
            }
            else if (Input.GetAxis("Horizontal") != 0f)
                transform.Translate((Vector3.right * Input.GetAxis("Horizontal")) * Time.deltaTime * moveSpeed);

            if (Input.GetAxis("Jump") != 0f)
                transform.Translate(Vector3.up * Time.deltaTime * (moveSpeed * Input.GetAxis("Jump")));

            // rotate camera while middle mouse click
            if (Input.GetKey(KeyCode.Mouse2))
                transform.Rotate(-rotationSpeed * Input.GetAxis("Mouse Y"), rotationSpeed * Input.GetAxis("Mouse X"), 0.0f);
            else if ((Input.GetAxis("Pitch") != 0f || Input.GetAxis("Yaw") != 0f) && !Input.GetKey(KeyCode.Joystick1Button9))
            {
                transform.Rotate((rotationSpeed * 10f) * Input.GetAxis("Pitch"), (rotationSpeed * 10f) * Input.GetAxis("Yaw"), 0.0f);
            }

            if (Input.GetAxis("Roll") != 0)
                transform.Rotate(0f, 0f, -Input.GetAxis("Roll") * 45 * Time.deltaTime);

            // output distance in Astronomical Units the camera is from the target
            if (GameObject.Find("StarName").GetComponent<Text>().text != "")
                GameObject.Find("StarDistance").GetComponent<Text>().text = (Vector3.Distance(transform.position, new Vector3(0,0,0)) * 0.1f).ToString("F2") + " AU";
        }
	}

    void LateUpdate()
    {
        if (travelManager.travelMode == TravelManager.TravelMode.Solar)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                moveSpeed += Input.GetAxis("Mouse ScrollWheel") * 25f;
                if (moveSpeed > 1000f) moveSpeed = 1000f;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                moveSpeed += Input.GetAxis("Mouse ScrollWheel") * 25f;
                if (moveSpeed < 0.1f) moveSpeed = 0.1f;
            }
            else if(Input.GetKey(KeyCode.Joystick1Button9))
            {
                if ((Input.GetAxis("Pitch") * -1f) > 0f)
                {
                    moveSpeed += (Input.GetAxis("Pitch") * -1f) * 25f;
                    if (moveSpeed > 1000f) moveSpeed = 1000f;
                }
                else if ((Input.GetAxis("Pitch") * -1f) < 0f)
                {
                    moveSpeed += (Input.GetAxis("Pitch") * -1f) * 25f;
                    if (moveSpeed < 0.1f) moveSpeed = 0.1f;
                }
            }

            // If we are far from the star then enter stellar mode
            if (Vector3.Distance(transform.position, new Vector3(0f, 0f, 0f)) > 10120f)
            {
                travelManager.travelMode = TravelManager.TravelMode.Stellar;
                GameObject.FindObjectOfType<SolarSystem>().ClearSolarSystem();
                GameObject.FindObjectOfType<TargetTracker>().cam = GameObject.FindObjectOfType<StarCamera>().transform.GetComponent<Camera>();
                GameObject.FindObjectOfType<TargetTracker>().target = new Vector3(0,0,0);
                print("Leaving Solar Mode");
            }
        }
    }
}
