using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIWorkshopBrowser : OverhaulUIBehaviour
    {
        public const string ADVENTURE_LEVEL_TYPE_TAB = "Adventure";
        public const string CHALLENGE_LEVEL_TYPE_TAB = "Challenge";
        public const string ENDLESS_LEVEL_TYPE_TAB = "Endless Level";
        public const string LBS_LEVEL_TYPE_TAB = "Last Bot Standing Level";

        public const string YOUR_LEVELS_SOURCE_TYPE = "user";
        public const string SUBSCRIPTIONS_SOURCE_TYPE = "subscriptions";
        public const string ALL_SOURCE_TYPE = "all";

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("OldUIButton")]
        private readonly Button m_legacyUIButton;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnSourceTabSelected))]
        private readonly TabManager m_sourceTabs;

        [UIElement("YourLevelsTab")]
        public GameObject m_yourLevelsTab;
        [UIElement("SubscriptionsTab")]
        public GameObject m_subscriptionsTab;
        [UIElement("BrowseTab")]
        public GameObject m_browseTab;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnQueryTabSelected))]
        private readonly TabManager m_queryTabs;

        [UIElement("TrendingTab")]
        public GameObject m_trendingTab;
        [UIElement("RecentTab")]
        public GameObject m_recentTab;
        [UIElement("MostPopularTab")]
        public GameObject m_mostPopularTab;
        [UIElement("MostSubscribersTab")]
        public GameObject m_mostSubscribersTab;
        [UIElement("ByFollowedUsersTab")]
        public GameObject m_byFollowedTab;
        [UIElement("ByFriendsTab")]
        public GameObject m_byFriendsTab;
        [UIElement("FriendsFavoritesTab")]
        public GameObject m_friendsFavoritesTab;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnLevelTypeTabSelected))]
        private readonly TabManager m_levelTypeTabs;

        [UIElement("AdventuresTab")]
        public GameObject m_adventuresTab;
        [UIElement("ChallengesTab")]
        public GameObject m_challengesTab;
        [UIElement("EndlessLevelsTab")]
        public GameObject m_endlessLevelsTab;
        [UIElement("LBSLevelsTab")]
        public GameObject m_lastBotStandingLevelsTab;

        [UIElement("WorkshopItemDisplay", false)]
        public ModdedObject m_workshopItemDisplay;

        [UIElement("Content")]
        public Transform m_container;

        [UIElement("LoadingIndicator", false)]
        public GameObject m_loadingIndicator;

        [UIElement("Tabs")]
        public CanvasGroup m_tabsCanvasGroup;

        public override bool hideTitleScreen => true;

        private bool m_initializedTabs;

        private bool m_getWorkshopItemsNextFrame;

        public int sourceType
        {
            get;
            set;
        }

        public string searchLevelType
        {
            get;
            set;
        }

        public CSteamID searchLevelsByUser
        {
            get;
            set;
        }

        public EUserUGCList searchUserList
        {
            get;
            set;
        }

        public EUGCQuery searchQuery
        {
            get;
            set;
        }

        public override void Show()
        {
            base.Show();

            if (!SteamManager.Instance || !SteamManager.Instance.Initialized)
            {
                ModUIUtils.MessagePopupOK("Steam not initialized", "To browse Steam workshop you must have Steam connection established.", true);
                return;
            }

            if (!m_initializedTabs)
            {
                m_levelTypeTabs.AddTab(m_adventuresTab, ADVENTURE_LEVEL_TYPE_TAB);
                m_levelTypeTabs.AddTab(m_challengesTab, CHALLENGE_LEVEL_TYPE_TAB);
                m_levelTypeTabs.AddTab(m_endlessLevelsTab, ENDLESS_LEVEL_TYPE_TAB);
                m_levelTypeTabs.AddTab(m_lastBotStandingLevelsTab, LBS_LEVEL_TYPE_TAB);
                m_levelTypeTabs.SelectTab(ADVENTURE_LEVEL_TYPE_TAB);

                m_queryTabs.AddTab(m_trendingTab, EUGCQuery.k_EUGCQuery_RankedByTrend.ToString());
                m_queryTabs.AddTab(m_recentTab, EUGCQuery.k_EUGCQuery_RankedByPublicationDate.ToString());
                m_queryTabs.AddTab(m_mostPopularTab, EUGCQuery.k_EUGCQuery_RankedByVote.ToString());
                m_queryTabs.AddTab(m_mostSubscribersTab, EUGCQuery.k_EUGCQuery_RankedByTotalUniqueSubscriptions.ToString());
                m_queryTabs.AddTab(m_byFollowedTab, EUGCQuery.k_EUGCQuery_CreatedByFollowedUsersRankedByPublicationDate.ToString());
                m_queryTabs.AddTab(m_byFriendsTab, EUGCQuery.k_EUGCQuery_CreatedByFriendsRankedByPublicationDate.ToString());
                m_queryTabs.AddTab(m_friendsFavoritesTab, EUGCQuery.k_EUGCQuery_FavoritedByFriendsRankedByPublicationDate.ToString());
                m_queryTabs.SelectTab(EUGCQuery.k_EUGCQuery_RankedByTrend.ToString());

                m_sourceTabs.AddTab(m_yourLevelsTab, YOUR_LEVELS_SOURCE_TYPE);
                m_sourceTabs.AddTab(m_subscriptionsTab, SUBSCRIPTIONS_SOURCE_TYPE);
                m_sourceTabs.AddTab(m_browseTab, ALL_SOURCE_TYPE);
                m_sourceTabs.SelectTab(ALL_SOURCE_TYPE);
                m_initializedTabs = true;
            }

            Populate();
        }

        public override void Hide()
        {
            base.Hide();

            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);
        }

        public override void Update()
        {
            if (m_getWorkshopItemsNextFrame)
                populate();
        }

        public void OnLevelTypeTabSelected(UIElementTab elementTab)
        {
            UIElementTab oldTab = m_levelTypeTabs.prevSelectedTab;
            UIElementTab newTab = m_levelTypeTabs.selectedTab;
            if (oldTab)
            {
                RectTransform rt = oldTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 25f;
                rt.sizeDelta = vector;
            }
            if (newTab)
            {
                RectTransform rt = newTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 30f;
                rt.sizeDelta = vector;
            }

            searchLevelType = elementTab.tabId;
            Populate();
        }

        public void OnQueryTabSelected(UIElementTab elementTab)
        {
            UIElementTab oldTab = m_levelTypeTabs.prevSelectedTab;
            UIElementTab newTab = m_levelTypeTabs.selectedTab;
            if (oldTab)
            {
                RectTransform rt = oldTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.x = 150f;
                rt.sizeDelta = vector;
            }
            if (newTab)
            {
                RectTransform rt = newTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.x = 160f;
                rt.sizeDelta = vector;
            }

            if (!Enum.TryParse(elementTab.tabId, out EUGCQuery result))
                result = EUGCQuery.k_EUGCQuery_RankedByTrend;

            searchQuery = result;
            Populate();
        }

        public void OnSourceTabSelected(UIElementTab elementTab)
        {
            CSteamID steamId = CSteamID.Nil;
            string tabId = elementTab.tabId;
            if (tabId == YOUR_LEVELS_SOURCE_TYPE)
            {
                sourceType = 2;
                searchUserList = EUserUGCList.k_EUserUGCList_Published;
                steamId = SteamUser.GetSteamID();

                m_tabsCanvasGroup.alpha = 0.25f;
                m_tabsCanvasGroup.interactable = false;
            }
            else if (tabId == SUBSCRIPTIONS_SOURCE_TYPE)
            {
                sourceType = 1;
                searchUserList = EUserUGCList.k_EUserUGCList_Subscribed;
                steamId = SteamUser.GetSteamID();

                m_tabsCanvasGroup.alpha = 0.25f;
                m_tabsCanvasGroup.interactable = false;
            }
            else
            {
                sourceType = 0;

                m_tabsCanvasGroup.alpha = 1f;
                m_tabsCanvasGroup.interactable = true;
            }
            searchLevelsByUser = steamId;
            Populate();
        }

        public void Populate()
        {
            m_getWorkshopItemsNextFrame = true;
        }

        private void populate()
        {
            m_getWorkshopItemsNextFrame = false;
            m_sourceTabs.interactable = false;
            m_levelTypeTabs.interactable = false;
            m_queryTabs.interactable = false;
            m_loadingIndicator.SetActive(true);

            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            ModSteamUGCUtils.RequestParameters requestParameters = new ModSteamUGCUtils.RequestParameters();
            requestParameters.EnableCaching();
            requestParameters.RequireTags(new List<string>() { searchLevelType });

            bool success = sourceType == 0
                ? ModSteamUGCUtils.GetAllWorkshopItems(searchQuery, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, 1, requestParameters, onGotItems, onError, null)
                : ModSteamUGCUtils.GetWorkshopUserItemList(searchLevelsByUser, 1, searchUserList, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_SubscriptionDateDesc, requestParameters, onGotItems, onError, null);
            if (!success)
            {
                onError("Internal error.");
            }
        }

        private void onGotItems(List<SteamWorkshopItem> list)
        {
            m_sourceTabs.interactable = true;
            m_levelTypeTabs.interactable = true;
            m_queryTabs.interactable = true;
            m_loadingIndicator.SetActive(false);

            if (list.IsNullOrEmpty())
                return;

            foreach (SteamWorkshopItem steamWorkshopItem in list)
            {
                ModdedObject moddedObject = Instantiate(m_workshopItemDisplay, m_container);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = steamWorkshopItem.Title;
                UIElementWorkshopItemDisplay workshopItemDisplay = moddedObject.gameObject.AddComponent<UIElementWorkshopItemDisplay>();
                workshopItemDisplay.InitializeElement();
                workshopItemDisplay.Populate(steamWorkshopItem);
            }
        }

        private void onError(string error)
        {
            m_sourceTabs.interactable = true;
            m_levelTypeTabs.interactable = true;
            m_queryTabs.interactable = true;
            m_loadingIndicator.SetActive(false);

            ModUIUtils.MessagePopupOK("Could not get workshop items", error, "ok", Populate, 150f, true);
        }

        public void OnLegacyUIButtonClicked()
        {
            TitleScreenUI titleScreenUI = ModCache.titleScreenUI;
            if (titleScreenUI)
            {
                Hide();
                titleScreenUI.OnWorkshopBrowserButtonClicked();
            }
        }
    }
}
