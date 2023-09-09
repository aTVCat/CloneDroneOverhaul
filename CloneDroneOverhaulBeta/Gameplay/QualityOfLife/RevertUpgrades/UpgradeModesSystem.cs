﻿using CDOverhaul.HUD;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class UpgradeModesSystem : OverhaulGameplaySystem
    {
        public const string REVERT_UPGRADES_COLOR = "#992125";
        public const string GET_UPGRADES_COLOR = "#269921";

        public static readonly Tuple<UpgradeType, int>[] UnrevertableUpgrades = new Tuple<UpgradeType, int>[]
        {
            new Tuple<UpgradeType, int>(UpgradeType.SwordUnlock, 1),
            new Tuple<UpgradeType, int>(UpgradeType.Hammer, 1),
            new Tuple<UpgradeType, int>(UpgradeType.SpearUnlock, 1),
            new Tuple<UpgradeType, int>(UpgradeType.BowUnlock, 1),
            new Tuple<UpgradeType, int>(UpgradeType.Armor, 0),
        };

        public static bool IsUnrevertableUpgrade(UpgradeType type, int level)
        {
            foreach (Tuple<UpgradeType, int> tuple in UnrevertableUpgrades)
            {
                if (tuple.Item1 == type && (level <= tuple.Item2))
                    return true;
            }
            return false;
        }

        public static UpgradeMode Mode
        {
            get;
            private set;
        }

        private Image m_ButtonGraphic;
        private Text m_ButtonText;

        public void PlaceButton()
        {
            RectTransform upgradeUITransform = GameUIRoot.Instance.UpgradeUI.transform as RectTransform;
            RectTransform centerHolderTransform = TransformUtils.FindChildRecursive(upgradeUITransform, "CenterHolder") as RectTransform;
            RectTransform iconContainerTransform = TransformUtils.FindChildRecursive(upgradeUITransform, "IconContainer") as RectTransform;

            OverhaulUIPanelScaler panelScaler = iconContainerTransform.GetComponent<OverhaulUIPanelScaler>();
            if (!panelScaler)
            {
                panelScaler = iconContainerTransform.gameObject.AddComponent<OverhaulUIPanelScaler>();
                panelScaler.Initialize(Vector3.one * 0.25f, Vector3.one, 15F, 3);
            }

            OverhaulUIPrefabs uiPrefabs = OverhaulUIManager.reference?.uiPrefabs;
            if (!m_ButtonGraphic && uiPrefabs)
            {
                RectTransform spawnedButton = uiPrefabs.InstantiatePrefab("UIPrefab_UpgradeUIModeToggle", centerHolderTransform, true).GetComponent<RectTransform>();
                spawnedButton.localPosition = new Vector2(270, 155);
                spawnedButton.localEulerAngles = Vector3.zero;
                spawnedButton.localScale = Vector3.one;
                _ = spawnedButton.gameObject.AddComponent<UpgradeModesButtonBehaviour>();

                Button button = spawnedButton.GetComponent<Button>();
                button.onClick.AddListener(ToggleMode);
                m_ButtonGraphic = spawnedButton.GetComponent<Image>();
                m_ButtonText = spawnedButton.GetChild(0).GetComponent<Text>();
            }
            SetMode(UpgradeMode.Upgrade);
        }

        public void ToggleMode()
        {
            SetMode(Mode == UpgradeMode.Upgrade ? UpgradeMode.RevertUpgrade : UpgradeMode.Upgrade);
        }

        public void SetMode(UpgradeMode upgradeMode)
        {
            Mode = upgradeMode;

            if (GameUIRoot.Instance && GameUIRoot.Instance.UpgradeUI && GameUIRoot.Instance.UpgradeUI.gameObject.activeSelf)
                GameUIRoot.Instance.UpgradeUI.PopulateIcons();

            if (!m_ButtonGraphic || !m_ButtonText)
                return;

            if (upgradeMode == UpgradeMode.Upgrade)
            {
                m_ButtonGraphic.color = REVERT_UPGRADES_COLOR.ToColor();
                m_ButtonText.text = OverhaulLocalizationManager.GetTranslation("Revert Upgrades");
                return;
            }
            m_ButtonGraphic.color = GET_UPGRADES_COLOR.ToColor();
            m_ButtonText.text = OverhaulLocalizationManager.GetTranslation("Get Upgrades");
        }
    }
}
