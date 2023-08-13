using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAxis : MonoBehaviour
{

    public float smooth = 5.0f;

    void Update()
    {
        transform.Rotate(0, smooth * Time.deltaTime, 0);
    }
}
