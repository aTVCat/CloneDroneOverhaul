using UnityEngine;
using UnityEngine.Networking;

namespace OverhaulMod.Utils
{
    internal static class ModUnityUtils
    {
        public static string GetWebRequestErrorString(UnityWebRequest unityWebRequest)
        {
            if (unityWebRequest == null)
                return null;

            if (!unityWebRequest.error.IsNullOrEmpty())
            {
                return unityWebRequest.error;
            }
            else if (unityWebRequest.isHttpError)
            {
                return "Unknown HTTP error.";
            }
            else if (unityWebRequest.isNetworkError)
            {
                return "Unknown network error.";
            }
            return null;
        }

        public static void SetEmissionEnabled(this ParticleSystem particleSystem, bool enabled)
        {
            if (!particleSystem) return;

            ParticleSystem.EmissionModule em = particleSystem.emission;
            em.enabled = enabled;
        }

        public static void DisableRendererAndCollider(GameObject gameObject)
        {
            if (!gameObject)
                return;

            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer)
                renderer.enabled = false;

            Collider collider = gameObject.GetComponent<Collider>();
            if (collider)
                collider.enabled = false;
        }

        public static Sprite ToSprite(this Texture2D texture2D) => texture2D ? Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect) : null;
    }
}