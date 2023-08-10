using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitalLine : MonoBehaviour {
    public Transform targetBody;
    [Range(0, 1000)]
    public int segments = 100;
    [Range(0, 3000)]
    private float radius;
    LineRenderer line;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        radius = (Vector3.Distance(transform.position, targetBody.position) );
        line.positionCount = segments + 1;
        line.useWorldSpace = true;
        CreatePoints();
    }

    void CreatePoints()
    {
        float x, z;
        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments);
        }
    }
}
