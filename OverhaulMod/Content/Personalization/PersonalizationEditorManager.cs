using OverhaulMod.Utils;
using System;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorManager : Singleton<PersonalizationEditorManager>
    {
        public void StartEditorGameMode()
        {
            GameFlowManager.Instance._gameMode = (GameMode)2500;
            LevelManager.Instance._currentLevelHidesTheArena = true;

            LevelManager.Instance.CleanUpLevelThisFrame();
            GameFlowManager.Instance.HideTitleScreen(false);

            GameUIRoot.Instance.LoadingScreen.Show();

            GameDataManager.Instance.SaveHighScoreDataWithoutModifyingIt();
            CacheManager.Instance.CreateOrClearInstance();
            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {
                GameUIRoot.Instance.LoadingScreen.Hide();
                GlobalEventManager.Instance.Dispatch(GlobalEvents.LevelSpawned);

                _ = new GameObject("Camera", new Type[] { typeof(Camera) });

                ModUIConstants.ShowPersonalizationEditorUI();
            });
        }

        public void CreateItem(string name, PersonalizationCategory personalizationCategory)
        {

        }

        public static string GetContentDirectoryForCategory(PersonalizationCategory personalizationCategory)
        {
            switch (personalizationCategory)
            {
                case PersonalizationCategory.WeaponSkins:
                    return "skins";
                case PersonalizationCategory.Accessories:
                    return "accessories";
                case PersonalizationCategory.Mounts:
                    return "mounts";
            }
            return "other_p";
        }
    }
}
