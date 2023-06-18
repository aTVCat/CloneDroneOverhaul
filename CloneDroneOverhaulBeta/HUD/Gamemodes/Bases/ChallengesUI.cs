using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class ChallengesUI : OverhaulGamemodeUIBase
    {
        private bool m_HasPopulatedChallenges;

        private OverhaulUI.PrefabAndContainer ChallengesContainer;

        protected override void OnInitialize()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            ChallengesContainer = new OverhaulUI.PrefabAndContainer(moddedObject, 0, 1);
            moddedObject.GetObject<Button>(2).onClick.AddListener(goBackToGamemodeSelection);
        }

        protected override void OnShow()
        {
            GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/ChallengesBG_" + UnityEngine.Random.Range(1, 5) + ".jpg");
            populateChallengesIfNeeded();
        }

        public override void Update()
        {
            base.Update();

            if (!GamemodesUI.AllowSwitching || GamemodesUI.FullscreenWindow.IsActive)
                return;

            if (Input.GetKeyDown(KeyCode.Backspace))
                goBackToGamemodeSelection();
        }

        private void goBackToGamemodeSelection()
        {
            Hide();
            GameUIRoot.Instance.TitleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(true);
        }

        private void populateChallengesIfNeeded()
        {
            if (m_HasPopulatedChallenges)
                return;

            m_HasPopulatedChallenges = true;

            ChallengesContainer.ClearContainer();
            var allSoloChallenges = ChallengeManager.Instance.GetChallenges(false);
            foreach(ChallengeDefinition definition in allSoloChallenges)
            {
                ModdedObject moddedObject = ChallengesContainer.CreateNew();
                moddedObject.gameObject.AddComponent<UIChallengeEntry>().Initialize(definition, moddedObject, this);
            }
        }

        public class UIChallengeEntry : OverhaulBehaviour
        {
            private ChallengesUI m_ChallengesUI;
            private ChallengeDefinition m_ChallengeDefinition;

            private GameObject[] m_ShowWhenCompleted;

            private Button m_ButtonComponent;
            private Text m_Title;
            private Text m_Description;
            private Image m_Preview;

            public UIChallengeEntry Initialize(ChallengeDefinition definition, ModdedObject moddedObject, ChallengesUI challengesUI)
            {
                m_ChallengesUI = challengesUI;
                m_ChallengeDefinition = definition;
                m_ShowWhenCompleted = new GameObject[2] { moddedObject.GetObject<Transform>(0).gameObject, moddedObject.GetObject<Transform>(1).gameObject };
                m_ButtonComponent = base.GetComponent<Button>();
                m_ButtonComponent.onClick.AddListener(StartChallenge);
                m_Title = moddedObject.GetObject<Text>(3);
                m_Description = moddedObject.GetObject<Text>(4);
                m_Preview = moddedObject.GetObject<Image>(2);
                RefreshUI();
                return this;
            }

            public void SetCompletedVizuals(bool value)
            {
                foreach (GameObject gameObject in m_ShowWhenCompleted)
                    gameObject.SetActive(value);
            }

            public void RefreshUI()
            {
                if (m_ChallengeDefinition == null)
                    return;

                SetCompletedVizuals(ChallengeManager.Instance.HasCompletedChallenge(m_ChallengeDefinition.ChallengeID));
                m_Title.text = LocalizationManager.Instance.GetTranslatedString(m_ChallengeDefinition.ChallengeName, -1);
                m_Description.text = LocalizationManager.Instance.GetTranslatedString(m_ChallengeDefinition.ChallengeDescription, -1);
                m_Preview.sprite = m_ChallengeDefinition.ImageSprite;
            }

            public void StartChallenge()
            {
                m_ChallengesUI.Hide();
                ChallengeManager.Instance.StartChallenge(m_ChallengeDefinition, false);
            }
        }
    }
}
