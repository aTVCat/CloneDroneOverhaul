using CDOverhaul.HUD;
using CDOverhaul.Localization;
using CDOverhaul.NetworkAssets.AdditionalContent;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class TitleScreenUIReplacement : ReplacementBase
    {
        private Transform m_ButtonsTransform;
        private Transform m_SpawnedPanel;

        private RectTransform m_MultiplayerNEWButtonTransform;

        private Text m_SettingsText;
        private Text m_BugReportText;

        public override void Replace()
        {
            base.Replace();
            TitleScreenUI target = GameUIRoot.Instance.TitleScreenUI;

            m_ButtonsTransform = TransformUtils.FindChildRecursive(target.transform, "BottomButtons");
            if (m_ButtonsTransform == null)
            {
                SuccessfullyPatched = false;
                return;
            }

            m_MultiplayerNEWButtonTransform = TransformUtils.FindChildRecursive(target.transform, "MultiplayerButton_NEW") as RectTransform;
            if (m_MultiplayerNEWButtonTransform == null)
            {
                SuccessfullyPatched = false;
                return;
            }
            m_MultiplayerNEWButtonTransform.localPosition = new Vector3(0, -85f, 0);

            GameObject panel = OverhaulMod.Core.CanvasController.GetHUDPrefab("TitleScreenUI_Buttons");
            if (panel == null)
            {
                SuccessfullyPatched = false;
                return;
            }
            m_SpawnedPanel = GameObject.Instantiate(panel, m_ButtonsTransform).transform;
            m_SpawnedPanel.SetAsFirstSibling();
            m_SpawnedPanel.gameObject.SetActive(true);

            ModdedObject moddedObject = m_SpawnedPanel.GetComponent<ModdedObject>();
            moddedObject.GetObject<Button>(1).onClick.AddListener(OverhaulController.GetController<OverhaulParametersMenu>().Show);
            moddedObject.GetObject<Button>(3).onClick.AddListener(OverhaulController.GetController<OverhaulLocalizationEditor>().Show);
            moddedObject.GetObject<Button>(6).onClick.AddListener(delegate
            {
                Application.OpenURL("https://forms.gle/SmA9AoBfpxr1Pg676");
            });
            moddedObject.GetObject<Button>(8).onClick.AddListener(OverhaulController.GetController<OverhaulAdditionalContentUI>().Show);
            moddedObject.GetObject<Transform>(9).gameObject.SetActive(OverhaulVersion.IsDebugBuild);
            m_BugReportText = moddedObject.GetObject<Text>(7);
            m_SettingsText = moddedObject.GetObject<Text>(4);

            m_ButtonsTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            m_ButtonsTransform.localPosition = new Vector3(0, -158f, 0);

            OverhaulEventsController.AddEventListener(GlobalEvents.UILanguageChanged, localizeTexts, true);
            localizeTexts();

            SuccessfullyPatched = true;
        }

        private void localizeTexts()
        {
            if (!SuccessfullyPatched || m_BugReportText == null || m_SettingsText == null || OverhaulLocalizationController.Error)
            {
                return;
            }

            m_BugReportText.text = OverhaulLocalizationController.Localization.GetTranslation("TitleScreen_BugReport");
            m_SettingsText.text = OverhaulLocalizationController.Localization.GetTranslation("TitleScreen_Settings");
        }

        public override void Cancel()
        {
            base.Cancel();
            if (SuccessfullyPatched)
            {
                OverhaulEventsController.RemoveEventListener(GlobalEvents.UILanguageChanged, localizeTexts, true);
                m_ButtonsTransform.localScale = Vector3.one;
                m_ButtonsTransform.localPosition = new Vector3(0, -195.5f, 0);
                m_MultiplayerNEWButtonTransform.localPosition = new Vector3(0, -87.8241f, 0);

                if (m_SpawnedPanel != null)
                {
                    GameObject.Destroy(m_SpawnedPanel.gameObject);
                }
            }
        }
    }
}
