using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarFlare : MonoBehaviour {

    private Transform target;
    private Star star;

    private void Start()
    {
        target = GameObject.FindObjectOfType<SolarSystemCamera>().transform;
        star = GameObject.FindObjectOfType<SolarSystem>().star;
    }
    // Update is called once per frame
    void Update () {
        transform.LookAt(target.position);
        float dist = Vector3.Distance(target.position, transform.position);
        float scale = (star.luminosity / (4f * 3.14f * (dist * dist))) * 1000000f;
        if (scale > 0.005f) // As stars get brighter they should gradually increase in size until they are very close but not get too big
        {
            if (scale > 0.01)
            {
                scale = 0.01f + ((scale - 0.01f) / 4f);
                if (scale > 0.02)
                {
                    scale = 0.02f + ((scale - 0.02f) / 8f);
                    if (scale > 0.03f)
                    {
                        scale = 0.03f + ((scale - 0.03f) / 16f);
                        if (scale > 0.04f)
                        {
                            scale = 0.04f + ((scale - 0.04f) / 16f);
                            if (scale > 0.1f) scale = 0.1f;
                        }
                    }
                }
            }
        }
        if (scale < 0.002f) scale = 0.002f;
        transform.localScale = new Vector3(scale * dist, scale * dist, scale * dist);
	}
}
