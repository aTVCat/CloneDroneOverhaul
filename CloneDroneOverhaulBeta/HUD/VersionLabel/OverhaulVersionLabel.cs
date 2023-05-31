using CDOverhaul.NetworkAssets;
using CDOverhaul.Workshop;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulVersionLabel : OverhaulUI
    {
        [OverhaulSetting("Game interface.Information.Watermark", true, false, "Show mod version label during gameplay")]
        public static bool WatermarkEnabled;

        public static bool ShowDiscordLabel = true;

        private OverhaulParametersMenu m_ParametersMenu;
        public static OverhaulVersionLabel Instance { get; private set; }

        private Text m_VersionLabel;
        private Text m_TitleScreenUIVersionLabel;

        private Transform m_DiscordHolderTransform;
        private Transform m_ServersList;
        private PrefabAndContainer m_ServersContainer;
        private Button m_ServersButton;
        private Button m_CloseButton;

        private Transform m_UpperButtonsContainer;
        private Button m_PatchNotesButton;

        private GameObject m_TitleScreenRootButtons;

        private bool m_wasOnTitleScreenBefore;

        private static readonly List<Texture> m_LoadedTextures = new List<Texture>();

        public override void Initialize()
        {
            if (IsDisposedOrDestroyed())
                return;

            Instance = this;
            if (GameUIRoot.Instance == null || GameUIRoot.Instance.TitleScreenUI == null || GameUIRoot.Instance.TitleScreenUI.VersionLabel == null)
            {
                base.enabled = false;
                return;
            }

            m_DiscordHolderTransform = MyModdedObject.GetObject<Transform>(1);
            m_DiscordHolderTransform.gameObject.SetActive(OverhaulFeatureAvailabilitySystem.BuildImplements.IsDiscordPanelEnabled && ShowDiscordLabel);
            m_ServersList = MyModdedObject.GetObject<Transform>(9);
            m_ServersList.gameObject.SetActive(false);
            m_ServersContainer = new PrefabAndContainer(MyModdedObject, 10, 11);
            m_ServersButton = MyModdedObject.GetObject<Button>(7);
            m_ServersButton.onClick.AddListener(onServersButtonClicked);
            m_CloseButton = MyModdedObject.GetObject<Button>(8);
            m_CloseButton.onClick.AddListener(delegate
            {
                ShowDiscordLabel = false;
                m_DiscordHolderTransform.gameObject.SetActive(false);
            });

            m_VersionLabel = MyModdedObject.GetObject<Text>(0);
            m_VersionLabel.gameObject.SetActive(true);
            m_TitleScreenUIVersionLabel = GameUIRoot.Instance.TitleScreenUI.VersionLabel;
            m_TitleScreenUIVersionLabel.gameObject.SetActive(false);
            m_TitleScreenRootButtons = GameUIRoot.Instance.TitleScreenUI.RootButtonsContainerBG;

            m_UpperButtonsContainer = MyModdedObject.GetObject<Transform>(5);
            m_PatchNotesButton = MyModdedObject.GetObject<Button>(6);
            m_PatchNotesButton.onClick.AddListener(onPatchNotesButtonClicked);

            _ = OverhaulEventsController.AddEventListener(SettingsController.SettingChangedEventString, refreshVisibility);

            DelegateScheduler.Instance.Schedule(delegate
            {
                m_ParametersMenu = GetController<OverhaulParametersMenu>();
            }, 0.1f);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            Instance = null;

            OverhaulEventsController.RemoveEventListener(SettingsController.SettingChangedEventString, refreshVisibility);
        }

        public void RefreshVersionLabel()
        {
            if (IsDisposedOrDestroyed())
                return;

            if (GameModeManager.IsOnTitleScreen())
            {
                bool workshopBrowserIsActive = OverhaulWorkshopBrowserUI.IsActive;
                bool parametersMenuIsActive = OverhaulParametersMenu.IsActive;
                bool bootuiIsActive = OverhaulBootUI.IsActive;

                m_DiscordHolderTransform.gameObject.SetActive(OverhaulFeatureAvailabilitySystem.BuildImplements.IsDiscordPanelEnabled && ShowDiscordLabel && !bootuiIsActive && !parametersMenuIsActive && !workshopBrowserIsActive);
                m_UpperButtonsContainer.gameObject.SetActive(m_TitleScreenRootButtons != null && m_TitleScreenRootButtons.activeInHierarchy);
                if (workshopBrowserIsActive || bootuiIsActive)
                {
                    m_VersionLabel.text = string.Empty;
                    return;
                }
                m_VersionLabel.text = string.Concat(m_TitleScreenUIVersionLabel.text,
               "\n",
                OverhaulVersion.ModFullName);
            }
            else
            {
                m_VersionLabel.text = OverhaulVersion.ModShortName;
                m_VersionLabel.gameObject.SetActive(WatermarkEnabled);
                m_UpperButtonsContainer.gameObject.SetActive(false);
                m_DiscordHolderTransform.gameObject.SetActive(false);
            }
        }

        public void ShowDiscordPanel() => m_DiscordHolderTransform.gameObject.SetActive(OverhaulFeatureAvailabilitySystem.BuildImplements.IsDiscordPanelEnabled);

        private void refreshVisibility() => m_wasOnTitleScreenBefore = !m_wasOnTitleScreenBefore;

        private void onPatchNotesButtonClicked()
        {
            OverhaulPatchNotesUI overhaulPatchNotesUI = GetController<OverhaulPatchNotesUI>();
            if (overhaulPatchNotesUI == null)
            {
                m_PatchNotesButton.interactable = false;
                return;
            }

            overhaulPatchNotesUI.Show();
        }

        private void onServersButtonClicked()
        {
            m_ServersList.gameObject.SetActive(!m_ServersList.gameObject.activeSelf);
            if (m_ServersList.gameObject.activeSelf)
            {
                if (!m_LoadedTextures.IsNullOrEmpty())
                {
                    foreach (Texture t in m_LoadedTextures)
                        if (t != null && t)
                            Destroy(t);
                    m_LoadedTextures.Clear();
                }
                m_ServersContainer.ClearContainer();

                ModdedObject overhaulServer = m_ServersContainer.CreateNew();
                overhaulServer.GetObject<Text>(0).text = "Clone Drone Overhaul Mod Discord";
                overhaulServer.GetObject<Button>(2).onClick.AddListener(delegate
                {
                    Application.OpenURL("https://discord.gg/qUkRhaqZZZ");
                });
                OverhaulNetworkDownloadHandler h1 = new OverhaulNetworkDownloadHandler();
                h1.DoneAction = delegate
                {
                    if (h1 != null && !h1.Error && overhaulServer != null)
                    {
                        m_LoadedTextures.Add(h1.DownloadedTexture);
                        overhaulServer.GetObject<RawImage>(1).texture = h1.DownloadedTexture;
                    }
                    else if (h1.DownloadedTexture)
                        Destroy(h1.DownloadedTexture);
                };
                OverhaulNetworkController.DownloadTexture("file://" + OverhaulMod.Core.ModDirectory + "Assets/Discord/ServerIcons/Clone Drone Overhaul Mod Discord.png", h1);

                ModdedObject modBotServer = m_ServersContainer.CreateNew();
                modBotServer.GetObject<Text>(0).text = "Clone Drone Mod-Bot discord";
                modBotServer.GetObject<Button>(2).onClick.AddListener(delegate
                {
                    Application.OpenURL("https://discord.gg/Em4n6gB");
                });
                OverhaulNetworkDownloadHandler h2 = new OverhaulNetworkDownloadHandler();
                h2.DoneAction = delegate
                {
                    if (h2 != null && !h2.Error && overhaulServer != null)
                    {
                        m_LoadedTextures.Add(h2.DownloadedTexture);
                        modBotServer.GetObject<RawImage>(1).texture = h2.DownloadedTexture;
                    }
                    else if (h2.DownloadedTexture)
                        Destroy(h2.DownloadedTexture);
                };
                OverhaulNetworkController.DownloadTexture("file://" + OverhaulMod.Core.ModDirectory + "Assets/Discord/ServerIcons/Clone Drone Mod-Bot discord.png", h2);

                ModdedObject doborogServer = m_ServersContainer.CreateNew();
                doborogServer.GetObject<Text>(0).text = "Doborog";
                doborogServer.GetObject<Button>(2).onClick.AddListener(delegate
                {
                    Application.OpenURL("https://discord.com/invite/VY7zEw2chm");
                });
                OverhaulNetworkDownloadHandler h3 = new OverhaulNetworkDownloadHandler();
                h3.DoneAction = delegate
                {
                    if (h3 != null && !h3.Error && overhaulServer != null)
                    {
                        m_LoadedTextures.Add(h3.DownloadedTexture);
                        doborogServer.GetObject<RawImage>(1).texture = h3.DownloadedTexture;
                    }
                    else if (h3.DownloadedTexture)
                        Destroy(h3.DownloadedTexture);
                };
                OverhaulNetworkController.DownloadTexture("file://" + OverhaulMod.Core.ModDirectory + "Assets/Discord/ServerIcons/Doborog.png", h3);

                ModdedObject clansServer = m_ServersContainer.CreateNew();
                clansServer.GetObject<Text>(0).text = "Clan Headquarters RU/EN";
                clansServer.GetObject<Button>(2).onClick.AddListener(delegate
                {
                    Application.OpenURL("https://discord.gg/MM2PNdRV5P");
                });
                OverhaulNetworkDownloadHandler h4 = new OverhaulNetworkDownloadHandler();
                h4.DoneAction = delegate
                {
                    if (h4 != null && !h4.Error && overhaulServer != null)
                    {
                        m_LoadedTextures.Add(h4.DownloadedTexture);
                        clansServer.GetObject<RawImage>(1).texture = h4.DownloadedTexture;
                    }
                    else if (h4.DownloadedTexture)
                        Destroy(h4.DownloadedTexture);
                };
                OverhaulNetworkController.DownloadTexture("file://" + OverhaulMod.Core.ModDirectory + "Assets/Discord/ServerIcons/Clan Headquarters RU-EN.png", h4);
            }
        }

        private void Update()
        {
            if (!IsDisposedOrDestroyed() && Time.frameCount % 5 == 0)
                RefreshVersionLabel();
        }
    }
}