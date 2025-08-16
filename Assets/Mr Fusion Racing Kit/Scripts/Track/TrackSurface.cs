using UnityEngine;
using System.Collections;

namespace RGSK
{
	public class TrackSurface : MonoBehaviour 
	{
        public Surface[] surfaces;
        public CollisionEffects[] collisionEffects;

        public Surface GetSurfaceData(PhysicsMaterial material, Texture2D terrainTexture)
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                if (material != null && material == surfaces[i].physicMaterial
                    || terrainTexture != null && terrainTexture == surfaces[i].terrainTexture)

                    return surfaces[i];
            }

            return null;
        }


        public CollisionEffects GetCollisionEffectData(PhysicsMaterial material)
        {
            for (int i = 0; i < collisionEffects.Length; i++)
            {
                for (int x = 0; x < collisionEffects[i].physicMaterials.Length; x++)
                {
                    if (material == collisionEffects[i].physicMaterials[x])
                        return collisionEffects[i];
                }             
            }

            return null;
        }


        public void ToggleSkidmarkVisibility(bool show)
        {
            Skidmarks[] skidmarks = FindObjectsOfType<Skidmarks>();
            foreach(Skidmarks s in skidmarks)
            {
                if (show)
                    s.ShowSkidmarks();
                else
                    s.HideSkidmarks();
            }
        }
    }

    [System.Serializable]
    public class Surface
    {
        public string name = "New Surface";
        public PhysicsMaterial physicMaterial;
        public Texture2D terrainTexture;
        [Space(10)]
        public ParticleSystem particleSystem;
        public Skidmarks surfaceSkidmarks;
        public AudioClip soundClip;
        [Range(0f, 2f)]
        public float forwardGrip = 1;
        [Range(0f, 2f)]
        public float sidewaysGrip = 1;
        [Range(0.01f, 0.5f)]
        public float minWheelSlip = 0.3f;
        public float damping = 1;
        public bool offTrack;
    }

    [System.Serializable]
    public class CollisionEffects
    {
        public string name = "New Collision Effect";
        public PhysicsMaterial[] physicMaterials;
        [Space(10)]
        public AudioClip collisionSound;
        public AudioClip collisionScrapeSound;
        public ParticleSystem collisionParticle;
    }
}
