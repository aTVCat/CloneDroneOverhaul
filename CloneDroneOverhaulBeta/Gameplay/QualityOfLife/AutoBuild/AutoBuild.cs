using ModLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AutoBuild : OverhaulGameplayController
    {
        [OverhaulSettingDropdownParameters("None@Random@Full sword@Full bow@Full hammer@Full spear@Kicker@Light kicker@Armored kicker@Kick'N'Dash@Kick'N'Run@Tactical sword@Tactical fire sword@Jetpack sword@Tactical bow@Tactical fire bow@Tactical Fire Spear@Fire Spear Kicker@Tactical Hammer@Hammer Kicker@Spectator")]
        [OverhaulSetting("QoL.Last Bot Standing.Auto-Build", 0, false, " ", null)]
        public static int SelectedAutoBuildVariant;

        /// <summary>
        /// All possible auto upgrade types
        /// </summary>
        public static readonly Dictionary<AutoBuildVariant, AutoBuildSequence> AutoUpgradeSequences = new Dictionary<AutoBuildVariant, AutoBuildSequence>()
        {
            { AutoBuildVariant.FullSword, new AutoBuildSequence(AutoBuildVariant.FullSword) },
            { AutoBuildVariant.FullBow, new AutoBuildSequence(AutoBuildVariant.FullBow) },
            { AutoBuildVariant.FullHammer, new AutoBuildSequence(AutoBuildVariant.FullHammer) },
            { AutoBuildVariant.FullSpear, new AutoBuildSequence(AutoBuildVariant.FullSpear) },

            { AutoBuildVariant.Kicker, new AutoBuildSequence(AutoBuildVariant.Kicker) },
            { AutoBuildVariant.LightKicker, new AutoBuildSequence(AutoBuildVariant.LightKicker) },
            { AutoBuildVariant.ArmoredKicker, new AutoBuildSequence(AutoBuildVariant.ArmoredKicker) },
            { AutoBuildVariant.KickNDash, new AutoBuildSequence(AutoBuildVariant.KickNDash) },
            { AutoBuildVariant.KickNRun, new AutoBuildSequence(AutoBuildVariant.KickNRun) },

            { AutoBuildVariant.TacticalSword, new AutoBuildSequence(AutoBuildVariant.TacticalSword) },
            { AutoBuildVariant.TacticalFireSword, new AutoBuildSequence(AutoBuildVariant.TacticalFireSword) },
            { AutoBuildVariant.JetPackSword, new AutoBuildSequence(AutoBuildVariant.JetPackSword) },
            { AutoBuildVariant.TacticalBow, new AutoBuildSequence(AutoBuildVariant.TacticalBow) },
            { AutoBuildVariant.TacticalFireBow, new AutoBuildSequence(AutoBuildVariant.TacticalFireBow) },
            { AutoBuildVariant.TacticalHammer, new AutoBuildSequence(AutoBuildVariant.TacticalHammer) },
            { AutoBuildVariant.TacticalHammerKicker, new AutoBuildSequence(AutoBuildVariant.TacticalHammerKicker) },
            { AutoBuildVariant.TacticalFireSpear, new AutoBuildSequence(AutoBuildVariant.TacticalFireSpear) },
            { AutoBuildVariant.TacticalFireSpearKicker, new AutoBuildSequence(AutoBuildVariant.TacticalFireSpearKicker) },

            { AutoBuildVariant.Spectator, new AutoBuildSequence(AutoBuildVariant.Spectator) },
        };

        private BattleRoyaleManager m_Manager;

        private float m_TimeToUpdateState;

        private bool m_HasSelectedUpgrades;
        public bool ShouldSelectUpgrades => !m_HasSelectedUpgrades && m_Manager != null && m_Manager.state.TimeToGameStart < ((AutoBuildVariant)SelectedAutoBuildVariant == AutoBuildVariant.Random ? 15 : 9) && m_Manager.state.TimeToGameStart > 6;

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (firstPersonMover != null && firstPersonMover.IsMainPlayer())
                m_HasSelectedUpgrades = false;
        }

        private void Update()
        {
            if (!GameModeManager.IsBattleRoyale())
                return;

            if (Input.GetKeyDown(KeyCode.U))
            {
                if (m_HasSelectedUpgrades || !GameUIRoot.Instance || !GameUIRoot.Instance.UpgradeUI || !MultiplayerPlayerInfoManager.Instance || !GameUIRoot.Instance.UpgradeUI.gameObject.activeInHierarchy)
                    return;

                _ = StaticCoroutineRunner.StartStaticCoroutine(selectUpgradesCorouitine());
                m_HasSelectedUpgrades = true;
            }

            float time = Time.unscaledTime;
            if (time >= m_TimeToUpdateState)
            {
                m_TimeToUpdateState = time + 0.5f;
                m_Manager = BattleRoyaleManager.Instance;

                if (ShouldSelectUpgrades)
                {
                    _ = StaticCoroutineRunner.StartStaticCoroutine(selectUpgradesCorouitine());
                    m_HasSelectedUpgrades = true;
                }
            }
        }

        private IEnumerator selectUpgradesCorouitine()
        {
            AutoBuildVariant variant = (AutoBuildVariant)SelectedAutoBuildVariant;
            if (variant == AutoBuildVariant.None)
                yield break;

            if (!GameUIRoot.Instance || !GameUIRoot.Instance.UpgradeUI || !MultiplayerPlayerInfoManager.Instance || !GameUIRoot.Instance.UpgradeUI.gameObject.activeInHierarchy)
                yield break;

            UpgradeUI ui = GameUIRoot.Instance.UpgradeUI;
            if (variant == AutoBuildVariant.Random)
            {
                _ = ui.StartCoroutine(ui.CallPrivateMethod<IEnumerator>("selectRandomUpgrade"));
                yield break;
            }

            List<UpgradeUIIcon> icons = ui.GetPrivateField<List<UpgradeUIIcon>>("_icons");
            if (icons.IsNullOrEmpty())
                yield break;

            MultiplayerPlayerInfoState s = MultiplayerPlayerInfoManager.Instance.GetLocalPlayerInfoState();
            if (!s || s.IsDetached())
                yield break;

            string playerPlayfabID = s.state.PlayFabID;
            if (string.IsNullOrEmpty(playerPlayfabID))
                yield break;

            bool success = AutoUpgradeSequences.TryGetValue(variant, out AutoBuildSequence autoBuildSequence);
            if(!success || autoBuildSequence.Upgrades.IsNullOrEmpty())
                yield break;

            int currentUpgradeIndex = 0;
            while (currentUpgradeIndex < autoBuildSequence.Upgrades.Length)
            {
                AutoBuildUpgrade autoBuildUpgrade = autoBuildSequence.Upgrades[currentUpgradeIndex];
                if(autoBuildUpgrade == null)
                    break;

                UpgradeUIIcon icon = ui.GetUpgradeUIIcon(autoBuildUpgrade.Upgrade, autoBuildUpgrade.Level);
                if (icon && icon.GetCanUpgradeRightNow(playerPlayfabID))
                {
                    icon.OnButtonClicked();
                    yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                }
                currentUpgradeIndex++;
            }
            yield break;
        }

        public override string[] Commands() => null;
        public override string OnCommandRan(string[] command) => null;
    }
}