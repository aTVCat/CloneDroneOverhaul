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

        public override void Replace()
        {
            base.Replace();
            TitleScreenUI target = GameUIRoot.Instance?.TitleScreenUI;
            if (!target)
                return;

            /*
            m_SingleplayerButtonTransform = TransformUtils.FindChildRecursive(target.transform, "PlaySingleplayer") as RectTransform;
            if (m_SingleplayerButtonTransform)
            {
                RectTransform playModdedButton = Object.Instantiate(m_SingleplayerButtonTransform, m_SingleplayerButtonTransform.parent);
                if (playModdedButton)
                {
                    m_MultiplayerNEWButtonTransform = TransformUtils.FindChildRecursive(target.transform, "MultiplayerButton_NEW") as RectTransform;
                    if (m_MultiplayerNEWButtonTransform)
                    {
                        m_MultiplayerNEWButtonTransform.localPosition = new Vector3(45, -62.5f, 0);
                        m_MultiplayerNEWButtonTransform.sizeDelta = new Vector2(85f, 27.5f);
                    }

                    m_SingleplayerButtonTransform.localPosition = new Vector3(0, -30f, 0);
                    m_SingleplayerButtonTransform.sizeDelta = new Vector2(175f, 27.5f);

                    playModdedButton.localPosition = new Vector3(-45, -62.5f, 0);
                    playModdedButton.sizeDelta = new Vector2(85f, 27.5f);
                    LocalizedTextField playModdedButtonText = playModdedButton.GetComponentInChildren<LocalizedTextField>();
                    if (playModdedButtonText)
                    {
                        Text playModdedButtonTextComponent = playModdedButtonText.GetComponent<Text>();
                        playModdedButtonTextComponent.text = OverhaulLocalizationController.Localization.GetTranslation("Play modded");
                        Object.Destroy(playModdedButtonText);
                    }

                    Transform gamemodeSelectionScreenPrefab = TransformUtils.FindChildRecursive(target.transform, "SingleplayerModeSelectScreen");
                    Transform moddedGamemodesSelectionScreenTransform = Object.Instantiate(gamemodeSelectionScreenPrefab, gamemodeSelectionScreenPrefab.parent);
                    moddedGamemodesSelectionScreenTransform.gameObject.SetActive(false);
                    GameModeSelectScreen moddedGameModesSelectScreen = moddedGamemodesSelectionScreenTransform.GetComponent<GameModeSelectScreen>();
                    moddedGameModesSelectScreen.GameModeData = new GameModeCardData[]
                    {
                    new GameModeCardData
                    {
                        NameOfMode = "Multiplayer Sandbox",
                        Description = "Coming in update 5 or later...",
                        ClickedCallback = new UnityEngine.Events.UnityEvent()
                    }
                    };
                    moddedGameModesSelectScreen.GameModeData[0].ClickedCallback.AddListener(OverhaulFullscreenDialogueWindow.ShowUnfinishedFeatureWindow);
                    patchGameModeSelectScreen(moddedGamemodesSelectionScreenTransform);

                    Button exitButton = moddedGamemodesSelectionScreenTransform.FindChildRecursive("GenericCloseButton").GetComponent<Button>();
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
            }*/

            m_AchievementsNewHint = TransformUtils.FindChildRecursive(target.transform, "AchievementsNewHint");
            if (m_AchievementsNewHint)
            {
                m_OriginalAchievementsNewHintPosition = m_AchievementsNewHint.localPosition;
                m_AchievementsNewHint.localPosition = new Vector3(70f, -182.5f, 0f);
            }

            Transform quitButton = TransformUtils.FindChildRecursive(target.transform, "QuitButton");
            if (quitButton)
            {
                OverhaulSurveyUI ui = OverhaulController.GetController<OverhaulSurveyUI>();
                if (!ui)
                    return;

                Button button = quitButton.GetComponent<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(ui.Show);
            }

            Transform lvlEditorButton = TransformUtils.FindChildRecursive(target.transform, "LevelEditorButton");
            if (lvlEditorButton && OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsNewTransitionScreenEnabled)
            {
                Button button = lvlEditorButton.GetComponent<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(delegate
                {
                    OverhaulTransitionController.DoTransitionWithAction(target.OnLevelEditorButtonClicked, null, 1f);
                });
            }

            Transform workshopButton = TransformUtils.FindChildRecursive(target.transform, "WorkshopButton");
            if (workshopButton)
            {
                Button button = workshopButton.GetComponent<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(delegate
                {
                    bool canShowNewBrowser = OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsNewWorkshopBrowserEnabled && OverhaulWorkshopBrowserUI.UseThisUI && !OverhaulWorkshopBrowserUI.BrowserIsNull;
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
                multiplayerErrorGroup.anchoredPosition = new Vector2(10f, 65f);
                RectTransform arrow = multiplayerErrorGroup.FindRectChildRecursive("Arrow");
                if (arrow)
                {
                    arrow.anchoredPosition = new Vector2(-37.5f, 0f);
                }
            }

            Transform singlePlayerGameModeSelectionMenu = TransformUtils.FindChildRecursive(target.transform, "SingleplayerModeSelectScreen");
            if (singlePlayerGameModeSelectionMenu)
            {
                patchGameModeSelectScreen(singlePlayerGameModeSelectionMenu);
            }

            Transform multiPlayerGameModeSelectionMenu = TransformUtils.FindChildRecursive(target.transform, "MultiplayerModeSelectScreen");
            if (multiPlayerGameModeSelectionMenu)
            {
                patchGameModeSelectScreen(multiPlayerGameModeSelectionMenu);
            }

            m_ButtonsTransform = TransformUtils.FindChildRecursive(target.transform, "BottomButtons");
            if (m_ButtonsTransform)
            {
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
                moddedObject.GetObject<Button>(0).onClick.AddListener(OverhaulController.GetController<ParametersMenu>().Show);
                m_SettingsText = moddedObject.GetObject<Text>(1);
                moddedObject.GetObject<Button>(2).onClick.AddListener(OverhaulController.GetController<OverhaulSurveyUI>().Show);
                m_BugReportText = moddedObject.GetObject<Text>(3);
                moddedObject.GetObject<Button>(4).onClick.AddListener(OverhaulController.GetController<AboutOverhaulMenu>().Show);
                m_AboutOverhaulText = moddedObject.GetObject<Text>(5);
                moddedObject.GetObject<Transform>(6).gameObject.SetActive(OverhaulVersion.IsDebugBuild);
                moddedObject.GetObject<Button>(6).onClick.AddListener(OverhaulController.GetController<OverhaulLocalizationEditor>().Show);

                m_ButtonsTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                m_ButtonsTransform.localPosition = new Vector3(0, -160f, 0);
            }

            _ = OverhaulEventsController.AddEventListener(GlobalEvents.UILanguageChanged, localizeTexts, true);
            localizeTexts();
            SuccessfullyPatched = true;
        }

        private void patchGameModeSelectScreen(Transform main)
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsGameModeSelectScreenRedesignEnabled)
                return;

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

            OverhaulCanvasController canvasController = OverhaulMod.Core.CanvasController;
            if (canvasController)
            {
                Transform ogCloseButton = gameModeSelectScreen.transform.FindChildRecursive("exitButton (1)");
                if (ogCloseButton)
                {
                    ogCloseButton.gameObject.SetActive(false);
                    Button newCloseButton = UnityEngine.Object.Instantiate(canvasController.GetHUDPrefab("GenericCloseButton"), ogCloseButton.parent).GetComponent<Button>();
                    newCloseButton.transform.localPosition = new Vector3(350f, 125f, 0f);
                    newCloseButton.transform.localEulerAngles = Vector3.zero;
                    newCloseButton.transform.localScale = Vector3.one;
                    newCloseButton.onClick = ogCloseButton.GetComponent<Button>().onClick;
                    newCloseButton.gameObject.name = "GenericCloseButton";
                    newCloseButton.gameObject.SetActive(true);
                }
            }
        }

        private void localizeTexts()
        {
            if (OverhaulLocalizationController.Error)
                return;

            if (m_BugReportText)
                m_BugReportText.text = OverhaulLocalizationController.Localization.GetTranslation("TitleScreen_BugReport");
            if (m_SettingsText)
                m_SettingsText.text = OverhaulLocalizationController.Localization.GetTranslation("TitleScreen_Settings");
            if(m_AboutOverhaulText)
                m_AboutOverhaulText.text = OverhaulLocalizationController.Localization.GetTranslation("About Overhaul");
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
                m_AchievementsNewHint.localPosition = m_OriginalAchievementsNewHintPosition;

                if (m_SpawnedPanel != null)
                {
                    GameObject.Destroy(m_SpawnedPanel.gameObject);
                }
            }
        }
    }
}
