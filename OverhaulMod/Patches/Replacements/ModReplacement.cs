using System;
using UnityEngine;

namespace OverhaulMod.Patches.Replacements
{
    internal class ModReplacement : MonoBehaviour
    {
        private static GameObject s_GameObject;
        private static ModReplacement[] s_Patches;

        public static void Load()
        {
            if (!s_GameObject)
            {
                GameObject gameObject = new GameObject("Simple patches");
                gameObject.transform.SetParent(ModManagers.Instance.transform);
                s_GameObject = gameObject;
            }

            if (s_Patches != null)
            {
                foreach (ModReplacement patch in s_Patches)
                    Destroy(patch);

                s_Patches = Array.Empty<ModReplacement>();
            }

            _ = s_GameObject.AddComponent<GameModeSelectMenusSimplePatch>();

            s_Patches = s_GameObject.GetComponents<ModReplacement>();
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
