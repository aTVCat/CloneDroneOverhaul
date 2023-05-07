using ModLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AutoBuild : OverhaulGameplayController
    {
        [SettingDropdownParameters("None@Random@Random Sword@Random Bow@Random Hammer@Random Spear@Random Kicker@Full sword@Full bow@Full hammer@Full spear@Kicker@Light kicker@Armored kicker@Kick'N'Dash@Kick'N'Run@Tactical sword@Tactical fire sword@Jetpack sword@Tactical bow@Tactical fire bow@Tactical Fire Spear@Fire Spear Kicker@Tactical Hammer@Hammer Kicker@Spectator")]
        [OverhaulSetting("QoL.Last Bot Standing.Auto-Build", 0, false, null, null, null, null)]
        public static int SelectedAutoBuildVariant;

        private float m_TimeToRefreshVariables;

        private BattleRoyaleManager m_LBSManager;

        private bool m_HasSelectedUpgrades;
        public bool ShouldSelectUpgrades => !m_HasSelectedUpgrades && m_LBSManager != null && m_LBSManager.state.TimeToGameStart < ((AutoBuildVariant)SelectedAutoBuildVariant == AutoBuildVariant.Random ? 15 : 9) && m_LBSManager.state.TimeToGameStart > 6;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (firstPersonMover != null && hasInitializedModel)
            {
                if (firstPersonMover.IsMainPlayer())
                {
                    m_HasSelectedUpgrades = false;
                }
            }
        }

        private void Update()
        {
            if (!GameModeManager.IsBattleRoyale())
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                if (m_HasSelectedUpgrades || GameUIRoot.Instance == null || GameUIRoot.Instance.UpgradeUI == null || MultiplayerPlayerInfoManager.Instance == null || !GameUIRoot.Instance.UpgradeUI.gameObject.activeInHierarchy)
                {
                    return;
                }

                StaticCoroutineRunner.StartStaticCoroutine(selectUpgradesCorouitine());
                m_HasSelectedUpgrades = true;
            }

            float time = Time.unscaledTime;
            if (time >= m_TimeToRefreshVariables)
            {
                m_TimeToRefreshVariables = time + 0.5f;
                m_LBSManager = BattleRoyaleManager.Instance;

                if(ShouldSelectUpgrades)
                {
                    StaticCoroutineRunner.StartStaticCoroutine(selectUpgradesCorouitine());
                    m_HasSelectedUpgrades = true;
                }
            }
        }

        private IEnumerator selectUpgradesCorouitine()
        {
            if(GameUIRoot.Instance == null || GameUIRoot.Instance.UpgradeUI == null || MultiplayerPlayerInfoManager.Instance == null || !GameUIRoot.Instance.UpgradeUI.gameObject.activeInHierarchy)
            {
                yield break;
            }

            UpgradeUI ui = GameUIRoot.Instance.UpgradeUI;
            if((AutoBuildVariant)SelectedAutoBuildVariant == AutoBuildVariant.Random)
            {
                ui.StartCoroutine(ui.CallPrivateMethod<IEnumerator>("selectRandomUpgrade"));
                yield break;
            }

            List<UpgradeUIIcon> icons = ui.GetPrivateField<List<UpgradeUIIcon>>("_icons");
            if (icons.IsNullOrEmpty())
            {
                yield break;
            }

            MultiplayerPlayerInfoState s = MultiplayerPlayerInfoManager.Instance.GetLocalPlayerInfoState();
            if(s == null || s.IsDetached())
            {
                yield break;
            }

            string playerPlayfabID = s.state.PlayFabID;
            if (string.IsNullOrEmpty(playerPlayfabID))
            {
                yield break;
            }

            int i = 0;
            foreach(UpgradeUIIcon icon in icons)
            {
                if (i % 10 == 0)
                {
                    yield return null;
                }

                if (icon == null)
                {
                    i++;
                    continue;
                }

                UpgradeDescription desc = icon.GetPrivateField<UpgradeDescription>("_upgradeDescription");
                if(desc == null)
                {
                    i++;
                    continue;
                }

                if (icon.GetCanUpgradeRightNow(playerPlayfabID))
                {
                    yield return ApplyBuildCoroutine(desc, icon, playerPlayfabID);
                }

                i++;
            }

            yield break;
        }

        public IEnumerator ApplyBuildCoroutine(UpgradeDescription desc, UpgradeUIIcon icon, string playerPlayfabID)
        {
            AutoBuildVariant variant = (AutoBuildVariant)SelectedAutoBuildVariant;
            switch (variant)
            {
                case AutoBuildVariant.FullSword:
                    if ((desc.UpgradeType == UpgradeType.SwordUnlock) ||
                        (desc.UpgradeType == UpgradeType.FireSword && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.FireSword && desc.Level == 2))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.FullBow:
                    if ((desc.UpgradeType == UpgradeType.BowUnlock) ||
                        (desc.UpgradeType == UpgradeType.FireArrow && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.ArrowWidth && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.ArrowWidth && desc.Level == 2))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.FullHammer:
                    if ((desc.UpgradeType == UpgradeType.Hammer && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.Hammer && desc.Level == 2) ||
                        (desc.UpgradeType == UpgradeType.Hammer && desc.Level == 3))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.FullSpear:
                    if ((desc.UpgradeType == UpgradeType.SpearUnlock) ||
                        (desc.UpgradeType == UpgradeType.FireSpear && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.ShieldSize && desc.Level == 1))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.JetPackSword:
                    if ((desc.UpgradeType == UpgradeType.SwordUnlock) ||
                        (desc.UpgradeType == UpgradeType.Jetpack && desc.Level == 1))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.Kicker:
                    if ((desc.UpgradeType == UpgradeType.KickUnlock) ||
                        (desc.UpgradeType == UpgradeType.KickPower && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 2))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.KickNDash:
                    if ((desc.UpgradeType == UpgradeType.KickUnlock) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 2) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 3))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.KickNRun:
                    if ((desc.UpgradeType == UpgradeType.KickUnlock) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.Jetpack && desc.Level == 1))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.LightKicker:
                    if ((desc.UpgradeType == UpgradeType.KickUnlock) ||
                        (desc.UpgradeType == UpgradeType.GetUp && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 2))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.Spectator:
                    if ((desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 2) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 3) ||
                        (desc.UpgradeType == UpgradeType.EnergyRecharge && desc.Level == 1))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.TacticalSword:
                    if ((desc.UpgradeType == UpgradeType.SwordUnlock) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.KickUnlock))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.TacticalFireSword:
                    if ((desc.UpgradeType == UpgradeType.SwordUnlock) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.FireSword && desc.Level == 1))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.TacticalBow:
                    if ((desc.UpgradeType == UpgradeType.BowUnlock) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.KickUnlock) ||
                        (desc.UpgradeType == UpgradeType.ArrowWidth && desc.Level == 1))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.TacticalFireBow:
                    if ((desc.UpgradeType == UpgradeType.BowUnlock) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.KickUnlock) ||
                        (desc.UpgradeType == UpgradeType.FireArrow && desc.Level == 1))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.TacticalFireSpear:
                    if ((desc.UpgradeType == UpgradeType.SpearUnlock) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.FireSpear && desc.Level == 1))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.TacticalFireSpearKicker:
                    if ((desc.UpgradeType == UpgradeType.SpearUnlock) ||
                        (desc.UpgradeType == UpgradeType.KickUnlock) ||
                        (desc.UpgradeType == UpgradeType.FireSpear && desc.Level == 1))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.TacticalHammer:
                    if ((desc.UpgradeType == UpgradeType.Hammer && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.Hammer && desc.Level == 2) ||
                        (desc.UpgradeType == UpgradeType.EnergyCapacity && desc.Level == 1))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
                case AutoBuildVariant.TacticalHammerKicker:
                    if ((desc.UpgradeType == UpgradeType.Hammer && desc.Level == 1) ||
                        (desc.UpgradeType == UpgradeType.Hammer && desc.Level == 2) ||
                        (desc.UpgradeType == UpgradeType.KickUnlock))
                    {
                        icon.OnButtonClicked();
                        yield return new WaitUntil(() => !icon.GetCanUpgradeRightNow(playerPlayfabID));
                    }
                    break;
            }
            yield break;
        }

        public override string[] Commands() => null;
        public override string OnCommandRan(string[] command) => null;
    }
}