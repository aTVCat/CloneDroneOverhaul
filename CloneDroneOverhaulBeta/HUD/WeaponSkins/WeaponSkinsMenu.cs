using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Patches;
using OverhaulAPI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    /// <summary>
    /// Used for outfits and skins menus
    /// </summary>
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
        public static float GetSkinChangeCooldown()
        {
            return 1f - Mathf.Clamp01((Time.unscaledTime - m_TimeChangedSkins) / (m_TimeToAllowChangingSkins - m_TimeChangedSkins));
        }

        public static bool AllowChangingSkins()
        {
            return Time.unscaledTime >= m_TimeToAllowChangingSkins;
        }

        public static void StartCooldown()
        {
            if (GameModeManager.IsMultiplayer())
            {
                m_TimeChangedSkins = Time.unscaledTime;
                m_TimeToAllowChangingSkins = m_TimeChangedSkins + 2f;
            }
        }

        private IWeaponSkinItemDefinition[] m_Items;
        private List<AccessoryItem> m_AccessoryItems;
        private WeaponSkinsController m_Controller;

        private Hashtable m_HashtableTest;
        private Text m_TextPrefab;

        private Text m_Description;
        private WeaponType m_SelectedWeapon;
        private Button m_DefaultSkinButton;
        private ScrollRect m_ScrollRect;
        private CanvasGroup m_ScrollRectCanvasGroup;

        private Transform m_LoadIndicatorTransform;
        private Image m_LoadIndicatorFill;

        private Button m_DebugApplyButton;
        private Button m_DebugSaveButton;
        private Dropdown m_DebugCharacterModelsDropdown;

        public bool IsOutfitSelection;
        public static WeaponSkinsMenu SkinsSelection;
        public static WeaponSkinsMenu OutfitSelection;

        public bool IsPopulatingSkins { get; private set; }

        public override void Initialize()
        {
            ModdedObject m = MyModdedObject;
            m_ScrollRect = m.GetObject<ScrollRect>(17);
            m_ScrollRectCanvasGroup = m.GetObject<CanvasGroup>(17);
            m_HashtableTest = new Hashtable
            {
                ["weapon"] = m.GetObject<ModdedObject>(2)
            };
            (m_HashtableTest["weapon"] as ModdedObject).gameObject.SetActive(false);
            m_HashtableTest["weaponsContainer"] = m.GetObject<Transform>(3);
            m_HashtableTest["weaponSkin"] = m.GetObject<ModdedObject>(0);
            (m_HashtableTest["weaponSkin"] as ModdedObject).gameObject.SetActive(false);
            m_HashtableTest["weaponsSkinsContainer"] = m.GetObject<Transform>(1);
            m_TextPrefab = m.GetObject<Text>(14);
            m_TextPrefab.gameObject.SetActive(false);
            m_Description = m.GetObject<Text>(15);
            m_Description.text = string.Empty;
            m_DefaultSkinButton = m.GetObject<Button>(6);
            m_DefaultSkinButton.onClick.AddListener(SetDefaultSkin);
            m_LoadIndicatorTransform = m.GetObject<Transform>(18);
            m_LoadIndicatorFill = m.GetObject<Image>(19);
            m.GetObject<Button>(4).onClick.AddListener(OnDoneButtonClicked);
            m.GetObject<Toggle>(7).onValueChanged.AddListener(SetAllowEnemiesUseSkins);
            MyModdedObject.GetObject<Text>(8).text = string.Empty;

            if (base.gameObject.name.Equals("SkinsSelection"))
            {
                SkinsSelection = this;
                _ = OverhaulEventsController.AddEventListener(EscMenuReplacement.OpenSkinsFromSettingsEventString, OpenMenuFromSettings);
            }
            else
            {
                m_AccessoryItems = new List<AccessoryItem>();
                OutfitSelection = this;
                _ = OverhaulEventsController.AddEventListener(EscMenuReplacement.OpenOutfitsFromSettingsEventString, OpenMenuFromSettings);

                if (OverhaulVersion.IsDebugBuild)
                {
                    m_DebugSaveButton = m.GetObject<Button>(28);
                    m_DebugSaveButton.onClick.AddListener(delegate
                    {
                        if (OutfitsController.EditingItem == null || string.IsNullOrEmpty(OutfitsController.EditingCharacterModel))
                        {
                            return;
                        }
                        OutfitsController.EditingItem.SaveOffsets();
                    });
                    m_DebugApplyButton = m.GetObject<Button>(29);
                    m_DebugApplyButton.onClick.AddListener(delegate
                    {
                        if (OutfitsController.EditingItem == null || string.IsNullOrEmpty(OutfitsController.EditingCharacterModel))
                        {
                            return;
                        }

                        ModelOffset off = OutfitsController.EditingItem.Offsets[OutfitsController.EditingCharacterModel];
                        try
                        {
                            off.OffsetPosition = new Vector3(float.Parse(MyModdedObject.GetObject<InputField>(19).text), float.Parse(MyModdedObject.GetObject<InputField>(20).text), float.Parse(MyModdedObject.GetObject<InputField>(21).text));
                            off.OffsetEulerAngles = new Vector3(float.Parse(MyModdedObject.GetObject<InputField>(22).text), float.Parse(MyModdedObject.GetObject<InputField>(23).text), float.Parse(MyModdedObject.GetObject<InputField>(24).text));
                            off.OffsetLocalScale = new Vector3(float.Parse(MyModdedObject.GetObject<InputField>(25).text), float.Parse(MyModdedObject.GetObject<InputField>(26).text), float.Parse(MyModdedObject.GetObject<InputField>(27).text));
                        }
                        catch
                        {

                        }
                        FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
                        if (player != null)
                        {
                            OutfitsWearer ow = player.GetComponent<OutfitsWearer>();
                            if (ow != null)
                            {
                                ow.SpawnAccessories();
                            }
                        }
                    });

                    DelegateScheduler.Instance.Schedule(delegate
                    {
                        m_DebugCharacterModelsDropdown = m.GetObject<Dropdown>(30);
                        m_DebugCharacterModelsDropdown.options = MultiplayerCharacterCustomizationManager.Instance.GetCharacterModelDropdownOptions(true);
                        m_DebugCharacterModelsDropdown.onValueChanged.AddListener(delegate (int index)
                        {
                            if (OutfitsController.EditingItem == null || string.IsNullOrEmpty(OutfitsController.EditingCharacterModel))
                            {
                                return;
                            }

                            SettingsManager.Instance.SetMultiplayerCharacterModelIndex(index);
                            SettingsManager.Instance.SetUseSkinInSingleplayer(true);

                            Vector3 pos = CharacterTracker.Instance.GetPlayerRobot().transform.position;
                            CharacterTracker.Instance.DestroyExistingPlayer();

                            GameObject gm = new GameObject("DebugSpawnpoint");
                            gm.transform.position = pos;
                            FirstPersonMover newPlayer = GameFlowManager.Instance.SpawnPlayer(gm.transform, true, true, null);
                            OutfitsController.EditingCharacterModel = MultiplayerCharacterCustomizationManager.Instance.CharacterModels[index].CharacterModelPrefab.gameObject.name + "(Clone)";
                            DebugSetInputFieldsValues(OutfitsController.EditingItem.Offsets[OutfitsController.EditingCharacterModel]);
                            Destroy(gm);
                        });
                    }, 0.5f);

                    m.GetObject<Button>(31).onClick.AddListener(delegate
                    {
                        if (OutfitsController.EditingItem == null || string.IsNullOrEmpty(OutfitsController.EditingCharacterModel))
                        {
                            return;
                        }

                        OutfitsController.CopiedModelOffset = OutfitsController.EditingItem.Offsets[OutfitsController.EditingCharacterModel];
                    });

                    m.GetObject<Button>(32).onClick.AddListener(delegate
                    {
                        if (OutfitsController.EditingItem == null || string.IsNullOrEmpty(OutfitsController.EditingCharacterModel) || OutfitsController.CopiedModelOffset == null)
                        {
                            return;
                        }

                        ModelOffset edititngOffset = OutfitsController.EditingItem.Offsets[OutfitsController.EditingCharacterModel];
                        edititngOffset.OffsetPosition = OutfitsController.CopiedModelOffset.OffsetPosition;
                        edititngOffset.OffsetEulerAngles = OutfitsController.CopiedModelOffset.OffsetEulerAngles;
                        edititngOffset.OffsetLocalScale = OutfitsController.CopiedModelOffset.OffsetLocalScale;
                        DebugSetInputFieldsValues(edititngOffset);

                        FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
                        if (player != null)
                        {
                            OutfitsWearer ow = player.GetComponent<OutfitsWearer>();
                            if (ow != null)
                            {
                                ow.SpawnAccessories();
                            }
                        }
                    });
                }
                else
                {
                    m.GetObject<Transform>(18).gameObject.SetActive(false);
                }
            }

            SetMenuActive(false);
        }

        public void OpenMenuFromSettings()
        {
            if (GameUIRoot.Instance == null)
            {
                return;
            }

            OverhaulPauseMenu menu = GetController<OverhaulPauseMenu>();
            OverhaulParametersMenu paramsMenu = GetController<OverhaulParametersMenu>();
            EscMenu escMenu = GameUIRoot.Instance.EscMenu;
            if (menu == null || paramsMenu == null || escMenu == null)
            {
                return;
            }

            menu.ScheduleHide = true;
            menu.Hide();
            escMenu.Hide();
            paramsMenu.Hide();
            SetMenuActive(true);
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
            return IsDisposedOrDestroyed()
                ? null
                : weaponSkin ? m_HashtableTest["weaponSkin"] as ModdedObject : m_HashtableTest["weapon"] as ModdedObject;
        }

        public Transform GetContainer(bool weaponSkins)
        {
            return IsDisposedOrDestroyed()
                ? null
                : weaponSkins ? m_HashtableTest["weaponsSkinsContainer"] as Transform : m_HashtableTest["weaponsContainer"] as Transform;
        }

        public void SetMenuActive(bool value)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            TransformUtils.DestroyAllChildren(GetContainer(true));
            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if (mover != null && mover.HasCharacterModel())
            {
                if (!GameModeManager.IsMultiplayer()) mover.InstantlySetTorsoTiltX(0);
                mover.GetCharacterModel().transform.GetChild(0).localEulerAngles = IsOutfitSelection ? value ? new Vector3(0, 180, 0) : Vector3.zero : value ? new Vector3(0, 90, 0) : Vector3.zero;
            }

            if (IsOutfitSelection)
            {
                PlayerStatusBehaviour.SetOwnStatus(value ? PlayerStatusType.EquippingAccessories : PlayerStatusType.Idle);
                if (value)
                {
                    PopulateSkins(WeaponType.None);
                }
            }
            else
            {
                PlayerStatusBehaviour.SetOwnStatus(value ? PlayerStatusType.SwitchingSkins : PlayerStatusType.Idle);
            }

            if (CharacterTracker.Instance != null && CharacterTracker.Instance.GetPlayer() != null)
            {
                if (value && GameUIRoot.Instance != null && GameUIRoot.Instance.CloneUI != null) GameUIRoot.Instance.CloneUI.Hide();
                if (!value && GameUIRoot.Instance != null && GameUIRoot.Instance.CloneUI != null && !SettingsManager.Instance.ShouldHideGameUI()) GameUIRoot.Instance.CloneUI.Show();
            }

            base.enabled = true;
            base.gameObject.SetActive(value);
            ShowCursor = value;

            if (!value || IsOutfitSelection)
            {
                return;
            }
            MyModdedObject.GetObject<Toggle>(7).isOn = WeaponSkinsController.AllowEnemiesWearSkins;
            PopulateWeapons();
        }

        public void RefreshDefaultSkinButton()
        {
            switch (m_SelectedWeapon)
            {
                case WeaponType.Sword:
                    m_DefaultSkinButton.interactable = !WeaponSkinsController.EquippedSwordSkin.Equals("Default");
                    break;
                case WeaponType.Bow:
                    m_DefaultSkinButton.interactable = !WeaponSkinsController.EquippedBowSkin.Equals("Default");
                    break;
                case WeaponType.Hammer:
                    m_DefaultSkinButton.interactable = !WeaponSkinsController.EquippedHammerSkin.Equals("Default");
                    break;
                case WeaponType.Spear:
                    m_DefaultSkinButton.interactable = !WeaponSkinsController.EquippedSpearSkin.Equals("Default");
                    break;
            }
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
            if (player != null)
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
            newPrefab.GetObject<Text>(1).text = LocalizationManager.Instance.GetTranslatedString(weaponType.ToString());
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

            if (!IsOutfitSelection)
            {
                FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
                if (mover != null && mover.HasCharacterModel() && mover.HasWeapon(weaponType))
                {
                    if (!GameModeManager.IsMultiplayer())
                    {
                        mover.SetEquippedWeaponType(weaponType);
                    }
                }
            }

            if (!IsOutfitSelection) WeaponSkinsMenuWeaponBehaviour.SelectSpecific(weaponType);
            _ = StaticCoroutineRunner.StartStaticCoroutine(populateSkinsCoroutine(weaponType));
        }

        private IEnumerator endPopulatingSkinsCoroutine()
        {
            IsPopulatingSkins = false;
            m_ScrollRectCanvasGroup.alpha = 0f;
            m_ScrollRectCanvasGroup.blocksRaycasts = true;
            for (int i = 0; i < 4; i++)
            {
                m_ScrollRectCanvasGroup.alpha += 0.25f;
                yield return new WaitForSecondsRealtime(0.016f);
            }
            m_ScrollRectCanvasGroup.alpha = 1f;
        }

        private IEnumerator populateSkinsCoroutine(WeaponType weaponType)
        {
            StaticCoroutineRunner.StopStaticCoroutine(endPopulatingSkinsCoroutine());
            SetFillProgress(0f);
            IsPopulatingSkins = true;
            m_DefaultSkinButton.interactable = false;
            yield return null;

            m_ScrollRectCanvasGroup.alpha = 1f;
            m_ScrollRectCanvasGroup.blocksRaycasts = false;
            for (int i = 0; i < 4; i++)
            {
                m_ScrollRectCanvasGroup.alpha -= 0.25f;
                yield return new WaitForSecondsRealtime(0.016f);
            }
            m_ScrollRectCanvasGroup.alpha = 0f;
            yield return null;

            TransformUtils.DestroyAllChildren(GetContainer(true));
            m_ScrollRect.verticalNormalizedPosition = 1f;
            int itemsSpawned = 0;

            if (IsOutfitSelection)
            {
                m_AccessoryItems = OutfitsController.AllAccessories;
                if (m_AccessoryItems.IsNullOrEmpty())
                {
                    yield return StaticCoroutineRunner.StartStaticCoroutine(endPopulatingSkinsCoroutine());
                    yield break;
                }
                m_AccessoryItems = m_AccessoryItems.OrderBy(f => f.Name).ToList();

                foreach (AccessoryItem aitem in m_AccessoryItems)
                {
                    if (!base.gameObject.activeSelf)
                    {
                        IsPopulatingSkins = false;
                        yield break;
                    }
                    string itemName = aitem.Name;

                    ModdedObject newPrefab = Instantiate<ModdedObject>(GetPrefab(true), GetContainer(true));
                    newPrefab.gameObject.SetActive(true);
                    newPrefab.GetObject<Text>(1).text = itemName;
                    newPrefab.GetComponent<Button>().interactable = aitem.IsUnlocked();
                    newPrefab.GetComponent<Animation>().enabled = !string.IsNullOrEmpty(aitem.AllowedPlayers);
                    WeaponSkinsMenuSkinBehaviour b = newPrefab.gameObject.AddComponent<WeaponSkinsMenuSkinBehaviour>();
                    b.IsOutfitSelection = true;
                    b.Initialize();
                    b.SetMenu(this);
                    b.SetSkin(itemName, aitem.Author, !string.IsNullOrEmpty(aitem.AllowedPlayers));
                    b.TrySelect();

                    itemsSpawned++;
                    SetFillProgress(itemsSpawned / (float)m_AccessoryItems.Count);
                    yield return null;
                }

                yield return StaticCoroutineRunner.StartStaticCoroutine(endPopulatingSkinsCoroutine());
                yield break;
            }

            m_SelectedWeapon = weaponType;
            if (m_SelectedWeapon.Equals(WeaponType.Bow) && !OverhaulGamemodeManager.SupportsBowSkins())
            {
                Text newPrefab = Instantiate<Text>(m_TextPrefab, GetContainer(true));
                newPrefab.text = "Bow skins are not supported in singleplayer when Gun mod is enabled";
                newPrefab.gameObject.SetActive(true);
                yield return StaticCoroutineRunner.StartStaticCoroutine(endPopulatingSkinsCoroutine());
                yield break;
            }

            m_Items = m_Controller.Interface.GetSkinItems(ItemFilter.Everything, weaponType);
            if (m_Items.IsNullOrEmpty())
            {
                yield return StaticCoroutineRunner.StartStaticCoroutine(endPopulatingSkinsCoroutine());
                yield break;
            }
            m_Items = m_Items.OrderBy(f => (f as WeaponSkinItemDefinitionV2).HasNameOverride ? (f as WeaponSkinItemDefinitionV2).OverrideName : f.GetItemName()).ToArray();

            foreach (IWeaponSkinItemDefinition skin in m_Items)
            {
                if (!base.gameObject.activeSelf)
                {
                    IsPopulatingSkins = false;
                    yield break;
                }

                string skinName = skin.GetItemName();

                ModdedObject newPrefab = Instantiate<ModdedObject>(GetPrefab(true), GetContainer(true));
                newPrefab.gameObject.SetActive(true);
                newPrefab.GetObject<Text>(1).text = (skin as WeaponSkinItemDefinitionV2).HasNameOverride ? (skin as WeaponSkinItemDefinitionV2).OverrideName : skinName;
                WeaponSkinsMenuSkinBehaviour b = newPrefab.gameObject.AddComponent<WeaponSkinsMenuSkinBehaviour>();
                b.Initialize();
                b.SetMenu(this);
                b.SetWeaponType(weaponType);
                b.SetSkin(skinName, (skin as WeaponSkinItemDefinitionV2).AuthorDiscord, !string.IsNullOrEmpty(skin.GetExclusivePlayerID()));
                b.TrySelect();
                b.GetComponent<Button>().interactable = skin.IsUnlocked(false);
                b.GetComponent<Animation>().enabled = !string.IsNullOrEmpty(skin.GetExclusivePlayerID());

                itemsSpawned++;
                SetFillProgress(itemsSpawned / (float)m_Items.Length);

                yield return null;
            }

            switch (weaponType)
            {
                case WeaponType.Sword:
                    ShowSkinInfo(weaponType, WeaponSkinsController.EquippedSwordSkin);
                    break;
                case WeaponType.Bow:
                    ShowSkinInfo(weaponType, WeaponSkinsController.EquippedBowSkin);
                    break;
                case WeaponType.Hammer:
                    ShowSkinInfo(weaponType, WeaponSkinsController.EquippedHammerSkin);
                    break;
                case WeaponType.Spear:
                    ShowSkinInfo(weaponType, WeaponSkinsController.EquippedSpearSkin);
                    break;
            }
            RefreshDefaultSkinButton();
            yield return StaticCoroutineRunner.StartStaticCoroutine(endPopulatingSkinsCoroutine());
            yield break;
        }

        public void SelectSkin(WeaponType weaponType, string skinName)
        {
            if (m_Controller == null || m_Controller.Interface == null || string.IsNullOrEmpty(skinName))
            {
                return;
            }

            if (!skinName.Equals("Default"))
            {
                // Check this skin is unlocked
                IWeaponSkinItemDefinition item = m_Controller.Interface.GetSkinItem(weaponType, skinName, ItemFilter.None, out _);
                if (item == null || !item.IsUnlocked(false))
                {
                    return;
                }
            }

            switch (weaponType)
            {
                case WeaponType.Sword:
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.Sword", true), skinName);
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.SwordVar", true), 0);
                    break;
                case WeaponType.Bow:
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.Bow", true), skinName);
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.BowVar", true), 0);
                    break;
                case WeaponType.Hammer:
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.Hammer", true), skinName);
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.HammerVar", true), 0);
                    break;
                case WeaponType.Spear:
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.Spear", true), skinName);
                    SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.SpearVar", true), 0);
                    break;
            }
            RefreshDefaultSkinButton();

            WeaponSkinsMenuSkinBehaviour.SelectSpecific();

            if (!getController())
            {
                return;
            }
            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if (mover != null)
            {
                WeaponSkinsController.RobotToPlayAnimationOn = mover;
                m_Controller.ApplySkinsOnCharacter(mover);
            }
            ShowSkinInfo(weaponType, skinName);

            OverhaulModdedPlayerInfo info = OverhaulModdedPlayerInfo.GetLocalPlayerInfo();
            if (info != null && info.HasReceivedData)
            {
                info.RefreshData();
            }

            StartCooldown();
        }

        public void ShowDescriptionTooltip(WeaponType type, string skinName)
        {
            MyModdedObject.GetObject<Transform>(16).parent.gameObject.SetActive(false);
            if (m_Controller == null || m_Controller.Interface == null || type == WeaponType.None)
            {
                return;
            }
            IWeaponSkinItemDefinition item = m_Controller.Interface.GetSkinItem(type, skinName, ItemFilter.None, out _);
            if (item == null || string.IsNullOrEmpty((item as WeaponSkinItemDefinitionV2).Description))
            {
                return;
            }

            MyModdedObject.GetObject<Text>(16).text = (item as WeaponSkinItemDefinitionV2).Description;
            MyModdedObject.GetObject<Transform>(16).parent.gameObject.SetActive(true);
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
            MyModdedObject.GetObject<Text>(8).text = string.Empty;
            m_Description.text = OverhaulLocalizationController.GetTranslation("NoDesc");
            if (type == WeaponType.None || string.IsNullOrEmpty(skinName))
            {
                return;
            }

            IWeaponSkinItemDefinition item = m_Controller.Interface.GetSkinItem(type, skinName, ItemFilter.Everything, out _);
            if (item == null)
            {
                return;
            }

            MyModdedObject.GetObject<Text>(8).text = (item as WeaponSkinItemDefinitionV2).HasNameOverride ? (item as WeaponSkinItemDefinitionV2).OverrideName : skinName;

            MyModdedObject.GetObject<Transform>(9).gameObject.SetActive(item.GetModel(false, false) != null);
            MyModdedObject.GetObject<Transform>(10).gameObject.SetActive(item.GetModel(false, true) != null || item.GetWeaponType() != WeaponType.Sword || (item as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer);
            MyModdedObject.GetObject<Transform>(11).gameObject.SetActive(item.GetModel(true, false) != null || item.GetWeaponType() == WeaponType.Bow);
            MyModdedObject.GetObject<Transform>(12).gameObject.SetActive(item.GetModel(true, true) != null || item.GetWeaponType() != WeaponType.Sword || item.GetWeaponType() == WeaponType.Bow || (item as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer);
            MyModdedObject.GetObject<Transform>(13).gameObject.SetActive(true);

            if (!string.IsNullOrEmpty((item as WeaponSkinItemDefinitionV2).Description))
            {
                m_Description.text = (item as WeaponSkinItemDefinitionV2).Description;
            }
        }

        public void SetAllowEnemiesUseSkins(bool value)
        {
            SettingInfo.SavePref(SettingsController.GetSetting("Player.WeaponSkins.EnemiesUseSkins", true), value);
        }

        public void DebugSetInputFieldsValues(ModelOffset transform)
        {
            if (!IsOutfitSelection)
            {
                return;
            }

            MyModdedObject.GetObject<InputField>(19).text = transform.OffsetPosition[0].ToString();
            MyModdedObject.GetObject<InputField>(20).text = transform.OffsetPosition[1].ToString();
            MyModdedObject.GetObject<InputField>(21).text = transform.OffsetPosition[2].ToString();

            MyModdedObject.GetObject<InputField>(22).text = transform.OffsetEulerAngles[0].ToString();
            MyModdedObject.GetObject<InputField>(23).text = transform.OffsetEulerAngles[1].ToString();
            MyModdedObject.GetObject<InputField>(24).text = transform.OffsetEulerAngles[2].ToString();

            MyModdedObject.GetObject<InputField>(25).text = transform.OffsetLocalScale[0].ToString();
            MyModdedObject.GetObject<InputField>(26).text = transform.OffsetLocalScale[1].ToString();
            MyModdedObject.GetObject<InputField>(27).text = transform.OffsetLocalScale[2].ToString();
        }

        public void SetFillProgress(float progress)
        {
            m_LoadIndicatorFill.fillAmount = progress;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetMenuActive(false);
            }
            m_LoadIndicatorTransform.gameObject.SetActive(IsPopulatingSkins);
        }
    }
}