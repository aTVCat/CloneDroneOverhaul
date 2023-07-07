using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulAPI.SharedMonoBehaviours
{
    [RequireComponent(typeof(Camera))]
    public class OverhaulCameraEffect : MonoBehaviour
    {
        private static readonly List<OverhaulCameraEffect> s_Behaviours = new List<OverhaulCameraEffect>();

        internal static void Reset()
        {
            s_Behaviours.Clear();
        }

        internal static void APIUpdate()
        {
            if (s_Behaviours.Count == 0)
                return;

            int i = 0;
            do
            {
                OverhaulCameraEffect b = s_Behaviours[i];
                b.enabled = b.EnableCondition == null || b.EnableCondition();
                i++;
            } while (i < s_Behaviours.Count);
        }

        /// <summary>
        /// Make camera use an image effect shader in <paramref name="imageEffectMaterial"/>
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="imageEffectMaterial"></param>
        /// <param name="enableCondition"></param>
        public static void AddEffect(Camera cam, Material imageEffectMaterial, Func<bool> enableCondition = null)
        {
            if (!cam)
            {
                OverhaulAPICore.ThrowException("Camera is NULL.");
            }
            if (!imageEffectMaterial)
            {
                OverhaulAPICore.ThrowException("ImageEffectMaterials are NULL.");
            }

            OverhaulCameraEffect r = cam.gameObject.AddComponent<OverhaulCameraEffect>();
            r.ShaderMaterial = imageEffectMaterial;
            r.EnableCondition = enableCondition;
            r.IsSupported = imageEffectMaterial.shader && imageEffectMaterial.shader.isSupported;
        }

        /// <summary>
        /// The material of image effect
        /// </summary>
        public Material ShaderMaterial;

        public Func<bool> EnableCondition;

        public bool IsSupported;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (IsSupported)
                Graphics.Blit(source, destination, ShaderMaterial);
        }

        private void Awake()
        {
            s_Behaviours.Add(this);
            base.enabled = true;
        }

        private void OnDestroy()
        {
            _ = s_Behaviours.Remove(this);
        }

    }
}