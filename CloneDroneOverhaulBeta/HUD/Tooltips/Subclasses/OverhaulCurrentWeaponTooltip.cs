using CDOverhaul.Gameplay;
using CDOverhaul.NetworkAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulCurrentWeaponTooltip : OverhaulTooltipWithPlayerData
    {
        public const string LASER_COLOR = "#AAD7FF";
        public const string FIRE_COLOR = "#FFB47D";

        public static string IconsDirectory => OverhaulMod.Core.ModDirectory + "Assets/Tooltips/Weapons/";
        private static readonly Dictionary<string, Texture> s_CachedIcons = new Dictionary<string, Texture>();

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
                string weaponString = weaponType.ToString();

                bool isFire = weaponSkins.IsFireVariant(weaponType, false);
                color = isFire ? FIRE_COLOR.ToColor() : LASER_COLOR.ToColor();
                if (isFire)
                    weaponString += "-Fire";
                weaponString += ".png";

                tryLoadIcon(weaponString);
            }
            SpawnedTooltipModdedObject.GetObject<Text>(1).color = color;

            refreshData(true);
        }

        private void tryLoadIcon(string fileName)
        {
            if (s_CachedIcons.ContainsKey(fileName))
            {
                SpawnedTooltipModdedObject.GetObject<RawImage>(0).texture = s_CachedIcons[fileName];
                return;
            }

            OverhaulDownloadInfo downloadInfo = new OverhaulDownloadInfo();
            downloadInfo.DoneAction = delegate
            {
                RawImage i = SpawnedTooltipModdedObject.GetObject<RawImage>(0);
                if (!i)
                    return;

                i.texture = downloadInfo.DownloadedTexture;
                if (!s_CachedIcons.ContainsKey(fileName))
                    s_CachedIcons.Add(fileName, downloadInfo.DownloadedTexture);
            };
            OverhaulNetworkAssetsController.DownloadTexture("file://" + IconsDirectory + fileName, downloadInfo);
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
