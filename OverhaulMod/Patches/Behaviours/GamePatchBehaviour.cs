using System;
using UnityEngine;

namespace OverhaulMod.Patches.Behaviours
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
                typeof(LocalizationManagerPatchBehaviour),
                typeof(EnergyBarPatchBehaviour),
                typeof(MenuButtonsPatchBehaviour),
                typeof(SubtitleTextFieldPatchBehaviour),
                typeof(ChallengeManagerPatchBehaviour),
                typeof(CustomizationButtonPatchBehaviour)
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
            if (!ModCore.isEnabled)
                UnPatch();
        }
    }
}
