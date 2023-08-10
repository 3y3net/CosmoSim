using UnityEngine;

public class OrbitByDistance : MonoBehaviour {

    public float speedFactor = 1f;
    private float actualSpeed;
    float distFromCenter;
    private Quaternion rotPos;

    // Use this for initialization
    void Start () {
        rotPos = transform.rotation;
        distFromCenter = Vector3.Distance(transform.parent.position, transform.position);
        actualSpeed = (100f / distFromCenter) * speedFactor;
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(transform.parent.position, transform.parent.TransformDirection(Vector3.up), actualSpeed * Time.deltaTime);
        transform.rotation = rotPos;
	}

    private void LateUpdate()
    {
        rotPos = transform.rotation;
    }
}
