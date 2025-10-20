using InternalModBot;
using OverhaulMod.Content;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using Steamworks;
using System.Collections;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UITitleScreenHypocrisisSkin : OverhaulUIBehaviour
    {
        public static readonly PublishedFileId_t LEVEL_STEAM_ID = new PublishedFileId_t(3557418313);

        public static readonly PublishedFileId_t MAIN_MENU_LEVEL_STEAM_ID = new PublishedFileId_t(3522164720);

        public const string LEVEL_AUTHOR_STEAM_PROFILE_PAGE = "https://steamcommunity.com/profiles/76561198886409131";

        public const string LEVEL_AUTHOR_DISCORD_SERVER = "https://discord.gg/79r26pu7MC";

        [UIElementAction(nameof(OnStartButtonClicked))]
        [UIElement("StartButton")]
        private readonly Button m_startButton;

        [UIElementAction(nameof(OnSettingsButtonClicked))]
        [UIElement("SettingsButton")]
        private readonly Button m_settingsButton;

        [UIElementAction(nameof(OnDiscordButtonClicked))]
        [UIElement("DiscordButton")]
        private readonly Button m_discordButton;

        [UIElementAction(nameof(OnSteamButtonClicked))]
        [UIElement("SteamButton")]
        private readonly Button m_steamButton;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;

        [UIElement("Holder", true)]
        private readonly GameObject m_holder;

        [UIElement("ProgressBarFill")]
        private readonly Image m_progressBarFill;

        [UIElement("DisabledModsPanel", false)]
        private readonly GameObject m_disabledModsPanel;

        [UIElement("Description")]
        private readonly Text m_disabledModsPanelText;

        [UIElementAction(nameof(OnModsButtonClicked))]
        [UIElement("ModsButton")]
        private readonly Button m_modsButton;

        [UIElementAction(nameof(OnCloseButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button m_closeButton;

        [UIElementAction(nameof(OnModsButtonClicked))]
        [UIElement("ModsButton2", false)]
        private readonly Button m_modsButton2;

        [UIElementAction(nameof(OnRestartButtonClicked))]
        [UIElement("RestartButton")]
        private readonly Button m_restartButton;

        public override bool closeOnEscapeButtonPress => false;

        private bool m_getCallbacks;

        private bool m_startChallengeWhenReady;

        private float m_timeLeftToUpdate;

        private bool m_ignoreMinorErrors;

        private WorkshopItem m_levelWorkshopItem;

        private bool m_prevHolderState;

        public static bool HideVersionLabel;

        protected override void OnInitialized()
        {
            m_startButton.interactable = true;

            ModActionUtils.DoInFrames(doChecks, 60);
        }

        public override void Start()
        {
            hideTitleScreenElements();
        }

        public override void OnEnable()
        {
            hideTitleScreenElements();
            HideVersionLabel = true;
        }

        public override void OnDisable()
        {
            showTitleScreenElements();
            HideVersionLabel = false;
        }

        private void showTitleScreenElements()
        {
            if (ModCache.titleScreenUI.gameObject.activeSelf)
            {
                ArenaCameraManager.Instance.SetTitleScreenLogoVisible(true);
                ArenaCameraManager.Instance.TitleScreenLogoCamera.gameObject.SetActive(true);

                Transform leftFade = TransformUtils.FindChildRecursive(ModCache.titleScreenUI.transform, "LeftFadeBG");
                if (leftFade)
                {
                    leftFade.gameObject.SetActive(true);
                }

                ModCache.titleScreenUI.SocialButtonPanel.gameObject.SetActive(true);
            }
        }

        private void hideTitleScreenElements()
        {
            ArenaCameraManager.Instance.SetTitleScreenLogoVisible(false);
            ArenaCameraManager.Instance.TitleScreenLogoCamera.gameObject.SetActive(false);

            Transform leftFade = TransformUtils.FindChildRecursive(ModCache.titleScreenUI.transform, "LeftFadeBG");
            if (leftFade)
            {
                leftFade.gameObject.SetActive(false);
            }

            ModCache.titleScreenUI.SocialButtonPanel.gameObject.SetActive(false);
        }

        public override void Update()
        {
            bool state = ModCache.titleScreenRootButtonsBG.activeInHierarchy;
            if (m_prevHolderState != state)
            {
                m_prevHolderState = state;

                m_holder.SetActive(state);

                if (state)
                {
                    hideTitleScreenElements();
                }
            }

            m_progressBarFill.fillAmount = ModSteamUGCUtils.GetItemDownloadProgress(LEVEL_STEAM_ID);

            m_timeLeftToUpdate = Mathf.Max(0f, m_timeLeftToUpdate - Time.unscaledDeltaTime);
            if (m_timeLeftToUpdate == 0f)
            {
                m_timeLeftToUpdate = 1f;

                if (m_startChallengeWhenReady && isReadyToStartChallenge())
                {
                    m_startChallengeWhenReady = false;
                    startChallenge();
                }
            }
        }

        private void doChecks()
        {
            waitUntilSteamInitializedThenSwapMenuLevel();
            checkMods();
        }

        private void waitUntilSteamInitializedThenSwapMenuLevel()
        {
            waitUntilSteamInitializedThenSwapMenuLevelCoroutine().Run();
        }

        private IEnumerator waitUntilSteamInitializedThenSwapMenuLevelCoroutine()
        {
            WorkshopLevelManager workshopLevelManager = WorkshopLevelManager.Instance;
            SteamManager steamManager = SteamManager.Instance;
            if (!steamManager || !workshopLevelManager)
                yield break;

            float timeOut = Time.unscaledTime + 10f;
            while ((!steamManager.Initialized || workshopLevelManager.GetAllWorkShopEndlessLevels() == null) && Time.unscaledTime < timeOut)
                yield return null;

            if (Time.unscaledTime >= timeOut)
                yield break;

            TitleScreenCustomizationManager.Instance.SetHypocrisisBackgroundLevel();

            yield break;
        }

        private void checkMods()
        {
            string[] ids = new string[]
            {
                "level-editor-custom-level-sounds",
                "11C6A601-560B-4547-9614-149970671EEB",
                "more-level-editor-objects",
                "level-editor-custom-models",
                "level-dir-api",
                "ee32ba1b-8c92-4f50-bdf4-400a14da829e",
                "de731a6b-0a96-4882-a02b-a336904f9853",
            };

            bool someModsNotEnabled = false;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < ids.Length; i++)
            {
                string id = ids[i];
                if (!ModSpecialUtils.IsModEnabled(id))
                {
                    someModsNotEnabled = true;
                    switch (i)
                    {
                        case 0:
                            stringBuilder.AppendLine("- Custom Level Sounds");
                            break;
                        case 1:
                            stringBuilder.AppendLine("- Soundpacks Mod");
                            break;
                        case 2:
                            stringBuilder.AppendLine("- Level Editor Extended");
                            break;
                        case 3:
                            stringBuilder.AppendLine("- Level Editor Custom Models");
                            break;
                        case 4:
                            stringBuilder.AppendLine("- Level Assets Directory API");
                            break;
                        case 5:
                            stringBuilder.AppendLine("- Glock-18 Mod");
                            break;
                        case 6:
                            stringBuilder.AppendLine("- Custom robot model editor");
                            break;
                        default:
                            stringBuilder.AppendLine($"- {id}");
                            break;
                    }
                }
            }

            m_modsButton2.gameObject.SetActive(!someModsNotEnabled);
            m_disabledModsPanel.SetActive(someModsNotEnabled);
            if (someModsNotEnabled)
            {
                m_disabledModsPanelText.text = stringBuilder.ToString();
            }
        }

        public void OnStartButtonClicked()
        {
            if (isReadyToStartChallenge())
            {
                startChallenge();
                return;
            }

            m_getCallbacks = true;
            m_startButton.interactable = false;
            m_loadingIndicator.SetActive(true);
            ModSteamUGCUtils.GetWorkshopItem(LEVEL_STEAM_ID, onGotItem, onError, null);
        }

        public void OnSettingsButtonClicked()
        {
            ModUIConstants.ShowSettingsMenuRework(false);
        }

        public void OnDiscordButtonClicked()
        {
            Application.OpenURL(LEVEL_AUTHOR_DISCORD_SERVER);
        }

        public void OnSteamButtonClicked()
        {
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
                SteamFriends.ActivateGameOverlayToWebPage(LEVEL_AUTHOR_STEAM_PROFILE_PAGE);
            else
                Application.OpenURL(LEVEL_AUTHOR_STEAM_PROFILE_PAGE);
        }

        public void OnCloseButtonClicked()
        {
            m_disabledModsPanel.SetActive(false);
            m_modsButton2.gameObject.SetActive(true);
        }

        public void OnModsButtonClicked()
        {
            ModsPanelManager.Instance.openModsMenu();
        }

        public void OnRestartButtonClicked()
        {
            m_restartButton.interactable = false;
            restartCoroutine().Run();
        }

        private IEnumerator restartCoroutine()
        {
            float timeOut = Time.unscaledTime + 10f;
            while (AddonManager.Instance.IsLoadingAddons() && Time.unscaledTime < timeOut)
                yield return null;

            _ = Process.Start("steam://rungameid/" + 597170U.ToString());
            Application.Quit();
            yield break;
        }

        private void onGotItem(Content.WorkshopItem workshopItem)
        {
            if (!m_getCallbacks)
                return;

            m_getCallbacks = false;
            m_startChallengeWhenReady = true;
            m_levelWorkshopItem = workshopItem;

            EItemState itemState = ModSteamUGCUtils.GetItemState(workshopItem.ItemID);
            bool installed = ModSteamUGCUtils.IsItemInstalled(workshopItem.ItemID);
            bool subscribed = itemState.HasFlag(EItemState.k_EItemStateSubscribed);
            bool downloading = itemState.HasFlag(EItemState.k_EItemStateDownloading) || itemState.HasFlag(EItemState.k_EItemStateDownloadPending);
            bool needsUpdate = itemState.HasFlag(EItemState.k_EItemStateNeedsUpdate);

            if (subscribed)
            {
                if (needsUpdate || !installed)
                {
                    _ = ModSteamUGCUtils.UpdateItem(workshopItem.ItemID, delegate (DownloadItemResult_t t)
                    {
                        if (t.m_nPublishedFileId == workshopItem.ItemID && t.m_eResult != EResult.k_EResultOK)
                        {
                            if (!installed)
                            {
                                m_loadingIndicator.SetActive(false);
                                m_startButton.interactable = true;
                                ModUIUtils.MessagePopupOK("Update error", $"Error code: {t.m_eResult}", 150f, true);
                            }
                            else
                                m_ignoreMinorErrors = true;
                        }
                    });
                }
            }
            else
            {
                ModSteamUGCUtils.SubscribeItem(workshopItem.ItemID, delegate (RemoteStorageSubscribePublishedFileResult_t t, bool ioError)
                {
                    if (t.m_nPublishedFileId == workshopItem.ItemID && (ioError || t.m_eResult != EResult.k_EResultOK))
                    {
                        if (!installed)
                        {
                            m_loadingIndicator.SetActive(false);
                            m_startButton.interactable = true;
                            ModUIUtils.MessagePopupOK("Subscription error", $"Error code: {t.m_eResult} (ioError: {ioError})", 150f, true);
                        }
                        else
                            m_ignoreMinorErrors = true;
                    }
                });
            }
        }

        private void onError(string error)
        {
            if (!m_getCallbacks)
                return;

            m_getCallbacks = false;
            m_loadingIndicator.SetActive(false);
            m_startButton.interactable = true;

            if (ModSteamUGCUtils.IsItemInstalled(LEVEL_STEAM_ID)) // if we have challenge installed then just start it
            {
                startChallenge();
                return;
            }
            ModUIUtils.MessagePopupOK("Error", error, 300f, true);
        }

        private bool isReadyToStartChallenge()
        {
            EItemState itemState = ModSteamUGCUtils.GetItemState(LEVEL_STEAM_ID);
            bool installed = ModSteamUGCUtils.IsItemInstalled(LEVEL_STEAM_ID);
            return installed && (m_ignoreMinorErrors || itemState.HasFlag(EItemState.k_EItemStateSubscribed)) && (m_ignoreMinorErrors || !itemState.HasFlag(EItemState.k_EItemStateNeedsUpdate)) && !itemState.HasFlag(EItemState.k_EItemStateDownloadPending) && !itemState.HasFlag(EItemState.k_EItemStateDownloading);
        }

        private void startChallenge()
        {
            if (m_levelWorkshopItem == null)
            {
                m_levelWorkshopItem = new WorkshopItem()
                {
                    Author = "Archaeologist",
                    AuthorID = (CSteamID)76561198886409131,
                    ItemID = LEVEL_STEAM_ID,
                    Name = "HYPOCRISIS: Chapter 3, Pack 1, The Deserters Saga",
                    Description = string.Empty
                };
            }

            if (SteamUGC.GetItemInstallInfo(m_levelWorkshopItem.ItemID, out _, out string folder, ModSteamUGCUtils.cchFolderSize, out _))
                m_levelWorkshopItem.Folder = folder;

            if (!WorkshopChallengeManager.Instance.StartChallengeFromWorkshop(m_levelWorkshopItem.ToSteamWorkshopItem()))
            {
                ModUIUtils.MessagePopupOK("Incompatible game version!", "This item was made on newer version of the game.\nTo become able to play this level, update the game.", true);
            }
        }
    }
}
