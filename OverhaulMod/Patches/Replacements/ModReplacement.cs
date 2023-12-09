using System;
using UnityEngine;

namespace OverhaulMod.Patches.Replacements
{
    internal class ModReplacement : MonoBehaviour
    {
        private static GameObject s_gameObject;
        private static ModReplacement[] s_patches;

        public static void Load()
        {
            if (!s_gameObject)
            {
                GameObject gameObject = new GameObject("Replacements");
                gameObject.transform.SetParent(ModManagers.Instance.transform);
                s_gameObject = gameObject;
            }

            if (s_patches != null)
            {
                foreach (ModReplacement patch in s_patches)
                    Destroy(patch);

                s_patches = Array.Empty<ModReplacement>();
            }

            _ = s_gameObject.AddComponent<GameModeSelectMenusReplacement>();

            s_patches = s_gameObject.GetComponents<ModReplacement>();
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
    }
}
