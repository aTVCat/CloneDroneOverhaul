using CDOverhaul.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulCurrentWeaponTooltip : OverhaulTooltipWithPlayerData
    {
        public const string LASER_COLOR = "#D9EAFF";
        public const string FIRE_COLOR = "#FFDAC0";

        private FirstPersonMover m_Player;
        private WeaponType m_PlayerEquippedWeaponType;

        public override GameObject GetTooltipPrefab() => TooltipsController.TooltipsUI.GetDefaultPrefab();

        private void Update()
        {
            RefreshPlayer(delegate
            {
                if (!SpawnedTooltipModdedObject)
                    return;

                showTooltips(true);
            });

            if (!Player || !SpawnedTooltipModdedObject)
                return;

            Color color = Color.white;
            WeaponSkinsWearer weaponSkins = Player.GetComponent<WeaponSkinsWearer>();
            if (weaponSkins)
            {
                WeaponType weaponType = Player.GetEquippedWeaponType();
                if (WeaponSkinsController.IsWeaponSupported(weaponType))
                {
                    bool isFire = weaponSkins.IsFireVariant(weaponType);
                    if (isFire)
                        color = FIRE_COLOR.ConvertToColor();
                    else
                        color = LASER_COLOR.ConvertToColor();
                }
            }
            SpawnedTooltipModdedObject.GetObject<Text>(1).color = color;

            refreshData(true);
        }
        private void refreshData(bool showTooltipsIfNeeded)
        {
            if (!Player)
                return;

            WeaponType newWeaponType = Player.GetEquippedWeaponType();
            if (m_PlayerEquippedWeaponType != newWeaponType)
            {
                m_PlayerEquippedWeaponType = newWeaponType;
                if (showTooltipsIfNeeded)
                    showTooltips(false);
            }
        }

        private void showTooltips(bool refreshDataIfNeeded)
        {
            if (!SpawnedTooltipModdedObject)
                return;

            if (refreshDataIfNeeded)
                refreshData(false);

            SpawnedTooltipModdedObject.GetObject<Text>(1).text = m_PlayerEquippedWeaponType.ToString();
            ShowTooltipsContainer();
        }
    }
}
