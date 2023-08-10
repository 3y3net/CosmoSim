using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitCamera : MonoBehaviour
{
    /*
    public float movementTime = 1;
    public float rotationSpeed = 0.1f;

    Vector3 refPos;
    Vector3 refRot;
    */

    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    public Vector3 originalPosition;
    public Quaternion originalRotation;
    public Vector3 targetPosition;
    public Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //Interpolate Position
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref refPos, movementTime);
        //Interpolate Rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, rotationSpeed * Time.deltaTime);
        */
    }

    public void SetTarget(Transform target, bool makeItNewOrigin=false)
	{
        if (makeItNewOrigin)
        {
            originalPosition = target.position;
            originalRotation = target.rotation;
        }
        else
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }
        targetPosition = target.position;
        targetRotation = target.rotation;
    }
    
    public void GoBack()
	{
        targetPosition = originalPosition;
        targetRotation = originalRotation;
    }

    void LateUpdate()
    {
        this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPosition, ref velocity, smoothTime);
        this.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothTime * 10f);
    }
}
