using System;
using UnityEngine;

namespace OverhaulMod.Patches.Addons
{
    public class GamePatchBehaviour : ModBehaviour
    {
        private static GameObject s_gameObject;
        private static GamePatchBehaviour[] s_patches;

        public static void Load()
        {
            GameObject gameObject = s_gameObject;
            if (!gameObject)
            {
                gameObject = new GameObject("PatchBehaviours");
                gameObject.transform.SetParent(ModManagers.Instance.transform);
                s_gameObject = gameObject;
            }

            Unload();

            _ = gameObject.AddComponent<GameModeCardsPatchBehaviour>();
            _ = gameObject.AddComponent<ProjectilePatchBehaviour>();
            _ = gameObject.AddComponent<SkyboxesPatchBehaviour>();
            _ = gameObject.AddComponent<ColorsPatchBehaviour>();
            _ = gameObject.AddComponent<MinorPatchBehaviour>();
            _ = gameObject.AddComponent<LocalizationPatchBehaviour>();
            _ = gameObject.AddComponent<GameModeSelectScreensPatchBehaviour>();
            _ = gameObject.AddComponent<EnergyUIPatchBehaviour>();
            _ = gameObject.AddComponent<MenuButtonsPatchBehaviour>();

            s_patches = gameObject.GetComponents<GamePatchBehaviour>();
        }

        public static void Unload()
        {
            GamePatchBehaviour[] patches = s_patches;
            if (patches != null)
            {
                foreach (GamePatchBehaviour patch in patches)
                    Destroy(patch);

                s_patches = Array.Empty<GamePatchBehaviour>();
            }
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
            UnPatch();
            Debug.Log($"Destroyed addon: {base.GetType()}");
        }
    }
}
