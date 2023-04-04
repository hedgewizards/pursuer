using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVisualizer : MonoBehaviour
{
    public Color color = Color.green;
    [HideInInspector] public Mesh mesh;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation, transform.lossyScale);
    }
    private void OnValidate()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    private void Reset()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }
}
