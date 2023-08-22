﻿using CDOverhaul.DevTools;
using CDOverhaul.Patches;
using Steamworks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulCrashScreen : OverhaulUIController
    {
        private static bool s_HasSentReport;

        private static OverhaulCrashScreen s_Instance;
        public static OverhaulCrashScreen Instance => OverhaulMod.IsHUDInitialized && OverhaulMod.IsModInitialized ? s_Instance : null;

        [ObjectReference("ErrorMessage")]
        private Text m_ErrorText;
        [ObjectReference("stacktraceText")]
        private Text m_StackTraceText;

        [ActionReference(nameof(IgnoreCrash))]
        [ObjectReference("IgnoreCrash")]
        private Button m_IgnoreCrash;
        [ActionReference(nameof(GoToMainMenu))]
        [ObjectReference("MainMenu")]
        private Button m_MainMenu;
        [ActionReference(nameof(ExitGame))]
        [ObjectReference("ExitGame")]
        private Button m_ExitGame;

        [ActionReference(nameof(OpenStackTrace))]
        [ObjectReference("ViewStacktrace")]
        private Button m_ViewStackTrace;
        [ActionReference(nameof(TriggerScreenshot))]
        [ObjectReference("Screenshot")]
        private Button m_MakeScreenshot;

        [ActionReference(nameof(SendReport))]
        [ObjectReference("SendReport")]
        private Button m_SendReport;
        [ObjectReference("SendReport")]
        private CanvasGroup m_SendReportCanvasGroup;

        [ObjectReference("StackTrace")]
        private GameObject m_StackTraceWindow;
        [ActionReference(nameof(CloseStackTrace))]
        [ObjectReference("CloseStackTraceButton")]
        private Button m_CloseStackTrace;

        private bool m_IsScreenshotting;

        public override void Initialize()
        {
            base.Initialize();
            s_Instance = this;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            s_Instance = null;
        }

        public void Show(string logString, string stackTrace)
        {
            m_ErrorText.text = logString;
            m_StackTraceText.text = logString + " " + stackTrace;

            base.gameObject.SetActive(true);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);

            ShowCursor = true;
            SetSendReportButtonInteractable(!s_HasSentReport);
        }

        public override void Hide()
        {
            base.Hide();
            if (GameModeManager.IsOnTitleScreen())
            {
                GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
            }
            ShowCursor = false;
        }

        public void SetSendReportButtonInteractable(bool value)
        {
            m_SendReport.interactable = value;
            m_SendReportCanvasGroup.alpha = value ? 1f : 0.25f;
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

        public void SendReport()
        {
            if (s_HasSentReport)
                return;

            if (string.IsNullOrEmpty(ErrorManager_Patch.Report))
                return;

            OverhaulWebhooksController.ExecuteCrashReportsWebhook(ErrorManager_Patch.Report);
            s_HasSentReport = true;
            SetSendReportButtonInteractable(false);
        }

        public void TriggerScreenshot()
        {
            if (m_IsScreenshotting || !SteamAPI.IsSteamRunning())
                return;

            m_IsScreenshotting = true;
            _ = StartCoroutine(triggerScreenshotCoroutine());
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
