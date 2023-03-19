using CDOverhaul.Gameplay;
using CDOverhaul.HUD.WeaponSkins;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class WeaponSkinsMenu : OverhaulUI
    {
        private IWeaponSkinItemDefinition[] m_CachedItems;

        private Hashtable m_HashtableTest;
        private WeaponSkinsController m_Controller;

        private WeaponType m_SelectedWeapon;

        public override void Initialize()
        {
            ModdedObject m = MyModdedObject;
            m_HashtableTest = new Hashtable
            {
                ["weapon"] = m.GetObject<ModdedObject>(2)
            };
            (m_HashtableTest["weapon"] as ModdedObject).gameObject.SetActive(false);
            m_HashtableTest["weaponsContainer"] = m.GetObject<Transform>(3);
            m_HashtableTest["weaponSkin"] = m.GetObject<ModdedObject>(0);
            (m_HashtableTest["weaponSkin"] as ModdedObject).gameObject.SetActive(false);
            m_HashtableTest["weaponsSkinsContainer"] = m.GetObject<Transform>(1);
            m.GetObject<Button>(6).onClick.AddListener(SetDefaultSkin);
            m.GetObject<Button>(4).onClick.AddListener(OnDoneButtonClicked);

            SetMenuActive(false);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();

            m_HashtableTest.Clear();
            m_HashtableTest = null;
        }

        private bool getController()
        {
            if (m_Controller != null)
            {
                return true;
            }
            m_Controller = GetController<WeaponSkinsController>();
            return m_Controller != null;
        }

        public ModdedObject GetPrefab(bool weaponSkin)
        {
            if (IsDisposedOrDestroyed())
            {
                return null;
            }

            if (weaponSkin)
            {
                return m_HashtableTest["weaponSkin"] as ModdedObject;
            }
            return m_HashtableTest["weapon"] as ModdedObject;
        }

        public Transform GetContainer(bool weaponSkins)
        {
            if (IsDisposedOrDestroyed())
            {
                return null;
            }

            if (weaponSkins)
            {
                return m_HashtableTest["weaponsSkinsContainer"] as Transform;
            }
            return m_HashtableTest["weaponsContainer"] as Transform;
        }

        public void SetMenuActive(bool value)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            base.gameObject.SetActive(value);
            ShowCursor = value;

            if (!value)
            {
                return;
            }

            PopulateWeapons();
        }

        public void OnDoneButtonClicked()
        {
            SetMenuActive(false);
        }

        public void PopulateWeapons()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            TransformUtils.DestroyAllChildren(GetContainer(false));

            PopulateWeapon(WeaponType.Sword);
            PopulateWeapon(WeaponType.Bow);
            PopulateWeapon(WeaponType.Hammer);
            PopulateWeapon(WeaponType.Spear);

            PopulateSkins(WeaponType.Sword);
        }

        public void PopulateWeapon(WeaponType weaponType)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            ModdedObject newPrefab = Instantiate<ModdedObject>(GetPrefab(false), GetContainer(false));
            newPrefab.gameObject.SetActive(true);
            newPrefab.GetObject<Text>(1).text = weaponType.ToString();
            WeaponSkinsMenuWeaponBehaviour b = newPrefab.gameObject.AddComponent<WeaponSkinsMenuWeaponBehaviour>();
            b.SetMenu(this);
            b.SetWeaponType(weaponType);
            b.SetSelected(false, true);
        }

        public void PopulateSkins(WeaponType weaponType)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (!getController())
            {
                return;
            }

            WeaponSkinsMenuWeaponBehaviour.SelectSpecific(weaponType);
            TransformUtils.DestroyAllChildren(GetContainer(true));

            m_SelectedWeapon = weaponType;

            if (m_CachedItems == null) m_CachedItems = m_Controller.Interface.GetSkinItems(ItemFilter.Everything);
            if (m_CachedItems.IsNullOrEmpty())
            {
                return;
            }

            foreach(IWeaponSkinItemDefinition skin in m_CachedItems)
            {
                if(skin.GetWeaponType() != weaponType)
                {
                    continue;
                }

                string skinName = skin.GetItemName();

                ModdedObject newPrefab = Instantiate<ModdedObject>(GetPrefab(true), GetContainer(true));
                newPrefab.gameObject.SetActive(true);
                newPrefab.GetObject<Text>(1).text = skinName;
                WeaponSkinsMenuSkinBehaviour b = newPrefab.gameObject.AddComponent<WeaponSkinsMenuSkinBehaviour>();
                b.SetMenu(this);
                b.SetWeaponType(weaponType);
                b.SetSkin(skinName);
                b.TrySelect();
            }
        }

        public void SelectSkin(WeaponType weaponType, string skinName)
        {
            switch (weaponType)
            {
                case WeaponType.Sword:
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.Sword", true), skinName);
                    break;
                case WeaponType.Bow:
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.Bow", true), skinName);
                    break;
                case WeaponType.Hammer:
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.Hammer", true), skinName);
                    break;
                case WeaponType.Spear:
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.Spear", true), skinName);
                    break;
            }

            WeaponSkinsMenuSkinBehaviour.SelectSpecific();

            if (!getController())
            {
                return;
            }
            m_Controller.ApplySkinsOnCharacter(CharacterTracker.Instance.GetPlayer());
        }

        public void SetDefaultSkin()
        {
            if (m_SelectedWeapon == default)
            {
                return;
            }

            SelectSkin(m_SelectedWeapon, "Default");
        }
    }
}