using ModLibrary;
using UnityEngine;

namespace OverhaulAPI
{
    public static class UpgradesAdder
    {
        private static Transform m_UpgradesStorage;

        internal static void Init()
        {
            m_UpgradesStorage = new GameObject("OverhaulAPI UpgradesStorage").transform;
        }

        public static T AddUpgrade<T>(Mod mod, UpgradeType type, int level, string name, string description, Sprite sprite, UpgradeType requirement1 = UpgradeType.None, UpgradeType requirement2 = UpgradeType.None, int req1Lvl = 1, int req2Lvl = 1) where T : UpgradeDescription
        {
            Transform newUpgrade = new GameObject(mod.ModInfo.DisplayName + "_" + name + "_" + type).transform;
            newUpgrade.SetParent(m_UpgradesStorage);

            T upgrade = newUpgrade.gameObject.AddComponent<T>();
            upgrade.UpgradeName = name;
            upgrade.UpgradeType = type;
            upgrade.Description = description;
            upgrade.Icon = sprite;
            upgrade.Level = level;
            upgrade.Requirement = UpgradeManager.Instance.GetUpgrade(requirement1, req1Lvl);
            upgrade.Requirement2 = UpgradeManager.Instance.GetUpgrade(requirement2, req2Lvl);

            UpgradeManager.Instance.AddUpgrade(upgrade, mod);
            return upgrade;
        }
    }
}
