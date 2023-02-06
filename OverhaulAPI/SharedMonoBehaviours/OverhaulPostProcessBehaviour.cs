using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulAPI.SharedMonoBehaviours
{
    [RequireComponent(typeof(Camera))]
    public class OverhaulPostProcessBehaviour : MonoBehaviour
    {
        private static List<OverhaulPostProcessBehaviour> _spawnedBehaviours = new List<OverhaulPostProcessBehaviour>();

        /// <summary>
        /// Clear the list of spawned post processing effect
        /// </summary>
        public static void Reset()
        {
            _spawnedBehaviours.Clear();
        }

        public static void APIUpdate()
        {
            foreach(OverhaulPostProcessBehaviour b in _spawnedBehaviours)
            {
                if(b.EnableCondition == null)
                {
                    b.enabled = true;
                }
                else
                {
                    b.enabled = b.EnableCondition();
                }
            }
        }

        /// <summary>
        /// Make camera use the image effects shaders in <paramref name="imageEffectMaterials"/>
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="imageEffectMaterials"></param>
        /// <param name="enableCondition"></param>
        public static void AddPostProcessEffects(Camera cam, Material[] imageEffectMaterials, Func<bool> enableCondition = null)
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
                r.EnableCondition = enableCondition;
                _spawnedBehaviours.Add(r);
            }
        }
        
        /// <summary>
        /// Make camera use an image effect shader in <paramref name="imageEffectMaterial"/>
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="imageEffectMaterial"></param>
        /// <param name="enableCondition"></param>
        public static void AddPostProcessEffect(Camera cam, Material imageEffectMaterial, Func<bool> enableCondition = null)
        {
            if (cam == null)
            {
                API.ThrowException("Camera is NULL.");
            }
            if (imageEffectMaterial == null)
            {
                API.ThrowException("ImageEffectMaterials are NULL.");
            }

            OverhaulPostProcessBehaviour r = cam.gameObject.AddComponent<OverhaulPostProcessBehaviour>();
            r.PostProcessMaterial = imageEffectMaterial;
            r.EnableCondition = enableCondition;
            _spawnedBehaviours.Add(r);
        }

        /// <summary>
        /// The material of image effect
        /// </summary>
        public Material PostProcessMaterial;

        public Func<bool> EnableCondition;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (PostProcessMaterial.shader != null && PostProcessMaterial.shader.isSupported)
                Graphics.Blit(source, destination, PostProcessMaterial);
        }

        private void OnDestroy()
        {
            if (_spawnedBehaviours.Contains(this))
            {
                _spawnedBehaviours.Remove(this);
            }
        }

    }
}