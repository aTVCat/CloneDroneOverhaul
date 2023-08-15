using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class CoopChallengePrivateMatchActions : FullscreenWindowPageBase
    {
        public static string ChallengeID;

        private Button m_PublicButton;
        private Button m_PrivateButton;

        public override Vector2 GetWindowSize() => new Vector3(300f, 125f);

        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            m_PublicButton = MyModdedObject.GetObject<Button>(0);
            m_PublicButton.onClick.AddListener(OnPublicClick);
            m_PrivateButton = MyModdedObject.GetObject<Button>(1);
            m_PrivateButton.onClick.AddListener(OnPrivateClick);
        }

        public void OnPrivateClick()
        {
            void action()
            {
                ChallengeManager.Instance.CreatePrivateCoopChallenge(ChallengeID);
                FullscreenWindow.GamemodesUI.Hide();
            }
            OverhaulTransitionController.DoTransitionWithAction(action, null, 0.10f);
        }

        public void OnPublicClick()
        {
            void action()
            {
                ChallengeManager.Instance.JoinPublicCoopChallenge(ChallengeID);
                FullscreenWindow.GamemodesUI.Hide();
            }
            OverhaulTransitionController.DoTransitionWithAction(action, null, 0.10f);
        }
    }
}
