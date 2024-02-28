using ModLibrary;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Combat
{
    public class ModUpgradesManager : Singleton<ModUpgradesManager>, IGameLoadListener
    {
        public const UpgradeType SCYTHE_UNLOCK_UPGRADE = (UpgradeType)500;
        public const UpgradeType SCYTHE_FIRE_UPGRADE = (UpgradeType)501;

        public const UpgradeType HALBERD_UNLOCK_UPGRADE = (UpgradeType)510;
        public const UpgradeType HALBERD_FIRE_UPGRADE = (UpgradeType)511;

        public const UpgradeType AXE_UNLOCK_UPGRADE = (UpgradeType)520;
        public const UpgradeType AXE_FIRE_UPGRADE = (UpgradeType)521;

        public const UpgradeType DUAL_KNIVES_UNLOCK_UPGRADE = (UpgradeType)530;
        public const UpgradeType DUAL_KNIVES_FIRE_UPGRADE = (UpgradeType)531;

        public const UpgradeType HANDS_UNLOCK_UPGRADE = (UpgradeType)540;

        public const UpgradeType CLAWS_UNLOCK_UPGRADE = (UpgradeType)550;
        public const UpgradeType CLAWS_FIRE_UPGRADE = (UpgradeType)551;

        public const UpgradeType LASER_BLASTER_UPGRADE = (UpgradeType)560;

        public const UpgradeType BOOMERANG_UNLOCK_UPGRADE = (UpgradeType)570;
        public const UpgradeType BOOMERANG_FIRE_UPGRADE = (UpgradeType)571;

        public const UpgradeType DOUBLE_JUMP_UPGRADE = (UpgradeType)580;

        private List<UpgradeDescription> m_upgrades;

        private Dictionary<(UpgradeType, int), Vector2> m_sizeDeltaOverrides;

        private Transform m_upgradesObjectTransform;

        public override void Awake()
        {
            base.Awake();

            m_upgrades = new List<UpgradeDescription>();
            m_sizeDeltaOverrides = new Dictionary<(UpgradeType, int), Vector2>();

            Transform transform = new GameObject("Upgrades").transform;
            transform.SetParent(base.transform);
            m_upgradesObjectTransform = transform;

            createUpgrades();
        }

        private void OnDestroy()
        {
            if (!m_upgrades.IsNullOrEmpty())
                foreach (UpgradeDescription upgradeDescription in m_upgrades)
                    if (upgradeDescription)
                    {
                        _ = UpgradeManager.Instance.UpgradeDescriptions.Remove(upgradeDescription);

                        if (upgradeDescription.Icon)
                            Destroy(upgradeDescription.Icon);

                        Destroy(upgradeDescription);
                    }
        }

        public void OnGameLoaded()
        {
            AddUpgrades();
        }

        private void createUpgrades()
        {
            UpgradeDescription scythe = CreateUpgrade<UpgradeDescription>("Scythe unlock",
                "You get a scythe!",
                SCYTHE_UNLOCK_UPGRADE,
                1,
                AssetBundleConstants.UPGRADES,
                "Scythe-128x128");
            _ = CreateUpgrade<UpgradeDescription>("Fire scythe",
                  "Your scythe is ON FIRE!",
                  SCYTHE_FIRE_UPGRADE,
                  1,
                  AssetBundleConstants.UPGRADES,
                  "Scythe-128x128",
                  scythe);

            _ = CreateUpgrade<UpgradeDescription>("Double jump",
                "Do second jump",
                DOUBLE_JUMP_UPGRADE,
                1);

            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.AllGameplayContent))
            {
                UpgradeDescription battleAxe = CreateUpgrade<UpgradeDescription>("Battle axe unlock",
                    "You get a battle axe!",
                    AXE_UNLOCK_UPGRADE,
                    1,
                    AssetBundleConstants.UPGRADES,
                    "Axe-128x128");
                _ = CreateUpgrade<UpgradeDescription>("Fire battle axe",
                      "Your battle axe gets fire!",
                      AXE_FIRE_UPGRADE,
                      1,
                      AssetBundleConstants.UPGRADES,
                      "Axe-128x128",
                      battleAxe);

                UpgradeDescription halberd = CreateUpgrade<UpgradeDescription>("Halberd unlock",
                    "You get a halberd!",
                    HALBERD_UNLOCK_UPGRADE,
                    1,
                    AssetBundleConstants.UPGRADES,
                    "Halberd-128x128");
                _ = CreateUpgrade<UpgradeDescription>("Fire halberd",
                      "Your halberd gets fire!",
                      HALBERD_FIRE_UPGRADE,
                      1,
                      AssetBundleConstants.UPGRADES,
                      "Halberd-128x128",
                      halberd);

                UpgradeDescription dualKnifes = CreateUpgrade<UpgradeDescription>("Dual knifes unlock",
                    "Made for chibi sword robot",
                    DUAL_KNIVES_UNLOCK_UPGRADE,
                    1,
                    AssetBundleConstants.UPGRADES,
                    "DKnives-128x128");
                _ = CreateUpgrade<UpgradeDescription>("Fire dual knives",
                      "Made for chibi sword robot",
                      DUAL_KNIVES_FIRE_UPGRADE,
                      1,
                      AssetBundleConstants.UPGRADES,
                      "DKnives-128x128",
                      dualKnifes);

                UpgradeDescription claws = CreateUpgrade<UpgradeDescription>("Claws unlock",
                    "you are not a zombie",
                    CLAWS_UNLOCK_UPGRADE,
                    1,
                    AssetBundleConstants.UPGRADES,
                    "Claws-128x128");
                _ = CreateUpgrade<UpgradeDescription>("Fire claws",
                      "hmm",
                      CLAWS_FIRE_UPGRADE,
                      1,
                      AssetBundleConstants.UPGRADES,
                      "Claws-128x128",
                      claws);

                _ = CreateUpgrade<UpgradeDescription>("Mounted laser blaster",
                    "pew pew",
                    LASER_BLASTER_UPGRADE,
                    1,
                    AssetBundleConstants.UPGRADES,
                    "MBlaster-128x128");

                _ = CreateUpgrade<UpgradeDescription>("Hands unlock",
                    "Literally hands",
                    HANDS_UNLOCK_UPGRADE,
                    1,
                    AssetBundleConstants.UPGRADES,
                    "Hands-128x128");

                _ = CreateUpgrade<UpgradeDescription>("Boomerang unlock",
                    "An unique weapon",
                    BOOMERANG_UNLOCK_UPGRADE,
                    1,
                    AssetBundleConstants.UPGRADES,
                    "Boomerang-128x128",
                    CreateUpgrade<UpgradeDescription>("Fire boomerang",
                      "hmm",
                      BOOMERANG_FIRE_UPGRADE,
                      1,
                      AssetBundleConstants.UPGRADES,
                      "FireBoomerang-128x128"));
            }

            OverrideSizeDeltaForUpgrade(SCYTHE_UNLOCK_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(SCYTHE_FIRE_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(AXE_UNLOCK_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(AXE_FIRE_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(HALBERD_UNLOCK_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(HALBERD_FIRE_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(DUAL_KNIVES_UNLOCK_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(DUAL_KNIVES_FIRE_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(CLAWS_UNLOCK_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(CLAWS_FIRE_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(LASER_BLASTER_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(HANDS_UNLOCK_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(BOOMERANG_UNLOCK_UPGRADE, 1, Vector2.zero);
            OverrideSizeDeltaForUpgrade(BOOMERANG_FIRE_UPGRADE, 1, Vector2.zero);
        }

        public void AddUpgrades()
        {
            Mod mod = ModCore.instance;
            UpgradeManager upgradeManager = UpgradeManager.Instance;
            foreach (UpgradeDescription upgradeDescription in m_upgrades)
                if (!upgradeManager.HasUpgrade(upgradeDescription.UpgradeType, upgradeDescription.Level))
                    UpgradeManager.Instance.AddUpgrade(upgradeDescription, mod);
        }

        public T CreateUpgrade<T>(string displayName, string description, UpgradeType upgradeType, int level = 1, string iconBundle = null, string iconAsset = null, UpgradeDescription r1 = null, UpgradeDescription r2 = null) where T : UpgradeDescription
        {
            GameObject gameObject = new GameObject($"{displayName} {level}");
            gameObject.transform.SetParent(m_upgradesObjectTransform);

            UpgradeDescription upgradeDescription = gameObject.AddComponent<T>();
            upgradeDescription.UpgradeName = displayName;
            upgradeDescription.Description = description;
            upgradeDescription.UpgradeType = upgradeType;
            upgradeDescription.Level = level;
            upgradeDescription.Requirement = r1;
            upgradeDescription.Requirement2 = r2;
            if (!iconBundle.IsNullOrEmpty() && !iconAsset.IsNullOrEmpty())
                upgradeDescription.Icon = ModResources.Load<Sprite>(iconBundle, iconAsset);

            m_upgrades.Add(upgradeDescription);
            return (T)upgradeDescription;
        }

        public void OverrideSizeDeltaForUpgrade(UpgradeType upgradeType, int level, Vector2 size)
        {
            m_sizeDeltaOverrides.Add((upgradeType, level), size);
        }

        public Vector2 GetOverrideSizeDeltaForUpgrade(UpgradeType upgradeType, int level)
        {
            foreach (KeyValuePair<(UpgradeType, int), Vector2> keyValue in m_sizeDeltaOverrides)
                if (keyValue.Key.Item1 == upgradeType && keyValue.Key.Item2 == level)
                    return keyValue.Value;

            return Vector2.one * -16f;
        }
    }
}
