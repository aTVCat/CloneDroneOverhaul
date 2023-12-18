using System;
using UnityEngine;

namespace OverhaulMod.Patches.Addons
{
    internal class GameAddon : OverhaulBehaviour
    {
        private static GameObject s_gameObject;
        private static GameAddon[] s_patches;

        public static void Load()
        {
            GameObject gameObject = s_gameObject;
            if (!gameObject)
            {
                gameObject = new GameObject("Replacements");
                gameObject.transform.SetParent(ModManagers.Instance.transform);
                s_gameObject = gameObject;
            }

            GameAddon[] patches = s_patches;
            if (patches != null)
            {
                foreach (GameAddon patch in patches)
                    Destroy(patch);

                s_patches = Array.Empty<GameAddon>();
            }

            _ = gameObject.AddComponent<GameModeSelectAddon>();
            _ = gameObject.AddComponent<ProjectileAddon>();

            s_patches = gameObject.GetComponents<GameAddon>();
        }

        private void Start()
        {
            Patch();
        }

        public virtual void Patch()
        {

        }

        public virtual void UnPatch()
        {

        }

        public sealed override void OnDestroy()
        {
            Debug.Log($"Destroyed addon: {base.GetType()}");
        }
    }
}
