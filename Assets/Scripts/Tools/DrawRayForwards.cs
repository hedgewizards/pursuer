using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * this debug script helps visualize the position and orientation
 * of objects that don't have a visual component in-editor.
 * I use it on room entrances and exits so I can make sure they
 * are lined up correctly
 */

public class DrawRayForwards : MonoBehaviour
{
    public bool alwaysDraw = true;
    public Color color = Color.red;

    private void OnDrawGizmos()
    {
        //only draw when selected... skip this
        if (!alwaysDraw)
            return;

        DrawTheThing();
    }
    private void OnDrawGizmosSelected()
    {
        //always draw, so it gets drawn in OnDrawGizmos already
        if (alwaysDraw)
            return;

        DrawTheThing();
    }

    void DrawTheThing()
    {
        Gizmos.color = color;
        Gizmos.DrawLine(transform.position,transform.position + transform.forward*2);
        Gizmos.DrawLine(transform.position + 1f*transform.right, transform.position + transform.forward * 2);
        Gizmos.DrawLine(transform.position - 1f * transform.right, transform.position + transform.forward * 2);

    }
}
