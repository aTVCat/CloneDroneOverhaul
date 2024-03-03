using OverhaulMod.Utils;
using System;
using UnityEngine;

namespace OverhaulMod.Patches.Addons
{
    public class GamePatchBehaviour : ModBehaviour
    {
        private static GameObject s_gameObject;

        public static void Load()
        {
            Unload();

            GameObject gameObject = new GameObject("PatchBehaviours", new Type[]
            {
                typeof(GameModeCardsPatchBehaviour),
                typeof(ProjectilePatchBehaviour),
                typeof(SkyboxesPatchBehaviour),
                typeof(ColorsPatchBehaviour),
                typeof(MinorPatchBehaviour),
                typeof(LocalizationPatchBehaviour),
                typeof(GameModeSelectScreensPatchBehaviour),
                typeof(EnergyUIPatchBehaviour),
                typeof(MenuButtonsPatchBehaviour),
            });
            gameObject.transform.SetParent(ModManagers.Instance.transform);
            s_gameObject = gameObject;
        }

        public static void Unload()
        {
            GameObject gameObject = s_gameObject;
            if (gameObject)
                Destroy(gameObject);

            s_gameObject = null;
        }

        public static T GetBehaviour<T>() where T : GamePatchBehaviour
        {
            GameObject gameObject = s_gameObject;
            if (!gameObject)
                return null;

            return gameObject.GetComponent<T>();
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
