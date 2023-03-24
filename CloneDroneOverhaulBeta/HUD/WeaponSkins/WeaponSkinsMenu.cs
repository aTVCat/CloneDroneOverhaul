using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class WeaponSkinsMenu : OverhaulUI
    {
        public static readonly WeaponType[] SupportedWeapons = new WeaponType[]
        {
            WeaponType.Sword,
            WeaponType.Bow,
            WeaponType.Hammer,
            WeaponType.Spear
        };

        private static float m_TimeToAllowChangingSkins = 0f;
        private static float m_TimeChangedSkins = 0f;
        public static float GetSkinChangeCooldown() => 1f - Mathf.Clamp01((Time.unscaledTime - m_TimeChangedSkins) / (m_TimeToAllowChangingSkins - m_TimeChangedSkins));
        public static bool AllowChangingSkins() => Time.unscaledTime >= m_TimeToAllowChangingSkins;

        private IWeaponSkinItemDefinition[] m_Items;

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
            m.GetObject<Toggle>(7).onValueChanged.AddListener(SetAllowEnemiesUseSkins);

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

            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if(mover != null && mover.HasCharacterModel())
            {
                mover.GetCharacterModel().transform.GetChild(0).localEulerAngles = value ? new Vector3(0, 90, 0) : Vector3.zero;
            }

            base.gameObject.SetActive(value);
            ShowCursor = value;

            if (!value)
            {
                return;
            }

            MyModdedObject.GetObject<Toggle>(7).isOn = WeaponSkinsController.AllowEnemiesWearSkins;
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

            FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
            if(player != null)
            {
                WeaponType wType = player.GetEquippedWeaponType();
                if (SupportedWeapons.Contains(wType))
                {
                    PopulateSkins(wType);
                    return;
                }
            }

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

            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if (mover != null && mover.HasCharacterModel() && mover.HasWeapon(weaponType))
            {
                mover.SetEquippedWeaponType(weaponType);
            }

            WeaponSkinsMenuWeaponBehaviour.SelectSpecific(weaponType);
            TransformUtils.DestroyAllChildren(GetContainer(true));

            m_SelectedWeapon = weaponType;

            m_Items = m_Controller.Interface.GetSkinItems(ItemFilter.Everything);
            if (m_Items.IsNullOrEmpty())
            {
                return;
            }
            m_Items = m_Items.OrderBy(f => f.GetItemName()).ToArray();

            foreach(IWeaponSkinItemDefinition skin in m_Items)
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
                b.SetSkin(skinName, (skin as WeaponSkinItemDefinitionV2).AuthorDiscord, !string.IsNullOrEmpty(skin.GetExclusivePlayerID()));
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
            ShowSkinInfo(weaponType, skinName);

            OverhaulModdedPlayerInfo info = OverhaulModdedPlayerInfo.GetLocalPlayerInfo();
            if(info != null && info.HasReceivedData)
            {
                info.RefreshData();
            }

            if (GameModeManager.IsMultiplayer())
            {
                m_TimeChangedSkins = Time.unscaledTime;
                m_TimeToAllowChangingSkins = m_TimeChangedSkins + 2f;
            }
        }

        public void SetDefaultSkin()
        {
            if (m_SelectedWeapon == default)
            {
                return;
            }

            SelectSkin(m_SelectedWeapon, "Default");
        }

        public void ShowSkinInfo(WeaponType type, string skinName)
        {
            MyModdedObject.GetObject<Transform>(13).gameObject.SetActive(false);
            MyModdedObject.GetObject<Text>(8).text = skinName;
            if (type == WeaponType.None || string.IsNullOrEmpty(skinName))
            {
                return;
            }

            IWeaponSkinItemDefinition item = m_Controller.Interface.GetSkinItem(type, skinName, ItemFilter.Everything, out _);
            if(item == null)
            {
                return;
            }

            MyModdedObject.GetObject<Transform>(9).gameObject.SetActive(item.GetModel(false, false) != null);
            MyModdedObject.GetObject<Transform>(10).gameObject.SetActive(item.GetModel(false, true) != null || item.GetWeaponType() != WeaponType.Sword || (item as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer);
            MyModdedObject.GetObject<Transform>(11).gameObject.SetActive(item.GetModel(true, false) != null || item.GetWeaponType() == WeaponType.Bow);
            MyModdedObject.GetObject<Transform>(12).gameObject.SetActive(item.GetModel(true, true) != null || item.GetWeaponType() != WeaponType.Sword || item.GetWeaponType() == WeaponType.Bow || (item as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer);
            MyModdedObject.GetObject<Transform>(13).gameObject.SetActive(true);
        }

        public void SetAllowEnemiesUseSkins(bool value)
        {
            SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.EnemiesUseSkins", true), value);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetMenuActive(false);
            }
        }
    }
}