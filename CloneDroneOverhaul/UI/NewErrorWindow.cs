using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class NewErrorWindow : ModGUIBase
    {
        private Text Stackrace;

        public override void OnInstanceStart()
        {
            Hide();
            MyModdedObject = base.GetComponent<ModdedObject>();
            Stackrace = MyModdedObject.GetObjectFromList<Text>(4);
            MyModdedObject.GetObjectFromList<Button>(6).onClick.AddListener(new UnityEngine.Events.UnityAction(BaseUtils.IgnoreLastCrash));
            MyModdedObject.GetObjectFromList<Button>(8).onClick.AddListener(new UnityEngine.Events.UnityAction(delegate
            {
                BoltGlobalEventListenerSingleton<SceneTransitionManager>.Instance.DisconnectAndExitToMainMenu();
                GameUIRoot.Instance.ErrorWindow.Hide();
            })); // Crash_WhatHappened
            MyModdedObject.GetObjectFromList<Button>(7).onClick.AddListener(new UnityEngine.Events.UnityAction(delegate
            {
                BaseUtils.QuitGame();
            }));

        }

        public void Show(string text)
        {
            base.gameObject.SetActive(true);
            Stackrace.text = text;

            MyModdedObject.GetObjectFromList<Text>(1).text = OverhaulMain.GetTranslatedString("Crash_WhatHappened");
            MyModdedObject.GetObjectFromList<Text>(0).text = OverhaulMain.GetTranslatedString("Crash_Ohno");
            MyModdedObject.GetObjectFromList<Text>(2).text = OverhaulMain.GetTranslatedString("Crash_IgnoreCrash");
            MyModdedObject.GetObjectFromList<Text>(9).text = OverhaulMain.GetTranslatedString("Crash_MainMenu");
            MyModdedObject.GetObjectFromList<Text>(3).text = OverhaulMain.GetTranslatedString("EscMenu_ExitDesktop");
            MyModdedObject.GetObjectFromList<Text>(5).text = OverhaulDescription.GetModName(true) + " | Game Ver. " + VersionNumberManager.Instance.GetVersionString() + " | " + LevelManager.Instance.GetLastSpawnedLevelID() + " | " + ArenaLiftManager.Instance.GetLiftTarget().ToString() + " | " + GameFlowManager.Instance.GetCurrentGameMode();
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            GameUIRoot.Instance.RefreshCursorEnabled();
        }
    }
}
