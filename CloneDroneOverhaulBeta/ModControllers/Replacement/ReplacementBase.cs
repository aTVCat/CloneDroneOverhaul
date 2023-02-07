using System;
using System.Collections.Generic;

namespace CDOverhaul.Patches
{
    /// <summary>
    /// This was made to patch the game without using HarmonyPatch
    /// </summary>
    public class ReplacementBase
    {
        internal static List<ReplacementBase> Replacements = new List<ReplacementBase>();

        public bool HasReplaced { get; private set; }
        protected bool SuccessfullyPatched { get; set; }

        private bool _hasAddedListeners;

        /// <summary>
        /// Do patch
        /// </summary>
        public virtual void Replace()
        {
            if (!_hasAddedListeners)
            {
                OverhaulEventManager.AddEventListener(OverhaulMod.ModDeactivatedEventString, Cancel);
                _hasAddedListeners = true;
            }

            HasReplaced = true;
        }

        /// <summary>
        /// Revert patch changes, called when mod is disabled
        /// </summary>
        public virtual void Cancel()
        {
            OverhaulEventManager.RemoveEventListener(OverhaulMod.ModDeactivatedEventString, Cancel);
            Replacements.Remove(this);
            HasReplaced = false;
        }

        public static T NewReplacement<T>() where T : ReplacementBase
        {
            T newR = Activator.CreateInstance<T>();
            Replacements.Add(newR);
            newR.Replace();
            return newR;
        }

        internal static void CreateReplacements()
        {
            Replacements.Clear();

            NewReplacement<TitleScreenUIReplacement>();
            NewReplacement<PlayerCameraPrefabReplacement>();
            NewReplacement<OptimizeOnStart>();
            NewReplacement<BaseFixes>();
            NewReplacement<EnergyUIReplacement>();
            NewReplacement<OptimizeRuntime>();
        }
    }
}
