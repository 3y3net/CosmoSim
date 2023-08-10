using UnityEngine;

public class PlanetViewCamera : MonoBehaviour {
    public GameObject target, clouds;
    public float scrollSpeed = 3f, minScroll = 5f, maxScroll = 15f, rotationSpeed = 1f;
    private bool cloudsOff = false;
    private float yaw = 0f; // Amount the camera has rotated

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.P)) // toggle camera arm as child of target so it rotates with the target
        {
            if (transform.parent.parent == target.transform) transform.parent.parent = null;
            else transform.parent.parent = target.transform;
        }

        // Toggle clouds when C is pressed
        if (clouds != null)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (cloudsOff) cloudsOff = false;
                else cloudsOff = true;
            }
            if (cloudsOff && clouds.GetComponent<Renderer>().material.GetFloat("_Cutoff") < 1.0f)
            {
                clouds.GetComponent<Renderer>().material.SetFloat("_Cutoff", clouds.GetComponent<Renderer>().material.GetFloat("_Cutoff") + Time.deltaTime * 0.5f);
            }
            else if (!cloudsOff && clouds.GetComponent<Renderer>().material.GetFloat("_Cutoff") > 0.4f)
            {
                clouds.GetComponent<Renderer>().material.SetFloat("_Cutoff", clouds.GetComponent<Renderer>().material.GetFloat("_Cutoff") - Time.deltaTime * 0.5f);
            }
        }

        // Scroll camera in and out from cameras arm
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && Vector3.Distance(transform.position, transform.parent.position) >= minScroll)
        {
            transform.Translate((Vector3.forward * Time.deltaTime) * scrollSpeed, Space.Self);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && Vector3.Distance(transform.position, transform.parent.position) <= maxScroll)
        {
            transform.Translate((Vector3.back * Time.deltaTime) * scrollSpeed, Space.Self);
        }
    }

    private void FixedUpdate()
    {
        transform.LookAt(transform.parent);
    }

    private void LateUpdate()
    {
        // Middle mouse click and drag to rotate camera
        if (Input.GetKey(KeyCode.Mouse2))
        {
            print("mouse2 got");
            yaw += rotationSpeed * Input.GetAxis("Mouse X");
            transform.parent.eulerAngles = new Vector3(0.0f, yaw, 0.0f);
        }
    }
}
