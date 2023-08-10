using UnityEngine;

public class FreeCamera : MonoBehaviour {

    public float rotationSpeed = 3.0f; // Speed the camera will rotate around the rig
    public float moveSpeed = 15.0f;
    private float pitch, yaw; // Amount the camera has rotated
    private bool forward = false, right = false, left = false, reverse = false, shift = false, ctrl = false;

    private void Update()
    {
        // Get key input
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.W))
            forward = true;
        else if (Input.GetKeyUp(KeyCode.W))
            forward = false;
        if (Input.GetKeyDown(KeyCode.S))
            reverse = true;
        else if (Input.GetKeyUp(KeyCode.S))
            reverse = false;
        if (Input.GetKeyDown(KeyCode.D))
            right = true;
        else if (Input.GetKeyUp(KeyCode.D))
            right = false;
        if (Input.GetKeyDown(KeyCode.A))
            left = true;
        else if (Input.GetKeyUp(KeyCode.A))
            left = false;
        if (Input.GetKeyDown(KeyCode.LeftShift))
            shift = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            shift = false;
        if (Input.GetKeyDown(KeyCode.LeftControl))
            ctrl = true;
        else if (Input.GetKeyUp(KeyCode.LeftControl))
            ctrl = false;

        // Do something with key input
        // move camera
        if (forward)
        {
            if (right)
                transform.Translate((Vector3.forward + Vector3.right).normalized * Time.deltaTime * moveSpeed);
            else if (left)
                transform.Translate((Vector3.forward + Vector3.left).normalized * Time.deltaTime * moveSpeed);
            else
                transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        }
        else if (reverse)
        {
            if (right)
                transform.Translate((Vector3.forward + Vector3.left).normalized * Time.deltaTime * moveSpeed * -1);
            else if (left)
                transform.Translate((Vector3.forward + Vector3.right).normalized * Time.deltaTime * moveSpeed * -1);
            else
                transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * -1);
        }
        else if (right)
        {
            transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
        }
        else if (left)
        {
            transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
        }
        if (shift)
        {
            transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
        }
        else if (ctrl)
        {
            transform.Translate(Vector3.up * Time.deltaTime * moveSpeed * -1);
        }

        // Middle mouse click and drag to rotate camera
        if (Input.GetKey(KeyCode.Mouse2))
        {
            pitch += rotationSpeed * Input.GetAxis("Mouse Y");
            yaw += rotationSpeed * Input.GetAxis("Mouse X");
            transform.eulerAngles = new Vector3(pitch * -1, yaw, 0.0f);
        }

    }

    // LateUpdate is called once near the end of every frame
    void LateUpdate()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            moveSpeed += Input.GetAxis("Mouse ScrollWheel");
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && moveSpeed > 0f)
        {
            moveSpeed += Input.GetAxis("Mouse ScrollWheel");
        }
    }
}