using CDOverhaul.Gameplay;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class WeaponSkinsMenuSkinBehaviour : OverhaulBehaviour
    {
        public const string Normal = "#1C6BFF";
        public const string Exclusive = "#1C6BFF";

        #region Static

        private static readonly List<WeaponSkinsMenuSkinBehaviour> m_InstantiatedButtons = new List<WeaponSkinsMenuSkinBehaviour>();

        public static void SelectSpecific()
        {
            if (m_InstantiatedButtons.IsNullOrEmpty())
            {
                return;
            }

            foreach (WeaponSkinsMenuSkinBehaviour b in m_InstantiatedButtons)
            {
                b.TrySelect();
            }
        }

        #endregion

        private GameObject m_SelectedImage;
        private WeaponSkinsMenu m_SkinsMenu;
        private string m_Skin;
        private WeaponType m_WeaponType;

        private GameObject m_ExclusiveIcon;
        private InputField m_Author;

        private bool m_IsSelected;

        public override void Awake()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            ModdedObject m = GetComponent<ModdedObject>();
            m_SelectedImage = m.GetObject<Transform>(0).gameObject;
            m_ExclusiveIcon = m.GetObject<Transform>(3).gameObject;
            m_Author = m.GetObject<InputField>(2);
            m_InstantiatedButtons.Add(this);

            Button b = GetComponent<Button>();
            b.onClick.AddListener(SelectThis);
        }

        protected override void OnDisposed()
        {
            _ = m_InstantiatedButtons.Remove(this);
            m_SelectedImage = null;
            m_SkinsMenu = null;
            m_Skin = null;
            m_Author = null;
            m_ExclusiveIcon = null;
        }

        public void SetMenu(WeaponSkinsMenu menu)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (m_SkinsMenu != null)
            {
                return;
            }
            m_SkinsMenu = menu;
        }

        public void SetSkin(string skin, string author, bool exclusive)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (m_Skin != null)
            {
                return;
            }
            m_Skin = skin;
            if (string.IsNullOrEmpty(author))
            {
                m_Author.text = string.Empty;
            }
            else
            {
                m_Author.text = "By " + author;
            }
            m_ExclusiveIcon.SetActive(exclusive);
        }

        public void SetWeaponType(WeaponType weaponType)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (m_WeaponType != default)
            {
                return;
            }
            m_WeaponType = weaponType;
        }

        public void SetSelected(bool value, bool initializing = false)
        {
            if (IsDisposedOrDestroyed() || (value == m_IsSelected && !initializing) || m_SelectedImage == null)
            {
                return;
            }

            m_SelectedImage.SetActive(value);
            m_IsSelected = value;
        }

        public void TrySelect()
        {
            switch (m_WeaponType)
            {
                case WeaponType.Sword:
                    SetSelected(WeaponSkinsController.EquippedSwordSkin == m_Skin);
                    break;
                case WeaponType.Bow:
                    SetSelected(WeaponSkinsController.EquippedBowSkin == m_Skin);
                    break;
                case WeaponType.Hammer:
                    SetSelected(WeaponSkinsController.EquippedHammerSkin == m_Skin);
                    break;
                case WeaponType.Spear:
                    SetSelected(WeaponSkinsController.EquippedSpearSkin == m_Skin);
                    break;
            }
        }

        public void SelectThis()
        {
            if (IsDisposedOrDestroyed() || m_IsSelected || m_SkinsMenu == null)
            {
                return;
            }

            m_SkinsMenu.SelectSkin(m_WeaponType, m_Skin);
        }
    }
}