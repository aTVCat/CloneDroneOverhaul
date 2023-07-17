using CDOverhaul.DevTools;
using Jint.Native.Set;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulCrashScreen : OverhaulUI 
    {
        private static OverhaulCrashScreen s_Instance;
        public static OverhaulCrashScreen Instance => OverhaulMod.IsHUDInitialized && OverhaulMod.IsModInitialized ? s_Instance : null;

        private Text m_ErrorText;
        private Text m_StackTraceText;

        private Button m_IgnoreCrash;
        private Button m_MainMenu;
        private Button m_ExitGame;

        private Button m_ViewStackTrace;
        private Button m_MakeScreenshot;

        private GameObject m_StackTraceWindow;
        private Button m_CloseStackTrace;

        private bool m_IsScreenshotting;

        public override void Initialize()
        {
            s_Instance = this;

            m_ErrorText = MyModdedObject.GetObject<Text>(0);
            m_StackTraceText = MyModdedObject.GetObject<Text>(8);

            m_IgnoreCrash = MyModdedObject.GetObject<Button>(1);
            m_IgnoreCrash.onClick.AddListener(IgnoreCrash);
            m_MainMenu = MyModdedObject.GetObject<Button>(2);
            m_MainMenu.onClick.AddListener(GoToMainMenu);
            m_ExitGame = MyModdedObject.GetObject<Button>(3);
            m_ExitGame.onClick.AddListener(ExitGame);

            m_ViewStackTrace = MyModdedObject.GetObject<Button>(4);
            m_ViewStackTrace.onClick.AddListener(OpenStackTrace);
            m_MakeScreenshot = MyModdedObject.GetObject<Button>(5);
            m_MakeScreenshot.onClick.AddListener(TriggerScreenshot);

            m_StackTraceWindow = MyModdedObject.GetObject<Transform>(6).gameObject;
            m_StackTraceWindow.gameObject.SetActive(false);
            m_CloseStackTrace = MyModdedObject.GetObject<Button>(7);
            m_CloseStackTrace.onClick.AddListener(CloseStackTrace);

            Hide();
        }

        public void Show(string logString, string stackTrace)
        {
            m_ErrorText.text = logString;
            m_StackTraceText.text = logString + " " + stackTrace;

            base.gameObject.SetActive(true);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);

            ShowCursor = true;
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);

            if (GameModeManager.IsOnTitleScreen())
            {
                GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
            }

            ShowCursor = false;
        }

        public void OpenStackTrace()
        {
            m_StackTraceWindow.SetActive(true);
        }

        public void CloseStackTrace()
        {
            m_StackTraceWindow.SetActive(false);
        }

        public void IgnoreCrash()
        {
            OverhaulDebugActions.IgnoreCrash();
            Hide();
        }

        public void GoToMainMenu()
        {
            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
            Hide();
        }

        public void ExitGame()
        {
            OverhaulTransitionController.DoTransitionWithAction(delegate
            {
                Application.Quit();
            });
        }

        public void TriggerScreenshot()
        {
            if (m_IsScreenshotting || !SteamAPI.IsSteamRunning())
                return;

            m_IsScreenshotting = true;
            StartCoroutine(triggerScreenshotCoroutine());
        }

        private IEnumerator triggerScreenshotCoroutine()
        {
            OpenStackTrace();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            SteamScreenshots.TriggerScreenshot();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            CloseStackTrace();
            m_IsScreenshotting = false;
        }
    }
}
