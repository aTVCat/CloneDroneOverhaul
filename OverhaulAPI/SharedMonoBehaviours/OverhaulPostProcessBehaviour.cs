using System;
using UnityEngine;

namespace OverhaulAPI.SharedMonoBehaviours
{
    [RequireComponent(typeof(Camera))]
    public class OverhaulPostProcessBehaviour : MonoBehaviour
    {
        public static void AddPostProcessEffects(Camera cam, Material[] imageEffectMaterials)
        {
            if (cam == null)
            {
                API.ThrowException("Camera is NULL.");
            }
            if (imageEffectMaterials == null)
            {
                API.ThrowException("ImageEffectMaterials are NULL.");
            }

            foreach(Material mat in imageEffectMaterials)
            {
                OverhaulPostProcessBehaviour r = cam.gameObject.AddComponent<OverhaulPostProcessBehaviour>();
                r.PostProcessMaterial = mat;
            }
        }

        /// <summary>
        /// The material of image effect
        /// </summary>
        public Material PostProcessMaterial;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (PostProcessMaterial.shader != null && PostProcessMaterial.shader.isSupported)
                Graphics.Blit(source, destination, PostProcessMaterial);
        }
    }
}