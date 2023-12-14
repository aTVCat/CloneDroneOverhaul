using OverhaulMod.Utils;
using UnityEngine.Events;

namespace OverhaulMod.Patches.Replacements
{
    internal class GameModeSelectMenusReplacement : ModReplacement
    {
        public override void Patch()
        {
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.EndlessModeMenu))
            {
                UnityEvent endlessModeEvent = new UnityEvent();
                endlessModeEvent.AddListener(ModUIConstants.ShowEndlessModeMenu);
                UnityEvent storyModeEvent = new UnityEvent();
                storyModeEvent.AddListener(ModUIConstants.ShowChapterSelectMenu);

                GameModeCardData[] datas = ModCache.titleScreenUI.SingleplayerModeSelectScreen.GameModeData;
                datas[0].ClickedCallback = storyModeEvent;
                datas[1].ClickedCallback = endlessModeEvent;
            }
        }
    }
}
