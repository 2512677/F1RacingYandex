using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class RearCamera : MonoBehaviour
    {
        Camera cam;

        void Start()
        {
            cam = GetComponent<Camera>();
        }

        void OnPreCull()
        {
            cam.ResetWorldToCameraMatrix();
            cam.ResetProjectionMatrix();
            cam.projectionMatrix = cam.projectionMatrix * Matrix4x4.Scale(new Vector3(-1, 1, 1));
        }

        // Set it to true so we can watch the flipped Objects
        void OnPreRender()
        {
            GL.invertCulling = true;
        }

        // Set it to false again because we dont want to affect all other cammeras.
        void OnPostRender()
        {
            GL.invertCulling = false;
        }
    }
}
