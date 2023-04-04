using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface
{
    Vector3 position;
    Vector3 normal;
    public readonly bool isNull;

    public Surface()
    {
        position = Vector3.zero;
        normal = Vector3.up;
        isNull = true;
    }
    public Surface(Vector3 _position, Vector3 _normal)
    {
        position = _position;
        normal = _normal;
        isNull = false;
    }

    public Vector3 GetPosition()
    {
        return position;
    }
    public Vector3 GetNormal()
    {
        return normal;
    }
}
