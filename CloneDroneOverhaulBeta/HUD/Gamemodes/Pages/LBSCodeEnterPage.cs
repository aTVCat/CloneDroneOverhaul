using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class LBSCodeEnterPage : FullscreenWindowPageBase
    {
        protected Button GoButton;
        private Button m_BackButton;
        private InputField m_CodeField;

        public override Vector2 GetWindowSize() => OverhaulGamemodesUIFullscreenWindow.CodeEnterWindowSize;
        public override bool AllowPressingBackspace() => false;

        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            m_CodeField = MyModdedObject.GetObject<InputField>(0);
            m_CodeField.text = string.Empty;
            GoButton = MyModdedObject.GetObject<Button>(1);
            GoButton.onClick.AddListener(OnGoClick);
            m_BackButton = MyModdedObject.GetObject<Button>(2);
            m_BackButton.onClick.AddListener(OnBackClick);
        }

        public void OnGoClick()
        {
            GoButton.interactable = false;
            TryJoinLobby();
        }

        public void OnBackClick()
        {
            FullscreenWindow.Hide();
        }

        protected virtual async void TryJoinLobby()
        {
            GameRequestType gameType = GameRequestType.BattleRoyaleInviteCodeJoin;
            GameRequest gameRequest = new GameRequest
            {
                GameType = gameType,
                InviteCodeToJoin = m_CodeField.text,
                Exclusivity = await MultiplayerMatchmakingManager.GetExclusivePlatformAsync()
            };

            MultiplayerMatchmakingManager.Instance.ResetMatchmakingStartTime();
            CustomMatchmakerClientAPI.Instance.FindMatch(new CustomMatchmakeRequest
            {
                GameRequest = gameRequest
            }, delegate (CustomMatchmakeResult result)
            {
                MultiplayerMatchmakingManager.Instance.ConnectToExternalMatchmakeResult(result, gameRequest);
                FullscreenWindow.GamemodesUI.Hide();
            }, delegate (CustomMatchmakerError error)
            {
                GoButton.interactable = true;
                OverhaulDialogues.CreateDialogue("Cannot enter the lobby", error.Type == CustomMatchmakerErrorType.InviteMatchFull ? "The lobby you want to connect to is full." : "No lobby found.", 0f, new Vector2(315, 160), null);
            });
        }

        protected string GetInputFieldValue() => m_CodeField.text;
        protected void SetInputFieldInteractable(bool value) => m_CodeField.interactable = value;
    }
}
