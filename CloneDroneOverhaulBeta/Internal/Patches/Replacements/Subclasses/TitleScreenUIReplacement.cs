using CDOverhaul.HUD;
using CDOverhaul.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class TitleScreenUIReplacement : ReplacementBase
    {
        private Transform _buttonsTransform;
        private Transform _spawnedPanel;

        private Text m_SettingsText;
        private Text m_BugReportText;

        public override void Replace()
        {
            base.Replace();
            TitleScreenUI target = GameUIRoot.Instance.TitleScreenUI;

            _buttonsTransform = TransformUtils.FindChildRecursive(target.transform, "BottomButtons");
            if (_buttonsTransform == null)
            {
                SuccessfullyPatched = false;
                return;
            }

            GameObject panel = OverhaulMod.Core.HUDController.GetHUDPrefab("TitleScreenUI_Buttons");
            if (panel == null)
            {
                SuccessfullyPatched = false;
                return;
            }
            _spawnedPanel = GameObject.Instantiate(panel, _buttonsTransform).transform;
            _spawnedPanel.SetAsFirstSibling();
            _spawnedPanel.gameObject.SetActive(true);

            ModdedObject moddedObject = _spawnedPanel.GetComponent<ModdedObject>();
            moddedObject.GetObject<Button>(1).onClick.AddListener(OverhaulController.GetController<OverhaulParametersMenu>().Show);
            moddedObject.GetObject<Button>(3).onClick.AddListener(OverhaulController.GetController<OverhaulLocalizationEditor>().Show);
            moddedObject.GetObject<Button>(6).onClick.AddListener(delegate
            {
                Application.OpenURL("https://forms.gle/SmA9AoBfpxr1Pg676");
            });
            moddedObject.GetObject<Transform>(3).gameObject.SetActive(OverhaulVersion.IsDebugBuild);
            m_BugReportText = moddedObject.GetObject<Text>(7);
            m_SettingsText = moddedObject.GetObject<Text>(4);

            _buttonsTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            _buttonsTransform.localPosition = new Vector3(0, -170f, 0);

            OverhaulEventManager.AddEventListener(GlobalEvents.UILanguageChanged, localizeTexts, true);
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
                OverhaulEventManager.RemoveEventListener(GlobalEvents.UILanguageChanged, localizeTexts, true);
                _buttonsTransform.localScale = Vector3.one;
                _buttonsTransform.localPosition = new Vector3(0, -195.5f, 0);

                if (_spawnedPanel != null)
                {
                    GameObject.Destroy(_spawnedPanel.gameObject);
                }
            }
        }
    }
}
