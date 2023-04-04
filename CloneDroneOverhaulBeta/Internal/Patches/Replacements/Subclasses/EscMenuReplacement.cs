using CDOverhaul.HUD;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class EscMenuReplacement : ReplacementBase
    {
        [OverhaulSetting("Shortcuts.Personalization.Skins", SettingsController.SettingEventDispatcherFlag, false, null, null, null, null)]
        public static SettingEventDispatcher OpenSkinsMenuFromSettings = new SettingEventDispatcher();

        [OverhaulSetting("Shortcuts.Personalization.Outfits", SettingsController.SettingEventDispatcherFlag, false, null, null, null, null)]
        public static SettingEventDispatcher OpenOutfitsMenuFromSettings = new SettingEventDispatcher();

        public const string OpenSkinsFromSettingsEventString = "Settings.OpenSkins";
        public const string OpenOutfitsFromSettingsEventString = "Settings.OpenOutfits";

        private RectTransform m_BG;
        private RectTransform m_BackToLvlEditorButton;
        private RectTransform m_SettingsButton;
        private RectTransform m_ModSettingsButton;

        private Vector2 m_OgSizeDelta;
        private Vector2 m_SettingsButtonOgSizeDelta;
        private Vector2 m_OgPosition;
        private Vector2 m_LvlEditorButtonOgPosition;

        private bool m_IsWaitingToPatchButtons;
        private bool m_SetUpButtons;

        private void setUpButtons()
        {
            if (m_SetUpButtons)
            {
                return;
            }
            m_SetUpButtons = true;

            OpenOutfitsMenuFromSettings.CanBeShown = shouldShowPersonalization;
            OpenOutfitsMenuFromSettings.EventString = OpenOutfitsFromSettingsEventString;
            OpenSkinsMenuFromSettings.CanBeShown = shouldShowPersonalization;
            OpenSkinsMenuFromSettings.EventString = OpenSkinsFromSettingsEventString;
        }

        private static Func<bool> shouldShowPersonalization => () => CharacterTracker.Instance.GetPlayerTransform() != null && !OverhaulPauseMenu.UseThisMenu;

        public override void Replace()
        {
            setUpButtons();
            if (SuccessfullyPatched)
            {
                return;
            }

            base.Replace();
            EscMenu target = GameUIRoot.Instance.EscMenu;
            if(target == null)
            {
                SuccessfullyPatched = false;
                return;
            }

            m_BG = target.transform.GetChild(5) as RectTransform;
            m_OgSizeDelta = m_BG.sizeDelta;
            m_OgPosition = m_BG.localPosition;
            if (m_BG == null)
            {
                SuccessfullyPatched = false;
                return;
            }
            m_BG.localPosition = new Vector3(0, -20, 0);
            m_BG.sizeDelta = new Vector2(180, 235);

            m_BackToLvlEditorButton = TransformUtils.FindChildRecursive(m_BG, "BackToLevelEditorButton") as RectTransform;
            if (m_BackToLvlEditorButton == null)
            {
                SuccessfullyPatched = false;
                return;
            }
            m_LvlEditorButtonOgPosition = m_BackToLvlEditorButton.localPosition;
            m_BackToLvlEditorButton.localPosition = new Vector3(0, -130, 0);

            m_SettingsButton = TransformUtils.FindChildRecursive(m_BG, "SettingsButton") as RectTransform;
            if (m_SettingsButton == null)
            {
                SuccessfullyPatched = false;
                return;
            }

            if (m_IsWaitingToPatchButtons)
            {
                return;
            }

            m_IsWaitingToPatchButtons = true;
            DelegateScheduler.Instance.Schedule(delegate
            {
                m_IsWaitingToPatchButtons = false;
                m_SettingsButtonOgSizeDelta = m_SettingsButton.sizeDelta;
                m_SettingsButton.sizeDelta = new Vector2(75, 30);
                m_SettingsButton.localPosition = new Vector3(-39f, m_SettingsButton.localPosition.y, 0f);

                m_ModSettingsButton = UnityEngine.Object.Instantiate(m_SettingsButton, m_BG);
                m_ModSettingsButton.localScale = Vector3.one;
                m_ModSettingsButton.localPosition = new Vector3(39f, m_SettingsButton.localPosition.y, 0f);

                LocalizedTextField l = m_ModSettingsButton.GetComponentInChildren<LocalizedTextField>();
                if (l == null)
                {
                    SuccessfullyPatched = false;
                    return;
                }

                Text text = l.GetComponent<Text>();
                if(text == null)
                {
                    SuccessfullyPatched = false;
                    return;
                }
                text.text = "Overhaul";
                UnityEngine.Object.Destroy(l);

                UnityEngine.UI.Button b = m_ModSettingsButton.GetComponent<UnityEngine.UI.Button>();
                if (b == null)
                {
                    SuccessfullyPatched = false;
                    return;
                }

                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(showParametersMenu);

                SuccessfullyPatched = true;
            }, 0.5f);
        }

        public override void Cancel()
        {
            base.Cancel();
            if (m_IsWaitingToPatchButtons)
            {
                return;
            }
            if (SuccessfullyPatched)
            {
                m_BG.localPosition = m_OgPosition;
                m_BG.sizeDelta = m_OgSizeDelta;
                m_SettingsButton.sizeDelta = m_SettingsButtonOgSizeDelta;
                m_SettingsButton.localPosition = new Vector3(0, m_SettingsButton.localPosition.y, 0f);
                m_BackToLvlEditorButton.localPosition = m_LvlEditorButtonOgPosition;
                UnityEngine.Object.Destroy(m_ModSettingsButton.gameObject);
            }
        }

        private void showParametersMenu()
        {
            OverhaulParametersMenu menu = OverhaulController.GetController<OverhaulParametersMenu>();
            if (menu != null)
            {
                OverhaulParametersMenu.ShouldSelectShortcuts = true;
                menu.Show();
            }
        }
    }
}
