using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class ChallengesUI : OverhaulGamemodeUIBase
    {
        private bool m_HasPopulatedChallenges;

        private OverhaulUI.PrefabAndContainer ChallengesContainer;
        private OverhaulUI.PrefabAndContainer CoopChallengesContainer;

        private Text m_ChallengeTitleLabel;
        private Text m_ChallengeCompletionLabel;

        private ScrollRect m_ScrollRect;
        private Transform m_SinglePlayerChallenges;
        private Transform m_MultiPlayerChallenges;

        private Button m_JoinButton;

        private bool m_ViewCoopChallenges;
        public bool ViewCoopChallenges
        {
            get => m_ViewCoopChallenges;
            set
            {
                m_ViewCoopChallenges = value;
                if (value)
                {
                    m_SinglePlayerChallenges.gameObject.SetActive(false);
                    m_MultiPlayerChallenges.gameObject.SetActive(true);
                    m_ScrollRect.content = m_MultiPlayerChallenges as RectTransform;
                }
                else
                {
                    m_SinglePlayerChallenges.gameObject.SetActive(true);
                    m_MultiPlayerChallenges.gameObject.SetActive(false);
                    m_ScrollRect.content = m_SinglePlayerChallenges as RectTransform;
                }
            }
        }

        protected override void OnInitialize()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_ChallengeTitleLabel = moddedObject.GetObject<Text>(3);
            m_ChallengeCompletionLabel = moddedObject.GetObject<Text>(4);
            m_ScrollRect = moddedObject.GetObject<ScrollRect>(5);
            m_SinglePlayerChallenges = moddedObject.GetObject<Transform>(6);
            m_MultiPlayerChallenges = moddedObject.GetObject<Transform>(7);
            m_JoinButton = moddedObject.GetObject<Button>(8);
            m_JoinButton.AddOnClickListener(OnJoinButtonClicked);
            ChallengesContainer = new OverhaulUI.PrefabAndContainer(moddedObject, 0, 6);
            CoopChallengesContainer = new OverhaulUI.PrefabAndContainer(moddedObject, 0, 7);
            moddedObject.GetObject<Button>(2).onClick.AddListener(goBackToGamemodeSelection);
            ShowChallengeTooltip(null);
        }

        protected override void OnShow()
        {
            GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/ChallengesBG_" + UnityEngine.Random.Range(1, 5) + ".jpg");
            populateChallengesIfNeeded();

            m_JoinButton.gameObject.SetActive(ViewCoopChallenges);
            if (ViewCoopChallenges)
            {
                GameUIRoot.Instance.TitleScreenUI.SetMultiplayerPlayerModeSelectButtonsVisibile(false);
                return;
            }
            GameUIRoot.Instance.TitleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(false);
        }

        public override void Update()
        {
            base.Update();

            if (GamemodesUI.FullscreenWindow.IsActive)
                return;

            if (Input.GetKeyDown(KeyCode.Backspace))
                goBackToGamemodeSelection();
        }

        private void goBackToGamemodeSelection()
        {
            Hide();

            if (ViewCoopChallenges)
            {
                GameUIRoot.Instance.TitleScreenUI.SetMultiplayerPlayerModeSelectButtonsVisibile(true);
                return;
            }
            GameUIRoot.Instance.TitleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(true);
        }

        private void populateChallengesIfNeeded()
        {
            if (m_HasPopulatedChallenges)
                return;

            m_HasPopulatedChallenges = true;

            ChallengesContainer.ClearContainer();
            CoopChallengesContainer.ClearContainer();
            ChallengeDefinition[] allSoloChallenges = ChallengeManager.Instance.GetChallenges(false);
            foreach (ChallengeDefinition definition in allSoloChallenges)
            {
                ModdedObject moddedObject = ChallengesContainer.CreateNew();
                _ = moddedObject.gameObject.AddComponent<UIChallengeEntry>().Initialize(definition, moddedObject, this, false);
            }

            ChallengeDefinition[] allCoopChallenges = ChallengeManager.Instance.GetChallenges(true);
            foreach (ChallengeDefinition definition in allCoopChallenges)
            {
                ModdedObject moddedObject = CoopChallengesContainer.CreateNew();
                _ = moddedObject.gameObject.AddComponent<UIChallengeEntry>().Initialize(definition, moddedObject, this, true);
            }
        }

        public void ShowChallengeTooltip(ChallengeDefinition definition)
        {
            bool isCoop = ViewCoopChallenges;
            if (isCoop || definition == null)
            {
                m_ChallengeTitleLabel.text = isCoop ? string.Empty : OverhaulLocalizationManager.GetTranslation("Hover mouse over challenge");
                m_ChallengeCompletionLabel.text = string.Empty;
                return;
            }

            int allLevels = definition.GetNumberOfTotalLevels();
            int beatenLevels = definition.GetNumberOfBeatenLevels();

            m_ChallengeTitleLabel.text = LocalizationManager.Instance.GetTranslatedString(definition.ChallengeName, -1);
            m_ChallengeCompletionLabel.text = allLevels == int.MaxValue ? beatenLevels + " " + OverhaulLocalizationManager.GetTranslation("Completed of") : beatenLevels + "/" + allLevels + " " + OverhaulLocalizationManager.GetTranslation("Completed of");
        }

        public void OnJoinButtonClicked()
        {
            GamemodesUI.FullscreenWindow.Show(null, 9);
        }

        public class UIChallengeEntry : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            private ChallengesUI m_ChallengesUI;
            private ChallengeDefinition m_ChallengeDefinition;

            private GameObject[] m_ShowWhenCompleted;

            private Button m_ButtonComponent;
            private Text m_Title;
            private Text m_Description;
            private Image m_Preview;

            private bool m_IsHighlighted;

            public bool IsCoop;

            public UIChallengeEntry Initialize(ChallengeDefinition definition, ModdedObject moddedObject, ChallengesUI challengesUI, bool isCoop)
            {
                IsCoop = isCoop;
                m_ChallengesUI = challengesUI;
                m_ChallengeDefinition = definition;
                m_ShowWhenCompleted = new GameObject[2] { moddedObject.GetObject<Transform>(0).gameObject, moddedObject.GetObject<Transform>(1).gameObject };
                m_ButtonComponent = base.GetComponent<Button>();
                m_ButtonComponent.onClick.AddListener(StartChallenge);
                m_Title = moddedObject.GetObject<Text>(3);
                m_Description = moddedObject.GetObject<Text>(4);
                m_Preview = moddedObject.GetObject<Image>(2);
                RefreshUI();
                return this;
            }

            public void SetCompletedVizuals(bool value)
            {
                foreach (GameObject gameObject in m_ShowWhenCompleted)
                    gameObject.SetActive(value);
            }

            public void RefreshUI()
            {
                if (m_ChallengeDefinition == null)
                    return;

                SetCompletedVizuals(ChallengeManager.Instance.HasCompletedChallenge(m_ChallengeDefinition.ChallengeID));
                m_Title.text = LocalizationManager.Instance.GetTranslatedString(m_ChallengeDefinition.ChallengeName, -1);
                m_Description.text = LocalizationManager.Instance.GetTranslatedString(m_ChallengeDefinition.ChallengeDescription, -1);
                if (IsCoop)
                {
                    CharacterModelCustomizationEntry characterModelCustomizationEntry = getCharacterModelUnlockedByChallenge(m_ChallengeDefinition.ChallengeID);
                    if (characterModelCustomizationEntry != null)
                    {
                        m_Preview.sprite = characterModelCustomizationEntry.ImageSprite;
                        return;
                    }
                }
                m_Preview.sprite = m_ChallengeDefinition.ImageSprite;
            }

            public void StartChallenge()
            {
                if (IsCoop)
                {
                    CoopChallengePrivateMatchActions.ChallengeID = m_ChallengeDefinition.ChallengeID;
                    m_ChallengesUI.GamemodesUI.FullscreenWindow.Show(null, 10);
                    return;
                }

                void action()
                {
                    ChallengeManager.Instance.StartChallenge(m_ChallengeDefinition, false);
                    OverhaulGamemodesUI gamemodesUI = OverhaulController.Get<OverhaulGamemodesUI>();
                    if (gamemodesUI)
                        gamemodesUI.Hide();
                }
                Func<bool> func = new Func<bool>(() => CharacterTracker.Instance.GetPlayer());
                OverhaulTransitionManager.reference.DoTransition(action, func, 0.10f);
            }

            private CharacterModelCustomizationEntry getCharacterModelUnlockedByChallenge(string challengeID)
            {
                CompleteChallengeAchievement completeChallengeAchievement = GameplayAchievementManager.Instance.GetCompleteChallengeAchievement(challengeID);
                return completeChallengeAchievement != null
                    ? MultiplayerCharacterCustomizationManager.Instance.GetCharacterModelUnlockedByAchievement(completeChallengeAchievement.AchievementID)
                    : null;
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                m_IsHighlighted = false;
                m_ChallengesUI.ShowChallengeTooltip(null);
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                m_IsHighlighted = true;
                m_ChallengesUI.ShowChallengeTooltip(m_ChallengeDefinition);
            }
        }
    }
}
