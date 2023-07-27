using CDOverhaul.NetworkAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulCurrentWeaponTooltip : OverhaulTooltip
    {
        public const string LASER_COLOR = "#AAD7FF";
        public const string FIRE_COLOR = "#FFB47D";

        public static string IconsDirectory => OverhaulMod.Core.ModDirectory + "Assets/Tooltips/Weapons/";
        private static readonly Dictionary<string, Texture> s_CachedIcons = new Dictionary<string, Texture>();

        private RawImage m_Icon;
        private Text m_Text;

        private bool m_IsLoadingIcon;

        public override void Initialize()
        {
            m_Icon = MyModdedObject.GetObject<RawImage>(0);
            m_Text = MyModdedObject.GetObject<Text>(1);
        }

        private void tryLoadIcon(string fileName)
        {
            if (m_IsLoadingIcon)
                return;

            if (s_CachedIcons.ContainsKey(fileName))
            {
                m_Icon.texture = s_CachedIcons[fileName];
                return;
            }

            m_IsLoadingIcon = true;
            OverhaulDownloadInfo downloadInfo = new OverhaulDownloadInfo();
            downloadInfo.DoneAction = delegate
            {
                m_IsLoadingIcon = false;
                if (downloadInfo.Error)
                {
                    m_Icon.texture = null;
                    return;
                }

                m_Icon.texture = downloadInfo.DownloadedTexture;
                if (!s_CachedIcons.ContainsKey(fileName))
                    s_CachedIcons.Add(fileName, downloadInfo.DownloadedTexture);
            };
            OverhaulNetworkAssetsController.DownloadTexture("file://" + IconsDirectory + fileName, downloadInfo);
        }

        public void ShowTooltip(WeaponType weaponType, bool isFire)
        {
            if (!MyModdedObject)
                return;

            Color color = isFire ? FIRE_COLOR.ToColor() : LASER_COLOR.ToColor();
            string weaponString = weaponType.ToString() + (isFire ? "-Fire" : string.Empty) + ".png";

            m_Text.color = color;
            m_Text.text = weaponType.ToString();
            tryLoadIcon(weaponString);
            Show();
        }
    }
}
