using OverhaulMod.Utils;
using UnityEngine.Events;

namespace OverhaulMod.Patches.Replacements
{
    internal class GameModeSelectMenusSimplePatch : ModReplacement
    {
        public override void Patch()
        {
            if (ModFeatures.IsEnabled(ModFeatures.EModFeature.EndlessModeMenu))
            {
                UnityEvent unityEvent = new UnityEvent();
                unityEvent.AddListener(UIConstants.ShowEndlessModeMenu);

                GameModeCardData data = ModCache.titleScreenUI.SingleplayerModeSelectScreen.GameModeData[1];
                data.ClickedCallback = unityEvent;
            }
        }
    }
}
