using OverhaulMod.Content;
using OverhaulMod.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UITitleScreenHypocrisisSkin : OverhaulUIBehaviour
    {
        public const string LEVEL_STEAM_ID_STRING = "3494951054";

        public static readonly PublishedFileId_t LEVEL_STEAM_ID = new PublishedFileId_t(3494951054);

        public const string LEVEL_AUTHOR_STEAM_PROFILE_PAGE = "https://steamcommunity.com/profiles/76561198886409131";

        public const string LEVEL_AUTHOR_DISCORD_SERVER = "https://discord.gg/MNQMANtzXe";

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

        public override bool closeOnEscapeButtonPress => false;

        private bool m_getCallbacks;

        private bool m_startChallengeWhenReady;

        private float m_timeLeftToUpdate;

        private bool m_ignoreMinorErrors;

        private WorkshopItem m_levelWorkshopItem;

        protected override void OnInitialized()
        {
            m_startButton.interactable = true;
        }

        public override void Start()
        {
            ArenaCameraManager.Instance.SetTitleScreenLogoVisible(false);
            ArenaCameraManager.Instance.TitleScreenLogoCamera.gameObject.SetActive(false);
        }

        public override void OnEnable()
        {
            ArenaCameraManager.Instance.SetTitleScreenLogoVisible(false);
            ArenaCameraManager.Instance.TitleScreenLogoCamera.gameObject.SetActive(false);
        }

        public override void OnDisable()
        {
            if (ModCache.titleScreenUI.gameObject.activeSelf)
            {
                ArenaCameraManager.Instance.SetTitleScreenLogoVisible(true);
                ArenaCameraManager.Instance.TitleScreenLogoCamera.gameObject.SetActive(true);
            }
        }

        public override void Update()
        {
            m_holder.SetActive(ModCache.titleScreenRootButtonsBG.activeInHierarchy);

            m_progressBarFill.fillAmount = ModSteamUGCUtils.GetItemDownloadProgress(LEVEL_STEAM_ID);

            m_timeLeftToUpdate = Mathf.Max(0f, m_timeLeftToUpdate - Time.unscaledDeltaTime);
            if (m_timeLeftToUpdate == 0f)
            {
                m_timeLeftToUpdate = 1f;

                ArenaCameraManager.Instance.TitleScreenLogoCamera.gameObject.SetActive(false);

                if (m_startChallengeWhenReady && isReadyToStartChallenge())
                {
                    m_startChallengeWhenReady = false;
                    startChallenge();
                }
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
                    Name = "HYPOCRISIS: Chapter 3, The Deserters Saga",
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
