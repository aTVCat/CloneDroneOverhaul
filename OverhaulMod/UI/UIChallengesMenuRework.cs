using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIChallengesMenuRework : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("OldUIButton")]
        private readonly Button m_legacyUIButton;

        [UIElementAction(nameof(OnDebugSoloChallengesButtonClicked))]
        [UIElement("DebugSoloButton")]
        private readonly Button m_debugSoloChallengesButton;
        [UIElementAction(nameof(OnDebugPublicCoopChallengesButtonClicked))]
        [UIElement("DebugPubCoopButton")]
        private readonly Button m_debugPublicCoopChallengesButton;
        [UIElementAction(nameof(OnDebugPrivateCoopChallengesButtonClicked))]
        [UIElement("DebugPrivCoopButton")]
        private readonly Button m_debugPrivateCoopChallengesButton;

        [UIElementAction(nameof(OnGetMoreChallengesButtonClicked))]
        [UIElement("WorkshopChallengesButton")]
        private readonly Button m_getMoreChallengesButton;

        [UIElement("SoloButtons")]
        private readonly GameObject m_soloButtonsContainerObject;
        [UIElement("CoopButtons")]
        private readonly GameObject m_coopButtonsContainerObject;

        [UIElement("ChallengeDisplay", false)]
        private readonly ModdedObject m_challengeDisplayPrefab;
        [UIElement("Content")]
        private readonly Transform m_challengesContainer;

        public override bool dontRefreshUI => true;

        public override void Show()
        {
            base.Show();
            ModCache.titleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(false);
        }

        public override void Hide()
        {
            base.Hide();
            ModCache.titleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(true);
        }

        public void Populate(bool isCoop, bool isPrivate)
        {
            m_coopButtonsContainerObject.SetActive(isCoop);
            m_soloButtonsContainerObject.SetActive(!isCoop);

            if (m_challengesContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_challengesContainer);

            ChallengeDefinition[] challenges = ChallengeManager.Instance.GetChallenges(isCoop);
            if (challenges.IsNullOrEmpty())
                return;

            foreach (ChallengeDefinition challengeDefinition in challenges)
            {
                bool hasCompleted = ChallengeManager.Instance.HasCompletedChallenge(challengeDefinition.ChallengeID);

                void action()
                {
                    if (isCoop)
                    {
                        if (isPrivate)
                            ChallengeManager.Instance.CreatePrivateCoopChallenge(challengeDefinition.ChallengeID);
                        else
                            ChallengeManager.Instance.JoinPublicCoopChallenge(challengeDefinition.ChallengeID);
                    }
                    else
                        ChallengeManager.Instance.StartChallenge(challengeDefinition, false);

                    Hide();
                }

                void leaderboardAction()
                {
                    string fileName = $"{GameDataManager.Instance.getChallengeHighScoreSavePath(challengeDefinition.ChallengeID)}.json";
                    List<HighScoreData> highScores = null;
                    try
                    {
                        highScores = ModDataManager.Instance.DeserializeFile<List<HighScoreData>>(fileName, false);
                    }
                    catch
                    {
                        highScores = new List<HighScoreData>();
                    }
                    ModUIConstants.ShowLeaderboard(base.transform, highScores, fileName);
                }

                CharacterModelCustomizationEntry characterModelCustomizationEntry = getCharacterModelUnlockedByChallenge(challengeDefinition.ChallengeID);
                Sprite sprite = characterModelCustomizationEntry != null ? characterModelCustomizationEntry.ImageSprite : challengeDefinition.ImageSprite;

                string rewardText = string.Empty;
                UpgradeDescription upgradeUnlockedByChallenge = UpgradeManager.Instance.GetUpgradeUnlockedByChallenge(challengeDefinition.ChallengeID);
                if (upgradeUnlockedByChallenge)
                {
                    rewardText = LocalizationManager.Instance.GetTranslatedString(upgradeUnlockedByChallenge.UpgradeName);
                }
                else
                {
                    EmoteDefinition emoteUnlockedByChallenge = getEmoteUnlockedByChallenge(challengeDefinition.ChallengeID);
                    rewardText = characterModelCustomizationEntry != null
                        ? LocalizationManager.Instance.GetTranslatedString(characterModelCustomizationEntry.Name) + " " + LocalizationManager.Instance.GetTranslatedString("skin")
                        : emoteUnlockedByChallenge != null
                            ? LocalizationManager.Instance.GetTranslatedString("Emote:") + " " + LocalizationManager.Instance.GetTranslatedString(emoteUnlockedByChallenge.EmoteID, -1)
                            : LocalizationManager.Instance.GetTranslatedString("A nice trophy!", -1);
                }

                ModdedObject moddedObject = Instantiate(m_challengeDisplayPrefab, m_challengesContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = LocalizationManager.Instance.GetTranslatedString(challengeDefinition.ChallengeName);
                moddedObject.GetObject<Text>(1).text = LocalizationManager.Instance.GetTranslatedString(challengeDefinition.ChallengeDescription);
                moddedObject.GetObject<Image>(2).sprite = sprite;
                moddedObject.GetObject<Text>(3).text = rewardText;
                moddedObject.GetObject<Button>(4).onClick.AddListener(action);
                moddedObject.GetObject<GameObject>(5).SetActive(hasCompleted);
                moddedObject.GetObject<GameObject>(6).SetActive(!hasCompleted);
                moddedObject.GetObject<Button>(7).onClick.AddListener(leaderboardAction);
                moddedObject.GetObject<Button>(7).interactable = challengeDefinition.UseEndlessLevels;
            }
        }

        private CharacterModelCustomizationEntry getCharacterModelUnlockedByChallenge(string challengeID)
        {
            CompleteChallengeAchievement completeChallengeAchievement = GameplayAchievementManager.Instance.GetCompleteChallengeAchievement(challengeID);
            return completeChallengeAchievement
                ? MultiplayerCharacterCustomizationManager.Instance.GetCharacterModelUnlockedByAchievement(completeChallengeAchievement.AchievementID)
                : null;
        }

        private EmoteDefinition getEmoteUnlockedByChallenge(string challengeID)
        {
            CompleteChallengeAchievement completeChallengeAchievement = GameplayAchievementManager.Instance.GetCompleteChallengeAchievement(challengeID);
            return completeChallengeAchievement
                ? EmoteManager.Instance.GetEmoteUnlockedByAchievement(completeChallengeAchievement.AchievementID)
                : null;
        }

        public void OnLegacyUIButtonClicked()
        {
            TitleScreenUI titleScreenUI = ModCache.titleScreenUI;
            if (titleScreenUI)
            {
                Hide();
                titleScreenUI.OnChallengeButtonClicked();
            }
        }

        public void OnGetMoreChallengesButtonClicked()
        {
            Hide();

            TitleScreenUI titleScreenUI = ModCache.titleScreenUI;
            if (!titleScreenUI)
                return;

            WorkshopBrowserUI workshopBrowserUI = titleScreenUI.WorkshopBrowserUI;
            if (!workshopBrowserUI)
                return;

            titleScreenUI.CoopChallengesInviteMenu.Hide();
            titleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(true);
            titleScreenUI.SingleplayerModeSelectScreen.Hide();
            titleScreenUI.SetMultiplayerPlayerModeSelectButtonsVisibile(true);
            titleScreenUI.MultiplayerModeSelectScreen.Hide();
            titleScreenUI.setLogoAndRootButtonsVisible(true);

            workshopBrowserUI.Show();
        }

        public void OnDebugSoloChallengesButtonClicked()
        {
            Populate(false, false);
        }

        public void OnDebugPublicCoopChallengesButtonClicked()
        {
            Populate(true, false);
        }

        public void OnDebugPrivateCoopChallengesButtonClicked()
        {
            Populate(true, true);
        }
    }
}
