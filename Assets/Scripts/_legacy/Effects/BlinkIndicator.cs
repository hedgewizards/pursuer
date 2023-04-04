using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkIndicator : MonoBehaviour
{
    private LineRenderer lr;
    public float maxLineDistance = 8;
    public int segments = 5;
    public Vector3 endpoint;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = segments;
    }

    private void Update()
    {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;
        if(Physics.Raycast(transform.position,Vector3.down,out hit, maxLineDistance,
            (int)LAYERMASK.SOLIDS_ONLY))
        {
            endpoint = hit.point;
        }
        else
        {
            endpoint = transform.position + new Vector3(0,-maxLineDistance,0);
        }
        for(int n = 1; n<segments; n++)
        {
            //Debug.Log(Vector3.Lerp(transform.position, endpoint, n / segments).magnitude);
            lr.SetPosition(n, Vector3.Lerp(transform.position, endpoint, (float)n / segments));
        }
    }
}
