using System;
using UnityEngine;

namespace OverhaulMod.Patches.Addons
{
    public class GameAddon : OverhaulBehaviour
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

            _ = gameObject.AddComponent<GameModeCardsAddon>();
            _ = gameObject.AddComponent<ProjectileAddon>();
            _ = gameObject.AddComponent<SkyboxesAddon>();
            _ = gameObject.AddComponent<ColorsAddon>();
            _ = gameObject.AddComponent<MinorChangesAddon>();
            _ = gameObject.AddComponent<LocalizationAddon>();
            _ = gameObject.AddComponent<GameModeSelectScreensAddon>();

            s_patches = gameObject.GetComponents<GameAddon>();
        }

        public override void Start()
        {
            Patch();
        }

        public virtual void Patch()
        {

        }

        public virtual void UnPatch()
        {

        }

        public override void OnDestroy()
        {
            Debug.Log($"Destroyed addon: {base.GetType()}");
        }
    }
}
