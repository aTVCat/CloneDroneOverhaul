using OverhaulMod.Gameplay;
using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class UpgradeModesManager : Singleton<UpgradeModesManager>, IGameLoadListener
    {
        public readonly Tuple<UpgradeType, int>[] WhitelistedUpgrades = new Tuple<UpgradeType, int>[] // Upgrade type - start level 
        {
            // sword
            new Tuple<UpgradeType, int>(UpgradeType.FireSword, 1),
            new Tuple<UpgradeType, int>(UpgradeType.BlockArrows, 1),

            // bow
            new Tuple<UpgradeType, int>(UpgradeType.ArrowWidth, 1),
            new Tuple<UpgradeType, int>(UpgradeType.FireArrow, 1),
            new Tuple<UpgradeType, int>(UpgradeType.AimTime, 1),

            // hammer
            new Tuple<UpgradeType, int>(UpgradeType.Hammer, 2),
            new Tuple<UpgradeType, int>(UpgradeType.FireHammer, 1),

            // spear
            new Tuple<UpgradeType, int>(UpgradeType.FireSpear, 1),
            new Tuple<UpgradeType, int>(UpgradeType.ShieldSize, 1),
            new Tuple<UpgradeType, int>(UpgradeType.ShieldBash, 1),

            // scythe
            new Tuple<UpgradeType, int>(ModUpgradesManager.SCYTHE_FIRE_UPGRADE, 1),
            new Tuple<UpgradeType, int>(ModUpgradesManager.SCYTHE_BLADE_UPGRADE, 1),

            // kick
            new Tuple<UpgradeType, int>(UpgradeType.KickPower, 1),
            new Tuple<UpgradeType, int>(UpgradeType.GetUp, 1),

            // energy
            new Tuple<UpgradeType, int>(UpgradeType.EnergyCapacity, 1),
            new Tuple<UpgradeType, int>(UpgradeType.EnergyRecharge, 1),

            // double jump
            new Tuple<UpgradeType, int>(ModUpgradesManager.DOUBLE_JUMP_UPGRADE, 1),

            // fire resistance
            new Tuple<UpgradeType, int>(UpgradeType.FireResistance, 1),

            // fire resistance
            new Tuple<UpgradeType, int>(UpgradeType.Jetpack, 1),
        };

        private UpgradeModes _mode;

        private UpgradeModeButtonController _buttonController;

        public void OnGameLoaded()
        {
            instantiateButton();
        }

        private void instantiateButton()
        {
            if (!_buttonController)
            {
                RectTransform spawnedButton = Instantiate(ModResources.Prefab(ModAssetBundles.UI, "RevertUpgradesButtonPrefab"), ModCache.UIRoot.UpgradeUI.ExitButton.transform).GetComponent<RectTransform>();
                spawnedButton.anchoredPosition = new Vector2(-35f, -4.15f);
                spawnedButton.sizeDelta = Vector2.one * 50f;
                spawnedButton.localEulerAngles = Vector3.zero;
                spawnedButton.localScale = Vector3.one;

                Button button = spawnedButton.GetComponent<Button>();
                button.onClick.AddListener(ToggleMode);
                UpgradeModeButtonController upgradeModeButtonController = spawnedButton.gameObject.AddComponent<UpgradeModeButtonController>();
                upgradeModeButtonController.InitializeAsElement();
                _buttonController = upgradeModeButtonController;
            }
            SetMode(UpgradeModes.Upgrade);
        }

        public bool CanRevertUpgrade(UpgradeType type, int level)
        {
            foreach (Tuple<UpgradeType, int> tuple in WhitelistedUpgrades)
            {
                if (tuple.Item1 == type && tuple.Item2 <= level)
                    return true;
            }
            return false;
        }

        public void ToggleMode()
        {
            SetMode(_mode == UpgradeModes.Upgrade ? UpgradeModes.RevertUpgrade : UpgradeModes.Upgrade);
        }

        public void SetMode(UpgradeModes upgradeMode)
        {
            _mode = upgradeMode;

            if (ModCache.UIRoot && ModCache.UIRoot.UpgradeUI && ModCache.UIRoot.UpgradeUI.gameObject.activeSelf)
                ModCache.UIRoot.UpgradeUI.PopulateIcons();

            UpgradeModeButtonController controller = _buttonController;
            if (!controller)
                return;

            if (upgradeMode == UpgradeModes.Upgrade)
            {
                controller.SetText(false);
                controller.SetSprite(ModResources.Sprite(ModAssetBundles.UI, "RevertUpgradesButton"));
                return;
            }
            controller.SetText(true);
            controller.SetSprite(ModResources.Sprite(ModAssetBundles.UI, "GetUpgradesButton"));
        }

        public UpgradeModes GetMode() => _mode;
    }
}
