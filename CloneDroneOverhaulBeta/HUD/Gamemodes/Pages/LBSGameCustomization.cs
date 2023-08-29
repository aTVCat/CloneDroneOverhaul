using ModLibrary;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class LBSGameCustomization : FullscreenWindowPageBase
    {
        [UIElementReferenceAttribute("Scroll View")]
        private readonly ScrollRect m_ScrollRect;
        [UIElementReferenceAttribute("GridContent")]
        private readonly CanvasGroup m_GridContent;
        [UIElementReferenceAttribute("VerticalContent")]
        private readonly CanvasGroup m_VerticalContent;

        [UIElementReferenceAttribute("Start")]
        private readonly Button m_Start;

        [UIElementReferenceAttribute("WorkshopMapsPage")]
        private readonly Button m_BrowseWorkshopMaps;
        [UIElementReferenceAttribute("StandardMapsPage")]
        private readonly Button m_BrowseStandardMaps;
        [UIElementReferenceAttribute("LibraryMapsPage")]
        private readonly Button m_BrowseLocalMaps;

        [UIElementReferenceAttribute("IncludeAllToggle")]
        private readonly Toggle m_IncludeAll;
        [UIElementReferenceAttribute("Reload")]
        private readonly Button m_Refresh;
        [UIElementReferenceAttribute("GetMore")]
        private readonly Button m_GetMore;

        [UIElementReferenceAttribute("MapInfo")]
        private readonly GameObject m_MapInfo;
        [UIElementReferenceAttribute("MapTitle")]
        private readonly Text m_MapTitle;
        [UIElementReferenceAttribute("MapCreator")]
        private readonly Text m_MapCreator;

        [UIElementReferenceAttribute("WorkshopMapEntry")]
        private readonly GameObject m_MapDisplayPrefab;

        public int BrowsingLevelsType
        {
            get;
            private set;
        }
        public bool IsBrowsingWorkshopMaps() => BrowsingLevelsType == 0;
        public bool IsBrowsingStandardMaps() => BrowsingLevelsType == 1;
        public bool IsBrowsingLocalMaps() => BrowsingLevelsType == 2;

        public bool HasToShowThumbnails() => !IsBrowsingLocalMaps();

        public bool IsSwitchingContainers
        {
            get;
            private set;
        }

        public bool IsPopulatingLevels
        {
            get;
            private set;
        }

        public bool IsStoppingLevelPopulation
        {
            get;
            private set;
        }

        public List<string> SelectedLevels = new List<string>();

        public override Vector2 GetWindowSize() => OverhaulGamemodesUIFullscreenWindow.GameCustomizationWindowSize;

        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);
            UIController.AssignValues(this);

            m_Start.AddOnClickListener(OnStartClicked);

            m_BrowseWorkshopMaps.AddOnClickListener(OnBrowseWorkshopLevelsClick);
            m_BrowseStandardMaps.AddOnClickListener(OnBrowseStandardLevelsClick);
            m_BrowseLocalMaps.AddOnClickListener(OnBrowseLocalLevelsClick);

            m_IncludeAll.onValueChanged.AddListener(OnIncludeAllClicked);
            m_Refresh.AddOnClickListener(OnRefreshClicked);
            m_GetMore.AddOnClickListener(OnGetMoreClicked);

            m_MapInfo.SetActive(false);
            m_MapDisplayPrefab.SetActive(false);
        }

        public void StartBrowsingLevels(int type)
        {
            if (IsSwitchingContainers)
                return;

            BrowsingLevelsType = type;
            SwitchContainer(HasToShowThumbnails());
            RefreshPageButtons();
        }

        public void RefreshMaps()
        {
            _ = StaticCoroutineRunner.StartStaticCoroutine(refreshMapsCoroutine());
        }

        private IEnumerator refreshMapsCoroutine()
        {
            IsPopulatingLevels = true;
            if (IsBrowsingStandardMaps())
            {
                List<LevelDescription> standardLevels = LevelManager.Instance.GetPrivateField<List<LevelDescription>>("_battleRoyaleLevels");
                foreach (LevelDescription level in standardLevels)
                {
                    if (IsStoppingLevelPopulation)
                    {
                        IsStoppingLevelPopulation = false;
                        IsPopulatingLevels = false;
                        yield break;
                    }

                    if (level.LevelID == "BR_WaitingRoom")
                        continue;

                    LBSMapDisplay display = CreateMapDisplay();
                    display.Initialize(level, this);
                    yield return new WaitForSecondsRealtime(0.016f);
                }
            }
            IsPopulatingLevels = false;

            yield break;
        }

        public void ClearMaps()
        {
            TransformUtils.DestroyAllChildren(m_ScrollRect.content);
        }

        public LBSMapDisplay CreateMapDisplay()
        {
            GameObject display = Instantiate(m_MapDisplayPrefab, m_ScrollRect.content);
            display.SetActive(true);
            return display.AddComponent<LBSMapDisplay>();
        }

        public void RefreshPageButtons()
        {
            m_BrowseWorkshopMaps.interactable = !IsBrowsingWorkshopMaps();
            m_BrowseStandardMaps.interactable = !IsBrowsingStandardMaps();
            m_BrowseLocalMaps.interactable = !IsBrowsingLocalMaps();
        }

        public void SwitchContainer(bool grid)
        {
            _ = StaticCoroutineRunner.StartStaticCoroutine(switchContainerCoroutine(grid));
        }

        private IEnumerator switchContainerCoroutine(bool grid)
        {
            IsStoppingLevelPopulation = IsPopulatingLevels;
            IsSwitchingContainers = true;
            m_GridContent.interactable = false;
            m_VerticalContent.interactable = false;

            for (int i = 0; i < 5; i++)
            {
                m_GridContent.alpha -= 0.2f;
                m_VerticalContent.alpha -= 0.2f;
                yield return new WaitForSecondsRealtime(0.016f);
            }
            yield return new WaitUntil(() => !IsStoppingLevelPopulation && !IsPopulatingLevels);
            yield return null;
            ClearMaps();
            m_ScrollRect.content = (RectTransform)(grid ? m_GridContent.transform : m_VerticalContent.transform);
            m_GridContent.interactable = true;
            m_VerticalContent.interactable = true;
            RefreshMaps();

            yield return null;
            for (int i = 0; i < 5; i++)
            {
                m_GridContent.alpha += 0.2f;
                m_VerticalContent.alpha += 0.2f;
                yield return new WaitForSecondsRealtime(0.016f);
            }
            IsSwitchingContainers = false;
            yield break;
        }

        public void ShowMapInfoTooltip(bool state, Vector3 position, string title, string author)
        {
            if (!state)
            {
                m_MapInfo.SetActive(false);
                return;
            }

            position.x += 5;
            position.z = 1000;
            m_MapInfo.SetActive(true);
            m_MapInfo.transform.position = position;
            m_MapTitle.text = title;
            m_MapCreator.text = author;
        }

        public void OnStartClicked()
        {
            BoltGlobalEventListenerSingleton<MultiplayerMatchmakingManager>.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = GameRequestType.BattleRoyaleInviteCodeCreate
            });
            FullscreenWindow.GamemodesUI.Hide();
        }

        public void OnBrowseWorkshopLevelsClick()
        {
            StartBrowsingLevels(0);
        }

        public void OnBrowseStandardLevelsClick()
        {
            StartBrowsingLevels(1);
        }

        public void OnBrowseLocalLevelsClick()
        {
            StartBrowsingLevels(2);
        }

        public void OnIncludeAllClicked(bool value)
        {

        }

        public void OnRefreshClicked()
        {

        }

        public void OnGetMoreClicked()
        {
            if (!SteamManager.Instance.Initialized)
            {
                OverhaulFullscreenDialogueWindow.ShowOkWindow("Steam error",
                    "It seems that Steam is incorrectly initialized.",
                    300, 175, OverhaulFullscreenDialogueWindow.IconType.Warn);
                return;
            }

            SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/workshop/browse/?appid=597170&requiredtags%5B0%5D=Last+Bot+Standing+Level&actualsort=trend&browsesort=trend&p=1&days=90");
        }
    }
}
