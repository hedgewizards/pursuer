using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyLine 
{
    int pointCount;

    Vector3[] points;
    public Vector3 GetControlPoint(int n)
    {
        return points[n];
    }
    public void SetControlPoint(int n, Vector3 val)
    {
        points[n] = val;
        if(n > 0)
        {
            //if this isn't the first element, recalc left length
            lengths[n - 1] = Vector3.Distance(points[n - 1], points[n]);
        }
        if(n < totalLength - 1)
        {
            //if this isn't the last element, recalc right length
            lengths[n] = Vector3.Distance(points[n], points[n + 1]);
        }
    }
    
    float[] lengths;
    float totalLength;

    public PolyLine(int _pointCount)
    {
        pointCount = _pointCount;
        points = new Vector3[pointCount];
        lengths = new float[pointCount - 1];
        totalLength = 0;
    }
    public PolyLine(Vector3[] _points)
    {
        pointCount = _points.Length;
        points = _points;
        lengths = new float[pointCount - 1];
        for(int n = 0; n < pointCount - 1; n++)
        {
            lengths[n] = Vector3.Distance(points[n], points[n + 1]);
        }
        RecalculateLength();
    }
    public void RecalculateLength()
    {
        totalLength = 0;
        foreach(float length in lengths)
        {
            totalLength += length;
        }
    }
    public Vector3 GetPoint(float t)
    {
        if(t == 1)
        {
            //special case because floating points imprecision
            return points[pointCount - 1];
        }

        float targL = Mathf.Clamp01(t) * totalLength;
        float curL = 0;
        int n = -1;
        while(curL <= targL)
        {
            n++;
            curL += lengths[n];
        }
        //point lies between n and n+1
        curL -= lengths[n];
        float segT = (targL - curL) / lengths[n];
        return Vector3.Lerp(points[n], points[n + 1], segT);
    }
    public Vector3 GetDirection(float t)
    {
        if(t == 1)
        {
            //just use a point earlier in the final line for the final point
            t -= 1f / pointCount / 2;
        }
        float targL = Mathf.Clamp01(t) * totalLength;
        float curL = 0;
        int n = -1;
        while (curL <= targL)
        {
            n++;
            curL += lengths[n];
        }
        //point lies between n and n+1
        return (points[n + 1] - points[n]).normalized;
    }
}
