using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class WeaponSkinsMenuWeaponBehaviour : OverhaulBehaviour
    {
        #region Static

        private static readonly List<WeaponSkinsMenuWeaponBehaviour> m_InstantiatedButtons = new List<WeaponSkinsMenuWeaponBehaviour>();

        public static void SelectSpecific(WeaponType weaponType)
        {
            if (m_InstantiatedButtons.IsNullOrEmpty())
            {
                return;
            }

            foreach (WeaponSkinsMenuWeaponBehaviour b in m_InstantiatedButtons)
            {
                if (b.m_WeaponType == weaponType)
                {
                    b.SetSelected(true);
                    continue;
                }
                b.SetSelected(false);
            }
        }

        private static Vector3? m_NormalPosition;
        private static Vector3? m_SelectedPosition;

        #endregion

        private GameObject m_SelectedImage;
        private RectTransform m_TextTransform;
        private WeaponSkinsMenu m_SkinsMenu;
        private WeaponType m_WeaponType;

        private bool m_IsSelected;

        public override void Awake()
        {
            if (IsDisposedOrDestroyed())
                return;

            ModdedObject m = GetComponent<ModdedObject>();
            m_SelectedImage = m.GetObject<Transform>(0).gameObject;
            m_TextTransform = m.GetObject<RectTransform>(1);
            m_InstantiatedButtons.Add(this);

            if (m_NormalPosition == null)
            {
                m_NormalPosition = m_TextTransform.anchoredPosition;
                m_SelectedPosition = m_TextTransform.anchoredPosition + new Vector2(0, 2);
            }

            Button b = GetComponent<Button>();
            b.onClick.AddListener(SelectThis);
        }

        protected override void OnDisposed()
        {
            _ = m_InstantiatedButtons.Remove(this);
            m_SelectedImage = null;
            m_SkinsMenu = null;
            m_WeaponType = default;
            m_TextTransform = null;
        }

        public void SetMenu(WeaponSkinsMenu menu)
        {
            if (IsDisposedOrDestroyed() || m_SkinsMenu != null)
                return;

            m_SkinsMenu = menu;
        }

        public void SetWeaponType(WeaponType weaponType)
        {
            if (IsDisposedOrDestroyed() || m_WeaponType != default)
                return;

            m_WeaponType = weaponType;
        }

        public void SetSelected(bool value, bool initializing = false)
        {
            if (IsDisposedOrDestroyed() || (value == m_IsSelected && !initializing) || m_SelectedImage == null)
                return;

            m_SelectedImage.SetActive(value);
            m_IsSelected = value;
            SetTextTransformForState(value);
        }

        public void SetTextTransformForState(bool value)
        {
            if (IsDisposedOrDestroyed() || m_TextTransform == null || m_SelectedPosition == null || m_NormalPosition == null)
                return;

            if (value)
            {
                m_TextTransform.anchoredPosition = m_SelectedPosition.Value;
                return;
            }
            m_TextTransform.anchoredPosition = m_NormalPosition.Value;
        }

        public void SelectThis()
        {
            if (IsDisposedOrDestroyed() || m_IsSelected || m_SkinsMenu == null || m_SkinsMenu.IsPopulatingSkins)
                return;

            m_SkinsMenu.PopulateSkins(m_WeaponType);
        }
    }
}