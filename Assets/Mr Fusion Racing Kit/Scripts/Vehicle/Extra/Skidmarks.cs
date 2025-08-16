using UnityEngine;
using System.Collections;

namespace RGSK
{
	public class Skidmarks : MonoBehaviour 
	{
		public Material skidmarksMaterial;
		public int MaxMarks = 1024;
		public float MarkWidth = 0.25f; 
		public float GroundOffset = 0.02f; 
		public float MinDistance = 0.1f;
		private int markIndex;
		private MarkSection[] skidmarks;
		private Mesh marksMesh;
		private MeshRenderer meshRenderer;
		private MeshFilter meshFilter;
		private Vector3[] vertices;
		private Vector3[] normals;
		private Vector4[] tangents;
		private Color32[] colors;
		private Vector2[] uvs;
		private int[] triangles;
		private bool updated;
		private bool haveSetBounds;

        class MarkSection
        {
            public Vector3 Pos = Vector3.zero;
            public Vector3 Normal = Vector3.zero;
            public Vector4 Tangent = Vector4.zero;
            public Vector3 Posl = Vector3.zero;
            public Vector3 Posr = Vector3.zero;
            public byte Intensity;
            public int LastIndex;
        }


        void Start()
		{
			skidmarks = new MarkSection[MaxMarks];

			for (int i = 0; i < MaxMarks; i++)
			{
				skidmarks[i] = new MarkSection();
			}

			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();

			if (meshRenderer == null)
			{
				meshRenderer = gameObject.AddComponent<MeshRenderer>();
			}

			marksMesh = new Mesh();
			marksMesh.MarkDynamic();

			if (meshFilter == null)
			{
				meshFilter = gameObject.AddComponent<MeshFilter>();
			}

			meshFilter.sharedMesh = marksMesh;

			vertices = new Vector3[MaxMarks * 4];
			normals = new Vector3[MaxMarks * 4];
			tangents = new Vector4[MaxMarks * 4];
			colors = new Color32[MaxMarks * 4];
			uvs = new Vector2[MaxMarks * 4];
			triangles = new int[MaxMarks * 6];

			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			meshRenderer.material = skidmarksMaterial;
			meshRenderer.receiveShadows = false;
			meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

			transform.position = Vector3.zero;
		}


		void LateUpdate()
		{
			if (!updated)
                return;

			updated = false;

			// Reassign the mesh if it's changed this frame
			marksMesh.vertices = vertices;
			marksMesh.normals = normals;
			marksMesh.tangents = tangents;
			marksMesh.triangles = triangles;
			marksMesh.colors32 = colors;
			marksMesh.uv = uvs;

			if (!haveSetBounds)
			{
				marksMesh.bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(10000, 10000, 10000));
				haveSetBounds = true;
			}

			meshFilter.sharedMesh = marksMesh;
		}


		public int AddSkidMark(Vector3 pos, Vector3 normal, float intensity, int lastIndex)
		{
            if (intensity > 1)
            {
                intensity = 1.0f;
            }
            else if (intensity < 0) return -1; if (lastIndex > 0)
			{
				float sqrDistance = (pos - skidmarks[lastIndex].Pos).sqrMagnitude;
				if (sqrDistance < (MinDistance * MinDistance)) return lastIndex;
			}

			MarkSection curSection = skidmarks[markIndex];

			curSection.Pos = pos + normal * GroundOffset;
			curSection.Normal = normal;
			curSection.Intensity = (byte)(intensity * 255f);
			curSection.LastIndex = lastIndex;

			if (lastIndex != -1)
			{
				MarkSection lastSection = skidmarks[lastIndex];
				Vector3 dir = (curSection.Pos - lastSection.Pos);
				Vector3 xDir = Vector3.Cross(dir, normal).normalized;

				curSection.Posl = curSection.Pos + xDir * MarkWidth * 0.5f;
				curSection.Posr = curSection.Pos - xDir * MarkWidth * 0.5f;
				curSection.Tangent = new Vector4(xDir.x, xDir.y, xDir.z, 1);

				if (lastSection.LastIndex == -1)
				{
					lastSection.Tangent = curSection.Tangent;
					lastSection.Posl = curSection.Pos + xDir * MarkWidth * 0.5f;
					lastSection.Posr = curSection.Pos - xDir * MarkWidth * 0.5f;
				}
			}

			UpdateSkidmarksMesh();

			int curIndex = markIndex;

			markIndex = ++markIndex % MaxMarks;

			return curIndex;
		}


		void UpdateSkidmarksMesh()
		{
            if (!meshRenderer.enabled)
                return;

            MarkSection curr = skidmarks[markIndex];

			// Nothing to connect to yet
			if (curr.LastIndex == -1) return;

			MarkSection last = skidmarks[curr.LastIndex];
			vertices[markIndex * 4 + 0] = last.Posl;
			vertices[markIndex * 4 + 1] = last.Posr;
			vertices[markIndex * 4 + 2] = curr.Posl;
			vertices[markIndex * 4 + 3] = curr.Posr;

			normals[markIndex * 4 + 0] = last.Normal;
			normals[markIndex * 4 + 1] = last.Normal;
			normals[markIndex * 4 + 2] = curr.Normal;
			normals[markIndex * 4 + 3] = curr.Normal;

			tangents[markIndex * 4 + 0] = last.Tangent;
			tangents[markIndex * 4 + 1] = last.Tangent;
			tangents[markIndex * 4 + 2] = curr.Tangent;
			tangents[markIndex * 4 + 3] = curr.Tangent;

			colors[markIndex * 4 + 0] = new Color32(0, 0, 0, last.Intensity);
			colors[markIndex * 4 + 1] = new Color32(0, 0, 0, last.Intensity);
			colors[markIndex * 4 + 2] = new Color32(0, 0, 0, curr.Intensity);
			colors[markIndex * 4 + 3] = new Color32(0, 0, 0, curr.Intensity);

			uvs[markIndex * 4 + 0] = new Vector2(0, 0);
			uvs[markIndex * 4 + 1] = new Vector2(1, 0);
			uvs[markIndex * 4 + 2] = new Vector2(0, 1);
			uvs[markIndex * 4 + 3] = new Vector2(1, 1);

			triangles[markIndex * 6 + 0] = markIndex * 4 + 0;
			triangles[markIndex * 6 + 2] = markIndex * 4 + 1;
			triangles[markIndex * 6 + 1] = markIndex * 4 + 2;

			triangles[markIndex * 6 + 3] = markIndex * 4 + 2;
			triangles[markIndex * 6 + 5] = markIndex * 4 + 1;
			triangles[markIndex * 6 + 4] = markIndex * 4 + 3;

			updated = true;
		}


		public void HideSkidmarks()
		{
            meshRenderer.enabled = false;
		}


        public void ShowSkidmarks()
        {
            meshRenderer.enabled = true;
        }
    }
}
