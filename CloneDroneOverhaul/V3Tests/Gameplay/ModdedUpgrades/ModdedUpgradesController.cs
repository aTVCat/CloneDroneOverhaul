using CloneDroneOverhaul.V3Tests.Base;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class ModdedUpgradesController : V3_ModControllerBase
    {
        private List<OverhaulUpgradeDescription> _upgrades = new List<OverhaulUpgradeDescription>();

        private void Awake()
        {
            UpgradeManager.Instance.PlayerStartUpgrades[2].Level = 0;
        }

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (eventName == "overhaul.onAssetsLoadDone")
            {
                OverhaulMain.Timer.AddNoArgActionToCompleteNextFrame(delegate
                {
                    UpgradeInfo info1 = new UpgradeInfo("Sprint", UpgradeType.Dash, 0, null);
                    OverhaulUpgradeDescription result1 = AddUpgrade<OverhaulUpgradeDescription>(info1, true);

                    UpgradeManager.Instance.GetUpgrade(UpgradeType.Dash, 1).Requirement = result1;
                    UpgradeInfo info2 = new UpgradeInfo("Dash", UpgradeType.Dash, 1, null);
                    info2.SetRequirement(result1);
                    AddUpgrade(info2, true);

                    changeNullSpritesTooMissingIcons();
                });
            }
        }

        /// <summary>
        /// Add overhaul upgrade description to game
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="upgradeInfo"></param>
        /// <returns></returns>
        public T AddUpgrade<T>(in UpgradeInfo upgradeInfo, in bool forceAdd = false) where T : OverhaulUpgradeDescription
        {
            T result = null;
            bool cannotBeAdded = ModdedUpgradesController.HasUpgradeWithType(upgradeInfo.UpgradeType) && !forceAdd;

            if (!cannotBeAdded)
            {
                GameObject gm = new GameObject(upgradeInfo.UpgradeName.Replace(" ", string.Empty) + "_" + upgradeInfo.UpgradeType);
                result = gm.AddComponent<T>();
                result.UpgradeName = upgradeInfo.UpgradeName;
                result.UpgradeType = upgradeInfo.UpgradeType;
                result.SkillPointCostDefault = 1;
                result.Level = upgradeInfo.UpgradeLevel;
                result.Icon = upgradeInfo.UpgradeIcon;
                result.Description = upgradeInfo.UpgradeDescription;
                result.Requirement = upgradeInfo.Requiement;
                result.UpgradeInformation = upgradeInfo;
                gm.transform.SetParent(base.transform);

                _upgrades.Add(result);
                UpgradeManager.Instance.AddUpgrade(result, OverhaulMain.Instance);
            }
            return result;
        }

        /// <summary>
        /// Add overhaul upgrade description to game
        /// </summary>
        /// <param name="upgradeInfo"></param>
        public void AddUpgrade(in UpgradeInfo upgradeInfo, in bool forceAdd = false)
        {
            AddUpgrade<OverhaulUpgradeDescription>(upgradeInfo, forceAdd);
        }

        /// <summary>
        /// Check if we already have an upgrade with type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasUpgradeWithType(in UpgradeType type)
        {
            foreach (UpgradeDescription desc in UpgradeManager.Instance.UpgradeDescriptions)
            {
                if (desc.UpgradeType == type)
                {
                    return true;
                }
            }
            return false;
        }

        private static void changeNullSpritesTooMissingIcons()
        {
            foreach (UpgradeDescription desc in UpgradeManager.Instance.UpgradeDescriptions)
            {
                if (desc.Icon == null)
                {
                    desc.Icon = OverhaulUpgradeDescription.MissingSpriteIcon;
                }
            }
        }
    }
}