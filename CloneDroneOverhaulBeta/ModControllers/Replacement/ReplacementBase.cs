using System;
using System.Collections.Generic;

namespace CDOverhaul.Patches
{
    /// <summary>
    /// This was made to patch the game without using HarmonyPatch
    /// </summary>
    public class ReplacementBase
    {
        internal static readonly List<ReplacementBase> Replacements = new List<ReplacementBase>();

        private bool _hasAddedListeners;

        /// <summary>
        /// Check if replacement is working
        /// </summary>
        public bool HasReplaced { get; private set; }

        /// <summary>
        /// This value doesn't affect anything, use it for marking fine working patches
        /// </summary>
        protected bool SuccessfullyPatched { get; set; }

        /// <summary>
        /// Do patch
        /// </summary>
        public virtual void Replace()
        {
            if (!_hasAddedListeners)
            {
                _ = OverhaulEventManager.AddEventListener(OverhaulMod.ModDeactivatedEventString, Cancel);
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
            _ = Replacements.Remove(this);
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

            _ = NewReplacement<TitleScreenUIReplacement>();
            _ = NewReplacement<PlayerCameraPrefabReplacement>();
            _ = NewReplacement<OptimizeOnStart>();
            _ = NewReplacement<BaseFixes>();
            _ = NewReplacement<EnergyUIReplacement>();
            _ = NewReplacement<OptimizeRuntime>();
        }
    }
}
