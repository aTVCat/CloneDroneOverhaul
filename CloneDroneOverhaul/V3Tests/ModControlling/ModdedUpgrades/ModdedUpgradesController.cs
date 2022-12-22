using UnityEngine;
using ModLibrary;
using System.Collections.Generic;

namespace CloneDroneOverhaul.V3Tests.Base
{
    public class ModdedUpgradesController : V3_ModControllerBase
    {
        private List<OverhaulUpgradeDescription> _upgrades = new List<OverhaulUpgradeDescription>();

        public override void OnEvent(in string eventName, in object[] args)
        {
            if(eventName == "overhaul.onAssetsLoadDone")
            {
                if (!OverhaulDescription.TEST_FEATURES_ENABLED)
                {
                    return;
                }
                UpgradeInfo info1 = new UpgradeInfo("TestUpgrade", UpgradeInfo.DEFAULT_UPGRADE_TYPE + 1, null);
                AddUpgrade(info1);
            }
        }

        /// <summary>
        /// Add overhaul upgrade description to game
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="upgradeInfo"></param>
        /// <returns></returns>
        public T AddUpgrade<T>(in UpgradeInfo upgradeInfo) where T : OverhaulUpgradeDescription
        {
            T result = null;
            bool cannotBeAdded = ModdedUpgradesController.HasUpgradeWithType(upgradeInfo.UpgradeType);

            if (!cannotBeAdded)
            {
                GameObject gm = new GameObject(upgradeInfo.UpgradeName.Replace(" ", string.Empty) + "_" + upgradeInfo.UpgradeType);
                result = gm.AddComponent<T>();
                result.UpgradeName = upgradeInfo.UpgradeName;
                result.UpgradeType = upgradeInfo.UpgradeType;
                result.SkillPointCostDefault = 1;
                result.Level = 1;
                result.IsUpgradeVisible = true;
                result.Icon = upgradeInfo.UpgradeIcon;
                result.Description = upgradeInfo.UpgradeDescription;
                result.UpgradeInformation = upgradeInfo;

                _upgrades.Add(result);
                UpgradeManager.Instance.AddUpgrade(result, OverhaulMain.Instance);
            }
            return result;
        }

        /// <summary>
        /// Add overhaul upgrade description to game
        /// </summary>
        /// <param name="upgradeInfo"></param>
        public void AddUpgrade(in UpgradeInfo upgradeInfo)
        {
            AddUpgrade<OverhaulUpgradeDescription>(upgradeInfo);
        }

        /// <summary>
        /// Check if we already have an upgrade with type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasUpgradeWithType(in UpgradeType type)
        {
            foreach(UpgradeDescription desc in UpgradeManager.Instance.UpgradeDescriptions)
            {
                if(desc.UpgradeType == type) return true;
            }
            return false;
        }
    }
}