using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleTester : MonoBehaviour
{
    public Transform left;
    public Transform right;
    public Transform center;

    public float LD;
    public float DR;

    // Update is called once per frame
    void Update()
    {
        Vector2 c = transform.position.ProjectVertical();
        Vector2 l = left.transform.position.ProjectVertical() - c;
        Vector2 r = right.transform.position.ProjectVertical() - c;
        Vector2 d = center.transform.position.ProjectVertical() - c;

        LD = Extensions.AngleBetween(l, d);
        DR = Extensions.AngleBetween(d, r);

    }
}
