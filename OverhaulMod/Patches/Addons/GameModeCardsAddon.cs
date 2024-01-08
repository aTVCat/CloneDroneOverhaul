using OverhaulMod.Utils;
using UnityEngine.Events;

namespace OverhaulMod.Patches.Addons
{
    internal class GameModeCardsAddon : GameAddon
    {
        public override void Patch()
        {
            GameModeCardData[] datas = ModCache.titleScreenUI.SingleplayerModeSelectScreen.GameModeData;
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.ChapterSelectMenuRework))
            {
                UnityEvent storyModeEvent = new UnityEvent();
                storyModeEvent.AddListener(ModUIConstants.ShowChapterSelectMenu);
                datas[0].ClickedCallback = storyModeEvent;
            }
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.EndlessModeMenu))
            {
                UnityEvent endlessModeEvent = new UnityEvent();
                endlessModeEvent.AddListener(ModUIConstants.ShowEndlessModeMenu);
                datas[1].ClickedCallback = endlessModeEvent;
            }
        }
    }
}
