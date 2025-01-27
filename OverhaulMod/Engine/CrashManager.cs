﻿namespace OverhaulMod.Engine
{
    public static class CrashManager
    {
        public static bool HasCrashedThisSession;

        public static bool IgnoreCrashes;

        public static bool OnGameCrashed()
        {
            LevelManager levelManager = LevelManager.Instance;
            if (levelManager && (levelManager._isSpawningCurrentLevel || levelManager._currentlySwappingInLevel))
            {
                GameFlowManager gameFlowManager = GameFlowManager.Instance;
                if (gameFlowManager)
                    gameFlowManager.StopAllCoroutines();

                levelManager.StopAllCoroutines();
                levelManager._isSpawningCurrentLevel = false;
                levelManager._currentlySwappingInLevel = false;
                GameUIRoot.HideSaveIndicator();

                if (GameModeManager.IsOnTitleScreen())
                {
                    TitleScreenCustomizationManager titleScreenCustomizationManager = TitleScreenCustomizationManager.Instance;
                    if (titleScreenCustomizationManager)
                    {
                        titleScreenCustomizationManager.overrideLevelDescription = null;
                        titleScreenCustomizationManager.SaveCustomizationInfo();
                    }
                }

                return true;
            }
            return false;
        }
    }
}
