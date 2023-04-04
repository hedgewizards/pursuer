using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapePatterns
{
    public static float sqrt3 = Mathf.Sqrt(3);
    public static Vector3[] Get(int count)
    {
        Vector3[] pattern = new Vector3[count];
        if(count == 1)
        {
            pattern[0] = new Vector3(0, -sqrt3);
        }
        else if(count == 2)
        {
            pattern[0] = new Vector3(0, -sqrt3);
            pattern[1] = new Vector3(1.5f, -sqrt3/2);
        }
        else if(count == 3)
        {
            pattern[0] = new Vector3(-1.5f, -sqrt3/2);
            pattern[1] = new Vector3(0, -sqrt3);
            pattern[2] = new Vector3(1.5f, -sqrt3/2);
        }
        else if(count == 4)
        {
            pattern[0] = new Vector3(-1.5f, -sqrt3/2);
            pattern[1] = new Vector3(0, -sqrt3);
            pattern[2] = new Vector3(1.5f, -sqrt3/2);
            pattern[3] = new Vector3(0, sqrt3);
        }
        else if(count == 5)
        {
            pattern[0] = new Vector3(-1.5f, sqrt3/2);
            pattern[1] = new Vector3(-1.5f, -sqrt3/2);
            pattern[2] = new Vector3(0, -sqrt3);
            pattern[3] = new Vector3(1.5f, -sqrt3/2);
            pattern[4] = new Vector3(1.5f, sqrt3/2);
        }
        else if(count >= 6)
        {
            for(int n = 0; n < count; n++)
            {
                int m = n % 6;
                if (m == 0)
                {
                    pattern[n] = new Vector3(-1.5f, sqrt3/2);
                }
                else if (m == 1)
                {
                    pattern[n] = new Vector3(-1.5f, -sqrt3/2);
                }
                else if (m == 2)
                {
                    pattern[n] = new Vector3(0, -sqrt3);
                }
                else if(m == 3)
                {
                    pattern[n] = new Vector3(1.5f, -sqrt3/2);
                }
                else if(m == 4)
                {
                    pattern[n] = new Vector3(1.5f, sqrt3/2);
                }
                else if(m == 5)
                {
                    pattern[n] = new Vector3(0, sqrt3);
                }
            }
        }

        return pattern;
    }
}
