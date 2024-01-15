using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace OverhaulMod.Patches.Addons
{
    internal class GameModeCardsAddon : GameAddon
    {
        public override void Patch()
        {
            GameModeCardData[] multiplayerDatas = ModCache.titleScreenUI.MultiplayerModeSelectScreen.GameModeData;
            GameModeCardData[] singleplayerDatas = ModCache.titleScreenUI.SingleplayerModeSelectScreen.GameModeData;
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.ChapterSelectMenuRework))
            {
                UnityEvent storyModeEvent = new UnityEvent();
                storyModeEvent.AddListener(ModUIConstants.ShowChapterSelectMenu);
                singleplayerDatas[0].ClickedCallback = storyModeEvent;
            }
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.EndlessModeMenu))
            {
                UnityEvent endlessModeEvent = new UnityEvent();
                endlessModeEvent.AddListener(ModUIConstants.ShowEndlessModeMenu);
                singleplayerDatas[1].ClickedCallback = endlessModeEvent;
            }

            ReplaceSprite(singleplayerDatas, 1, "Endless");
            ReplaceSprite(singleplayerDatas, 2, "Bot");
            ReplaceSprite(multiplayerDatas, 0, "Humans");
            ReplaceSprite(multiplayerDatas, 1, "Bot");
            ReplaceSprite(multiplayerDatas, 2, "Raptor");
            ReplaceSprite(multiplayerDatas, 3, "DuelHumans");
        }

        public void ReplaceSprite(GameModeCardData[] array, int index, string imageName)
        {
            string key = $"OverhaulGameModeImage_{imageName}";
            if (ModAdvancedCache.TryGet(key, out Sprite item))
            {
                array[index].ThumbnailSprite = item;
                return;
            }

            ContentRepositoryManager.Instance.GetLocalTexture(ModCore.texturesFolder + $"gamemodes/{imageName}.jpg", delegate (Texture2D texture2D)
            {
                Sprite sprite = texture2D.ToSprite();
                array[index].ThumbnailSprite = sprite;
                ModAdvancedCache.Add(key, sprite);
            }, null, out _);
        }
    }
}
