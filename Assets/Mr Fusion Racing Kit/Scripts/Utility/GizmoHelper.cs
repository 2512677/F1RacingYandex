using UnityEngine;
using System.Collections;

public class GizmoHelper : MonoBehaviour
{
    public enum GizmoType { Sphere, Cube};
    public GizmoType gizmoType;
    public Color gizmoColor = Color.red;
    public float size = 0.05f;

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        if(gizmoType == GizmoType.Cube)
            Gizmos.DrawCube(transform.position, Vector3.one * size);
        else
            Gizmos.DrawSphere(transform.position, size);
    }
}
