using CDOverhaul.HUD;
using CDOverhaul.Workshop;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class TitleScreenUIReplacement : ReplacementBase
    {
        private Transform m_ButtonsTransform;
        private Transform m_SpawnedPanel;

        private RectTransform m_SingleplayerButtonTransform;
        private RectTransform m_MultiplayerNEWButtonTransform;

        private Text m_SettingsText;
        private Text m_BugReportText;
        private Text m_AboutOverhaulText;

        private Vector3 m_OriginalAchievementsNewHintPosition;
        private Transform m_AchievementsNewHint;

        private Transform m_MessagePanel;
        public TitleScreenMessagePanel MessagePanelComponent;

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
            if (m_MultiplayerNEWButtonTransform != null)
            {
                m_MultiplayerNEWButtonTransform.localPosition = new Vector3(45, -62.5f, 0);
                m_MultiplayerNEWButtonTransform.sizeDelta = new Vector2(85f, 27.5f);
            }

            m_SingleplayerButtonTransform = TransformUtils.FindChildRecursive(target.transform, "PlaySingleplayer") as RectTransform;
            if (m_SingleplayerButtonTransform != null)
            {
                m_SingleplayerButtonTransform.localPosition = new Vector3(0, -30f, 0);
                m_SingleplayerButtonTransform.sizeDelta = new Vector2(175f, 27.5f);

                RectTransform playModdedButton = Object.Instantiate(m_SingleplayerButtonTransform, m_SingleplayerButtonTransform.parent);
                playModdedButton.localPosition = new Vector3(-45, -62.5f, 0);
                playModdedButton.sizeDelta = new Vector2(85f, 27.5f);
                LocalizedTextField playModdedButtonText = playModdedButton.GetComponentInChildren<LocalizedTextField>();
                Text playModdedButtonTextComponent = playModdedButtonText.GetComponent<Text>();
                playModdedButtonTextComponent.text = OverhaulLocalizationManager.LocalizationData.GetTranslation("Play modded");
                Object.Destroy(playModdedButtonText);

                Transform gamemodeSelectionScreenPrefab = TransformUtils.FindChildRecursive(target.transform, "SingleplayerModeSelectScreen");
                Transform moddedGamemodesSelectionScreenTransform = Object.Instantiate(gamemodeSelectionScreenPrefab, gamemodeSelectionScreenPrefab.parent);
                moddedGamemodesSelectionScreenTransform.gameObject.SetActive(false);
                GameModeSelectScreen moddedGameModesSelectScreen = moddedGamemodesSelectionScreenTransform.GetComponent<GameModeSelectScreen>();
                moddedGameModesSelectScreen.GameModeData = new GameModeCardData[]
                {
                    new GameModeCardData
                    {
                        NameOfMode = OverhaulLocalizationManager.LocalizationData.GetTranslation("Multiplayer Sandbox"),
                        Description = OverhaulLocalizationManager.LocalizationData.GetTranslation("Multiplayer Sandbox Desc"),
                        ClickedCallback = new UnityEngine.Events.UnityEvent()
                    }/*,
                    new GameModeCardData
                    {
                        NameOfMode = "Testing mode",
                        Description = "",
                        ClickedCallback = new UnityEngine.Events.UnityEvent()
                    }*/
                };
                moddedGameModesSelectScreen.GameModeData[0].ClickedCallback.AddListener(OverhaulFullscreenDialogueWindow.ShowUnfinishedFeatureWindow);
                //moddedGameModesSelectScreen.GameModeData[1].ClickedCallback.AddListener(OverhaulController.GetController<OvermodesController>().StartTestMode);
                patchGameModeSelectScreen(moddedGamemodesSelectionScreenTransform);
                Transform exitButtonTransform = moddedGamemodesSelectionScreenTransform.FindChildRecursive("UIPrefab_CloseButton");
                if (!exitButtonTransform)
                {
                    exitButtonTransform = moddedGamemodesSelectionScreenTransform.FindChildRecursive("exitButton (1)");
                }
                Button exitButton = exitButtonTransform.GetComponent<Button>();
                exitButton.onClick = new Button.ButtonClickedEvent();
                exitButton.onClick.AddListener(delegate
                {
                    target.SetLogoAndRootButtonsVisible(true);
                    moddedGameModesSelectScreen.Hide();
                });

                Button playModdedButtonComponent = playModdedButton.GetComponent<Button>();
                playModdedButtonComponent.onClick = new Button.ButtonClickedEvent();
                playModdedButtonComponent.onClick.AddListener(delegate
                {
                    target.SetLogoAndRootButtonsVisible(false);
                    moddedGameModesSelectScreen.Show();
                });
            }

            m_AchievementsNewHint = TransformUtils.FindChildRecursive(target.transform, "AchievementsNewHint");
            if (m_AchievementsNewHint == null)
            {
                SuccessfullyPatched = false;
                return;
            }

            Transform quitButton = TransformUtils.FindChildRecursive(target.transform, "QuitButton");
            if (quitButton)
            {
                Button button = quitButton.GetComponent<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(delegate
                {
                    OverhaulSurveyUI ui = OverhaulController.Get<OverhaulSurveyUI>();
                    if (ui)
                        ui.Show();
                    else
                        Application.Quit();
                });
            }

            Transform lvlEditorButton = TransformUtils.FindChildRecursive(target.transform, "LevelEditorButton");
            if (lvlEditorButton)
            {
                Button button = lvlEditorButton.GetComponent<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(delegate
                {
                    OverhaulTransitionManager.reference.DoTransition(target.OnLevelEditorButtonClicked, null, 1f);
                });
            }

            Transform workshopButton = TransformUtils.FindChildRecursive(target.transform, "WorkshopButton");
            if (workshopButton)
            {
                BaseFixes.ChangeButtonAction(workshopButton, "WorkshopButton", delegate
                {
                    bool canShowNewBrowser = OverhaulWorkshopBrowserUI.UseThisUI && !OverhaulWorkshopBrowserUI.BrowserIsNull;
                    if (canShowNewBrowser)
                    {
                        OverhaulWorkshopBrowserUI.Instance.Show();
                    }
                    else
                    {
                        GameUIRoot.Instance.TitleScreenUI.OnWorkshopBrowserButtonClicked();
                    }
                });
            }

            RectTransform multiplayerErrorGroup = target.transform.FindRectChildRecursive("MultiplayerErrorGroup");
            if (multiplayerErrorGroup)
            {
                multiplayerErrorGroup.anchoredPosition = new Vector2(10f, 90f);
                RectTransform arrow = multiplayerErrorGroup.FindRectChildRecursive("Arrow");
                if (arrow)
                {
                    arrow.anchoredPosition = new Vector2(-35f, 0f);
                }
            }

            m_MessagePanel = TransformUtils.FindChildRecursive(target.transform, "TitleScreenMessagePanel");
            if (m_MessagePanel == null)
            {
                SuccessfullyPatched = false;
                return;
            }
            MessagePanelComponent = m_MessagePanel.GetComponent<TitleScreenMessagePanel>();

            Transform singlePlayerGameModeSelectionMenu = TransformUtils.FindChildRecursive(target.transform, "SingleplayerModeSelectScreen");
            if (singlePlayerGameModeSelectionMenu != null)
            {
                patchGameModeSelectScreen(singlePlayerGameModeSelectionMenu);
            }

            Transform multiPlayerGameModeSelectionMenu = TransformUtils.FindChildRecursive(target.transform, "MultiplayerModeSelectScreen");
            if (multiPlayerGameModeSelectionMenu != null)
            {
                patchGameModeSelectScreen(multiPlayerGameModeSelectionMenu);
            }

            m_OriginalAchievementsNewHintPosition = m_AchievementsNewHint.localPosition;
            m_AchievementsNewHint.localPosition = new Vector3(70f, -162.5f, 0f);

            m_SpawnedPanel = OverhaulUIManager.reference?.uiPrefabs?.InstantiatePrefab("UIPrefab_TitleScreenUIButtons", m_ButtonsTransform, true).transform;
            m_SpawnedPanel.SetAsFirstSibling();

            ModdedObject moddedObject = m_SpawnedPanel.GetComponent<ModdedObject>();
            moddedObject.GetObject<Button>(0).onClick.AddListener(UIConstants.ShowSettingsMenu);
            m_SettingsText = moddedObject.GetObject<Text>(1);
            //moddedObject.GetObject<Button>(2).onClick.AddListener(OverhaulController.Get<OverhaulSurveyUI>().Show);
            m_BugReportText = moddedObject.GetObject<Text>(3);
            //moddedObject.GetObject<Button>(4).onClick.AddListener(OverhaulController.Get<AboutOverhaulMenu>().Show);
            m_AboutOverhaulText = moddedObject.GetObject<Text>(5);
            moddedObject.GetObject<Transform>(6).gameObject.SetActive(OverhaulVersion.IsDebugBuild);
            //moddedObject.GetObject<Button>(6).onClick.AddListener(OverhaulController.Get<OverhaulLocalizationEditor>().Show);

            m_ButtonsTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            m_ButtonsTransform.localPosition = new Vector3(0, -135f, 0);

            OverhaulEvents.AddEventListener(GlobalEvents.UILanguageChanged, localizeTexts, true);
            localizeTexts();
            SuccessfullyPatched = true;
        }

        private void patchGameModeSelectScreen(Transform main)
        {
            GameModeSelectScreen gameModeSelectScreen = main.GetComponent<GameModeSelectScreen>();
            _ = gameModeSelectScreen.gameObject.AddComponent<GameModeSelectPanelBehaviourFix>();
            BaseFixes.UpdateSprites(gameModeSelectScreen.ButtonPrefab.transform);

            RectTransform rectTransformOfMain = main as RectTransform;
            rectTransformOfMain.localScale = Vector3.one;

            RectTransform box = main.FindChildRecursive("Box") as RectTransform;
            box.anchorMax = new Vector2(1f, 0.5f);
            box.anchorMin = new Vector2(0f, 0.5f);
            box.anchoredPosition = Vector2.zero;
            box.sizeDelta = new Vector2(0f, 300f);
            box.gameObject.SetActive(true);
            OverhaulUIPanelSlider slider1 = box.gameObject.AddComponent<OverhaulUIPanelSlider>();
            slider1.TargetPosition = new Vector3(0f, 0, 0f);
            slider1.StartPosition = new Vector3(1000f, 0, 0f);
            slider1.Multiplier = 15f;
            slider1.StopForFrames = 3;

            Transform cardContainer = main.FindChildRecursive("CardContainer");
            cardContainer.localScale = Vector3.one * 1.1f;
            HorizontalLayoutGroup horizontalLayoutGroup = cardContainer.GetComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.spacing = 17f;

            OverhaulUIPanelSlider slider = cardContainer.gameObject.AddComponent<OverhaulUIPanelSlider>();
            slider.TargetPosition = new Vector3(0f, -13f, 0f);
            slider.StartPosition = new Vector3(200f, -13f, 0f);
            slider.Multiplier = 9f;
            slider.StopForFrames = 3;

            Transform bg = main.FindChildRecursive("BG");
            Image bgImage = bg.GetComponent<Image>();
            bgImage.color = new Color(0.07f, 0.07f, 0.07f, 0.5f);

            // Card
            RectTransform cardTransform = gameModeSelectScreen.ButtonPrefab.transform as RectTransform;
            cardTransform.sizeDelta = new Vector2(155f, 200f);
            RectTransform cardBG = cardTransform.FindChildRecursive("BG") as RectTransform;
            cardBG.anchoredPosition = Vector2.zero;
            cardBG.sizeDelta = Vector2.zero;
            cardBG.GetComponent<Image>().color = new Color(0.5f, 0.7f, 1f, 0.3f);
            UIColorSwapper cardBGColors = cardBG.GetComponent<UIColorSwapper>();
            cardBGColors.ColorVariants[0] = new Color(0.5f, 0.7f, 1f, 0.3f);
            cardBGColors.ColorVariants[1] = new Color(0f, 1f, 0.25f, 0.65f);
            RectTransform cardImage = cardBG.FindChildRecursive("GameModeImage") as RectTransform;
            cardImage.sizeDelta = new Vector2(170f, 200f);

            Transform cardButtonBG = cardTransform.FindChildRecursive("buttonBG");
            cardButtonBG.gameObject.SetActive(false);

            Transform cardGamemodeButton = cardTransform.FindChildRecursive("GameModeButton");
            cardGamemodeButton.GetComponent<Image>().enabled = false;
            cardGamemodeButton.FindChildRecursive("Image (2)").gameObject.SetActive(false);

            Transform cardHeadingLabel = cardGamemodeButton.FindChildRecursive("headingLabel");
            if (!cardHeadingLabel.GetComponent<Outline>())
            {
                _ = cardHeadingLabel.gameObject.AddComponent<Outline>();
                cardHeadingLabel.gameObject.AddComponent<Shadow>().effectDistance = Vector2.one * -1.5f;
            }
            cardHeadingLabel.GetComponent<Text>().color = Color.white;
            cardHeadingLabel.localPosition = new Vector3(0, 5f);
            UIColorSwapper cardGamemodeButtonColors = cardHeadingLabel.GetComponent<UIColorSwapper>();
            cardGamemodeButtonColors.ColorVariants[0] = Color.white;
            cardGamemodeButtonColors.ColorVariants[1] = new Color(0.3f, 1f, 0.35f, 1f);

            OverhaulUIPrefabs uiPrefabs = OverhaulUIManager.reference?.uiPrefabs;
            if (uiPrefabs)
            {
                Transform ogCloseButton = gameModeSelectScreen.transform.FindChildRecursive("exitButton (1)");
                if (ogCloseButton)
                {
                    ogCloseButton.gameObject.SetActive(false);
                    Button newCloseButton = uiPrefabs.InstantiatePrefab("UIPrefab_CloseButton", ogCloseButton.parent, true).GetComponent<Button>();
                    newCloseButton.transform.localPosition = new Vector3(350f, 125f, 0f);
                    newCloseButton.transform.localEulerAngles = Vector3.zero;
                    newCloseButton.transform.localScale = Vector3.one;
                    newCloseButton.onClick = ogCloseButton.GetComponent<Button>().onClick;
                    newCloseButton.gameObject.name = "GenericCloseButton";
                }
            }
        }

        private void localizeTexts()
        {
            if (!SuccessfullyPatched || OverhaulLocalizationManager.Error)
            {
                return;
            }

            m_BugReportText.text = OverhaulLocalizationManager.LocalizationData.GetTranslation("TitleScreen_BugReport");
            m_SettingsText.text = OverhaulLocalizationManager.LocalizationData.GetTranslation("TitleScreen_Settings");
            m_AboutOverhaulText.text = OverhaulLocalizationManager.LocalizationData.GetTranslation("About Overhaul");
        }

        public override void Cancel()
        {
            base.Cancel();
            if (SuccessfullyPatched)
            {
                OverhaulEvents.RemoveEventListener(GlobalEvents.UILanguageChanged, localizeTexts, true);
                m_ButtonsTransform.localScale = Vector3.one;
                m_ButtonsTransform.localPosition = new Vector3(0, -195.5f, 0);
                m_MultiplayerNEWButtonTransform.localPosition = new Vector3(0, -87.8241f, 0);
                m_AchievementsNewHint.localPosition = m_OriginalAchievementsNewHintPosition;

                if (m_SpawnedPanel != null)
                {
                    GameObject.Destroy(m_SpawnedPanel.gameObject);
                }
            }
        }
    }
}
