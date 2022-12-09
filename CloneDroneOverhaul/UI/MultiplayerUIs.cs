using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class MultiplayerUIs : ModGUIBase
    {
        public static MultiplayerUIs Instance;

        public enum MultiplayerUI
        {
            BattleRoyale,

            Duel,

            Coop
        }

        public List<MultiplayerUI> ActiveMultiplayerUIs = new List<MultiplayerUI>();
        public ModdedObject BRMObj;

        public override void OnInstanceStart()
        {
            Instance = this;
            base.MyModdedObject = base.GetComponent<ModdedObject>();
            BRMObj = base.MyModdedObject.GetObjectFromList<ModdedObject>(0);

            BRMObj.GetObjectFromList<Button>(8).onClick.AddListener(GameUIRoot.Instance.BattleRoyaleUI.ResultScreen.OnNextGameClicked);
            BRMObj.GetObjectFromList<Button>(10).onClick.AddListener(GameUIRoot.Instance.BattleRoyaleUI.ResultScreen.OnMainMenuButtonClicked);
            BRMObj.GetObjectFromList<Animator>(0).Play("BR_Loop");

            foreach (GameObject obj in MyModdedObject.objects)
            {
                obj.SetActive(false);
            }
        }

        public override void OnManagedUpdate()
        {
            if (BattleRoyaleManager.Instance != null)
            {
                BRMObj.GetObjectFromList<RectTransform>(0).gameObject.SetActive(BattleRoyaleManager.Instance.IsProgress(BattleRoyaleMatchProgress.InWaitingArea));
                BR_ShowResultScreen(false);
            }
        }

        public override void RunFunction(string name, object[] arguments)
        {
            if (BattleRoyaleManager.Instance == null)
            {
                return;
            }

            if (name == "onBoltShutdown")
            {
                base.gameObject.SetActive(false);
            }
            if (name == "battleRoyale.MatchProgressUpdated")
            {
                BattleRoyaleMatchProgress progress = (BattleRoyaleMatchProgress)arguments[0];
            }
            if (name == "battleRoyale.TimeToGameStartUpdated")
            {
                if (BattleRoyaleManager.Instance == null)
                {
                    return;
                }
                if (BattleRoyaleManager.Instance.IsProgress(BattleRoyaleMatchProgress.InWaitingArea))
                {

                }
                int secs = (int)arguments[0];
                BRMObj.GetObjectFromList<Text>(2).gameObject.SetActive(secs != -1 && secs >= 1);
                BRMObj.GetObjectFromList<Text>(2).text = secs.ToString();
            }
            if (name == "battleRoyale.NumMultiplayerPlayersChanged")
            {
                int count = (int)arguments[0];
                BRMObj.GetObjectFromList<Text>(5).text = count.ToString();
            }
            if (name == "battleRoyale.CharacterKilled")
            {
                BR_ShowResultScreen(!Singleton<CharacterTracker>.Instance.IsLocalMultiplayerCharacterAlive());
            }
        }

        private bool _isResultShownAlready;
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
                if (isMatchedSettled)
                {
                    shouldShowUI = true;
                }
                else
                {
                    shouldShowUI = false;
                }
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
                    Color col;
                    ColorUtility.TryParseHtmlString("#99FFA2", out col);
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
                    Color col;
                    ColorUtility.TryParseHtmlString("#FB6262", out col);
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
            if (!_isResultShownAlready) BRMObj.GetObjectFromList<Animator>(6).Play("BR_ResultScreen");
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

        public void Show(MultiplayerUI toShow)
        {
            if (!ActiveMultiplayerUIs.Contains(toShow))
            {
                ActiveMultiplayerUIs.Add(toShow);
            }
            if (toShow == MultiplayerUI.BattleRoyale)
            {
                base.MyModdedObject.GetObjectFromList<RectTransform>(0).gameObject.SetActive(true);
                BRMObj.GetObjectFromList<RectTransform>(6).gameObject.SetActive(false);
            }
        }
        public void Hide(MultiplayerUI hide)
        {
            if (ActiveMultiplayerUIs.Contains(hide))
            {
                ActiveMultiplayerUIs.Remove(hide);
            }
            if (hide == MultiplayerUI.BattleRoyale)
            {
                base.MyModdedObject.GetObjectFromList<RectTransform>(0).gameObject.SetActive(false);
            }
        }
    }
}
