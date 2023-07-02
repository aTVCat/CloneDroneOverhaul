using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulAPI.SharedMonoBehaviours
{
    [RequireComponent(typeof(Camera))]
    public class OverhaulPostProcessBehaviour : MonoBehaviour
    {
        private static readonly List<OverhaulPostProcessBehaviour> _spawnedBehaviours = new List<OverhaulPostProcessBehaviour>();

        /// <summary>
        /// Clear the list of spawned post processing effect
        /// </summary>
        public static void Reset()
        {
            _spawnedBehaviours.Clear();
        }

        public static void APIUpdate()
        {
            if (_spawnedBehaviours.Count == 0)
            {
                return;
            }
            int i = 0;
            do
            {
                OverhaulPostProcessBehaviour b = _spawnedBehaviours[i];
                b.enabled = b.EnableCondition == null || b.EnableCondition();
                i++;
            } while (i < _spawnedBehaviours.Count);
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

            foreach (Material mat in imageEffectMaterials)
            {
                OverhaulPostProcessBehaviour r = cam.gameObject.AddComponent<OverhaulPostProcessBehaviour>();
                r.PostProcessMaterial = mat;
                r.EnableCondition = enableCondition;
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
            r.IsSupported = imageEffectMaterial.shader != null && imageEffectMaterial.shader.isSupported;
        }

        /// <summary>
        /// The material of image effect
        /// </summary>
        public Material PostProcessMaterial;

        public Func<bool> EnableCondition;

        public bool IsSupported;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (IsSupported)
            {
                Graphics.Blit(source, destination, PostProcessMaterial);
            }
        }

        private void Awake()
        {
            _spawnedBehaviours.Add(this);
            enabled = true;
        }

        private void OnDestroy()
        {
            if (_spawnedBehaviours.Contains(this))
            {
                _ = _spawnedBehaviours.Remove(this);
            }
        }

    }
}