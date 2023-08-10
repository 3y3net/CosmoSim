using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupManager : MonoBehaviour {
    private GameObject activeCamera;

	void Start () {
        activeCamera = GameObject.Find("Galaxy Camera");
        activeCamera.GetComponent<Camera>().enabled = true;
	}
	
	void Update () {
		
	}

    public void TabButtonPressed(GameObject cam)
    {
        activeCamera.GetComponent<Camera>().enabled = false;
        activeCamera = cam;
        cam.GetComponent<Camera>().enabled = true;
    }
}
