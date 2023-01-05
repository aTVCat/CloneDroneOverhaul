using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3Tests.HUD
{
    public class UIMultiplayer : V3_ModHUDBase
    {
        public enum EMultiplayerUI
        {
            BattleRoyale,

            Duel,

            Coop
        }

        public List<EMultiplayerUI> ActiveMultiplayerUIs = new List<EMultiplayerUI>();
        public ModdedObject BRMObj;

        private bool _isResultShownAlready;

        private void Start()
        {
            BRMObj = base.MyModdedObject.GetObjectFromList<ModdedObject>(0);

            BRMObj.GetObjectFromList<Button>(8).onClick.AddListener(GameUIRoot.Instance.BattleRoyaleUI.ResultScreen.OnNextGameClicked);
            BRMObj.GetObjectFromList<Button>(10).onClick.AddListener(GameUIRoot.Instance.BattleRoyaleUI.ResultScreen.OnMainMenuButtonClicked);
            BRMObj.GetObjectFromList<Animator>(0).Play("BR_Loop");

            foreach (GameObject obj in MyModdedObject.objects)
            {
                obj.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            if (BattleRoyaleManager.Instance != null)
            {
                BRMObj.GetObjectFromList<RectTransform>(0).gameObject.SetActive(BattleRoyaleManager.Instance.IsProgress(BattleRoyaleMatchProgress.InWaitingArea));
                BR_ShowResultScreen(false);
            }
        }

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (BattleRoyaleManager.Instance == null)
            {
                return;
            }

            if (eventName == "onBoltShutdown")
            {
                base.gameObject.SetActive(false);
            }
            if (eventName == "battleRoyale.MatchProgressUpdated")
            {
                BattleRoyaleMatchProgress progress = (BattleRoyaleMatchProgress)args[0];
            }
            if (eventName == "battleRoyale.TimeToGameStartUpdated")
            {
                int secs = (int)args[0];
                BRMObj.GetObjectFromList<Text>(2).gameObject.SetActive(secs != -1 && secs >= 1);
                BRMObj.GetObjectFromList<Text>(2).text = secs.ToString();
            }
            if (eventName == "battleRoyale.NumMultiplayerPlayersChanged")
            {
                int count = (int)args[0];
                BRMObj.GetObjectFromList<Text>(5).text = count.ToString();
            }
            if (eventName == "battleRoyale.CharacterKilled")
            {
                BR_ShowResultScreen(!Singleton<CharacterTracker>.Instance.IsLocalMultiplayerCharacterAlive());
            }
        }

        private void BR_ShowResultScreen(bool showBecauseOfDeath)
        {
            if (BattleRoyaleManager.Instance == null)
            {
                return;
            }

            bool shouldShowUI = false;
            bool isMatchedSettled = BattleRoyaleManager.Instance.IsProgress(BattleRoyaleMatchProgress.MatchSettled);

            if (!showBecauseOfDeath)
            {
                shouldShowUI = isMatchedSettled;
            }
            else
            {
                shouldShowUI = true;
            }

            if (!BattleRoyaleManager.Instance.IsProgressAtLeast(BattleRoyaleMatchProgress.FightingStarted))
            {
                shouldShowUI = false;
            }

            if (!shouldShowUI)
            {
                return;
            }

            bool areWeTheWinner = BattleRoyaleManager.Instance.IsLocalPlayerTheWinner();
            MultiplayerPlayerInfoState state = MultiplayerPlayerInfoManager.Instance.GetLocalPlayerInfoState();

            BRMObj.GetObjectFromList<RectTransform>(11).gameObject.SetActive(isMatchedSettled);

            if (isMatchedSettled)
            {
                BRMObj.GetObjectFromList<Text>(12).text = BattleRoyaleManager.Instance.GetWinnerName();
                if (BattleRoyaleManager.Instance.IsLocalPlayerTheWinner())
                {
                    ColorUtility.TryParseHtmlString("#99FFA2", out Color col);
                    BRMObj.GetObjectFromList<Text>(12).color = col;

                    ColorUtility.TryParseHtmlString("#147816", out col);
                    BRMObj.GetObjectFromList<Image>(20).color = col;

                    ColorUtility.TryParseHtmlString("#25572B", out col);
                    BRMObj.GetObjectFromList<Image>(8).color = col;
                    BRMObj.GetObjectFromList<Image>(9).color = col;
                    BRMObj.GetObjectFromList<Image>(10).color = col;

                }
                else
                {
                    ColorUtility.TryParseHtmlString("#FB6262", out Color col);
                    BRMObj.GetObjectFromList<Text>(12).color = col;

                    ColorUtility.TryParseHtmlString("#871D22", out col);
                    BRMObj.GetObjectFromList<Image>(20).color = col;

                    ColorUtility.TryParseHtmlString("#741D2D", out col);
                    BRMObj.GetObjectFromList<Image>(8).color = col;
                    BRMObj.GetObjectFromList<Image>(9).color = col;
                    BRMObj.GetObjectFromList<Image>(10).color = col;
                }
            }
            BRMObj.GetObjectFromList<Image>(9).gameObject.SetActive(!isMatchedSettled);
            BRMObj.GetObjectFromList<RectTransform>(6).gameObject.SetActive(true);
            if (!_isResultShownAlready)
            {
                BRMObj.GetObjectFromList<Animator>(6).Play("BR_ResultScreen");
            }

            GameUIRoot.Instance.RefreshCursorEnabled();

            //Count the place
            if (!_isResultShownAlready)
            {
                BRMObj.GetObjectFromList<Text>(17).text = state.state.Kills.ToString();
                BRMObj.GetObjectFromList<Text>(16).text = "N/A";
                int num = Singleton<MultiplayerPlayerInfoManager>.Instance.GetGamePlacementOfLocalPlayer();
                if (areWeTheWinner)
                {
                    num = 1;
                }
                if (num != 0)
                {
                    BRMObj.GetObjectFromList<Text>(16).text = num + "/" + Singleton<MultiplayerPlayerInfoManager>.Instance.GetPlayerCountIncludingDisconnects();
                }
            }

            _isResultShownAlready = true;
        }

        public void Show(EMultiplayerUI toShow)
        {
            if (!ActiveMultiplayerUIs.Contains(toShow))
            {
                ActiveMultiplayerUIs.Add(toShow);
            }
            if (toShow == EMultiplayerUI.BattleRoyale)
            {
                base.MyModdedObject.GetObjectFromList<RectTransform>(0).gameObject.SetActive(true);
                BRMObj.GetObjectFromList<RectTransform>(6).gameObject.SetActive(false);
            }
        }
        public void Hide(EMultiplayerUI hide)
        {
            if (ActiveMultiplayerUIs.Contains(hide))
            {
                ActiveMultiplayerUIs.Remove(hide);
            }
            if (hide == EMultiplayerUI.BattleRoyale)
            {
                base.MyModdedObject.GetObjectFromList<RectTransform>(0).gameObject.SetActive(false);
            }
        }
    }
}
