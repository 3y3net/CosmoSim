using UnityEngine;

public class GalaxyCamera : MonoBehaviour
{
    private GameObject starCam;
    private float galaxyRadius;

    void Start()
    {
        starCam = GameObject.FindObjectOfType<StarCamera>().gameObject;
        galaxyRadius = GameObject.FindObjectOfType<StarGeneration>().galaxy.radiusH;
    }

    void LateUpdate()
    {
        transform.position = new Vector3((10 / galaxyRadius) * starCam.transform.position.x, (10 / galaxyRadius) * starCam.transform.position.y, (10 / galaxyRadius) * starCam.transform.position.z);
        transform.rotation = starCam.transform.rotation;
        //print(starCam.transform.position + " " + transform.position);
    }
}
