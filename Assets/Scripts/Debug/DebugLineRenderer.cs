using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[ExecuteInEditMode]
public class DebugLineRenderer : MonoBehaviour
{
    public Color color;

    public void LateUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward, color);
    }
}
