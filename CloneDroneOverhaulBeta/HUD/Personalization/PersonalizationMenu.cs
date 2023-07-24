using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Patches;
using Newtonsoft.Json;
using OverhaulAPI;
using System;
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
    [Obsolete("This one will be split into several classes")]
    public class PersonalizationMenu : OverhaulUI
    {
        private static bool m_HasRefreshedUpdates;

        private static float m_TimeToAllowChangingSkins = 0f;
        private static float m_TimeChangedSkins = 0f;

        public static float GetSkinChangeCooldown() => 1f - Mathf.Clamp01((Time.unscaledTime - m_TimeChangedSkins) / (m_TimeToAllowChangingSkins - m_TimeChangedSkins));
        public static bool AllowChangingSkins() => Time.unscaledTime >= m_TimeToAllowChangingSkins;
        public static void StartCooldown()
        {
            if (GameModeManager.IsMultiplayer())
            {
                m_TimeChangedSkins = Time.unscaledTime;
                m_TimeToAllowChangingSkins = m_TimeChangedSkins + 2f;
            }
        }

        private IWeaponSkinItemDefinition[] m_Items;
        private List<OutfitItem> m_AccessoryItems;
        private WeaponSkinsController m_Controller;

        private Hashtable m_HashtableTest;
        private Text m_TextPrefab;

        private Text m_Description;
        private WeaponType m_SelectedWeapon;
        private Button m_DefaultSkinButton;
        private Button m_RefreshDatabaseButton;
        private ScrollRect m_ScrollRect;
        private CanvasGroup m_ScrollRectCanvasGroup;

        private Transform m_LoadIndicatorTransform;
        private Image m_LoadIndicatorFill;

        private Button m_DebugApplyButton;
        private Button m_DebugSaveButton;
        private Dropdown m_DebugCharacterModelsDropdown;

        private Text m_FileVersionText;

        private Button m_EditSkinButton;
        private Transform m_SkinEditorTranform;
        private Button m_SkinEditorExitButton;

        private Button m_UpdateSkinsButton;
        private Text m_UpdateSkinsText;
        private Image m_SkinsUpdateRefreshButtonCooldownFill;
        public void SetUpdateButtonInteractableState(bool value)
        {
            m_UpdateSkinsButton.interactable = value;
        }

        private bool m_SearchOnlyExclusive;
        private InputField m_SearchBox;
        private Dropdown m_SearchByDropdown;
        private Button m_SearchOnlyExclusiveButton;

        public PersonalizationCategory Category;
        public bool IsOutfitSelection => Category == PersonalizationCategory.Outfits;
        public static PersonalizationMenu SkinsSelection;
        public static PersonalizationMenu OutfitSelection;

        private static int s_CurrentlyEditingItemIndex = -1;
        public static WeaponSkinsImportedItemDefinition CurrentlyEditingItem;
        public static ModelOffset CurrentlyEditingOffset;

        public bool IsPopulatingSkins { get; private set; }

        public override void Initialize()
        {
            ModdedObject m = MyModdedObject;
            m_HashtableTest = new Hashtable
            {
                ["weapon"] = m.GetObject<ModdedObject>(2),
                ["weaponSkin"] = m.GetObject<ModdedObject>(0),
                ["weaponsContainer"] = m.GetObject<Transform>(3),
                ["weaponsSkinsContainer"] = m.GetObject<Transform>(1)
            };
            (m_HashtableTest["weapon"] as ModdedObject).gameObject.SetActive(false);
            (m_HashtableTest["weaponSkin"] as ModdedObject).gameObject.SetActive(false);

            m_ScrollRect = m.GetObject<ScrollRect>(17);
            m_ScrollRectCanvasGroup = m.GetObject<CanvasGroup>(17);

            m_TextPrefab = m.GetObject<Text>(14);
            m_TextPrefab.gameObject.SetActive(false);
            m_Description = m.GetObject<Text>(15);
            m_Description.text = string.Empty;

            GameObject panelBG = MyModdedObject.GetObject<Transform>(92).gameObject;
            OverhaulUIAnchoredPanelSlider slider = panelBG.AddComponent<OverhaulUIAnchoredPanelSlider>();
            slider.StartPosition = new Vector3(-265f, 0f, 0f);
            slider.TargetPosition = new Vector3(25f, 0f, 0f);

            m_DefaultSkinButton = m.GetObject<Button>(6);
            m_DefaultSkinButton.onClick.AddListener(SetDefaultSkin);
            if (base.gameObject.name.Equals("SkinsSelection"))
            {
                m_LoadIndicatorTransform = m.GetObject<Transform>(18);
                m_LoadIndicatorFill = m.GetObject<Image>(19);

                // Skin editor stuff
                m_FileVersionText = m.GetObject<Text>(20);
                m_RefreshDatabaseButton = m.GetObject<Button>(21);
                m_RefreshDatabaseButton.onClick.AddListener(WeaponSkinsController.ReloadAllModels);
                m_EditSkinButton = m.GetObject<Button>(22);
                m_EditSkinButton.onClick.AddListener(ShowSkinEditor);
                m_SkinEditorTranform = m.GetObject<Transform>(23);
                m_SkinEditorExitButton = m.GetObject<Button>(24);
                m_SkinEditorExitButton.onClick.AddListener(HideSkinEditor);
                m_UpdateSkinsButton = m.GetObject<Button>(67);
                m_UpdateSkinsButton.onClick.AddListener(RefreshSkinUpdates);
                m_UpdateSkinsText = m.GetObject<Text>(68);
                m_SkinsUpdateRefreshButtonCooldownFill = m.GetObject<Image>(69);

                m_SearchBox = m.GetObject<InputField>(83);
                m_SearchBox.text = string.Empty;
                m_SearchBox.onValueChanged.AddListener(SearchSkins);
                m_SearchByDropdown = m.GetObject<Dropdown>(84);
                m_SearchByDropdown.onValueChanged.AddListener(ResetSearchBox);
                m_SearchOnlyExclusiveButton = m.GetObject<Button>(85);
                m_SearchOnlyExclusiveButton.onClick.AddListener(ToggleSearchOnlyExclusive);
                RefreshSearchExclusiveSkinsButton();
            }

            m.GetObject<Button>(4).onClick.AddListener(OnDoneButtonClicked);
            m.GetObject<Toggle>(7).onValueChanged.AddListener(SetAllowEnemiesUseSkins);
            MyModdedObject.GetObject<Text>(8).text = string.Empty;

            if (base.gameObject.name.Equals("SkinsSelection"))
            {
                ShowDescriptionTooltip(WeaponType.None, null, 0f);
                SkinsSelection = this;
                _ = OverhaulEventsController.AddEventListener(EscMenuReplacement.OpenSkinsFromSettingsEventString, OpenMenuFromSettings);
            }
            else
            {
                m_AccessoryItems = new List<OutfitItem>();
                OutfitSelection = this;
                _ = OverhaulEventsController.AddEventListener(EscMenuReplacement.OpenOutfitsFromSettingsEventString, OpenMenuFromSettings);

                if (OverhaulVersion.IsDebugBuild)
                {
                    m_DebugSaveButton = m.GetObject<Button>(28);
                    m_DebugSaveButton.onClick.AddListener(delegate
                    {
                        if (OutfitsEditor.EditingItem == null || string.IsNullOrEmpty(OutfitsEditor.EditingCharacterModel))
                            return;

                        OutfitsEditor.EditingItem.SaveOffsets();
                    });
                    m_DebugApplyButton = m.GetObject<Button>(29);
                    m_DebugApplyButton.onClick.AddListener(delegate
                    {
                        if (OutfitsEditor.EditingItem == null || string.IsNullOrEmpty(OutfitsEditor.EditingCharacterModel))
                            return;

                        ModelOffset off = OutfitsEditor.EditingItem.Offsets[OutfitsEditor.EditingCharacterModel];
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
                                ow.SpawnItems();
                        }
                    });

                    DelegateScheduler.Instance.Schedule(delegate
                    {
                        m_DebugCharacterModelsDropdown = m.GetObject<Dropdown>(30);
                        m_DebugCharacterModelsDropdown.options = MultiplayerCharacterCustomizationManager.Instance.GetCharacterModelDropdownOptions(true);
                        m_DebugCharacterModelsDropdown.onValueChanged.AddListener(delegate (int index)
                        {
                            if (OutfitsEditor.EditingItem == null || string.IsNullOrEmpty(OutfitsEditor.EditingCharacterModel))
                                return;

                            SettingsManager.Instance.SetMultiplayerCharacterModelIndex(index);
                            SettingsManager.Instance.SetUseSkinInSingleplayer(true);

                            Vector3 pos = CharacterTracker.Instance.GetPlayerRobot().transform.position;
                            CharacterTracker.Instance.DestroyExistingPlayer();

                            GameObject gm = new GameObject("DebugSpawnpoint");
                            gm.transform.position = pos;
                            FirstPersonMover newPlayer = GameFlowManager.Instance.SpawnPlayer(gm.transform, true, true, null);
                            OutfitsEditor.EditingCharacterModel = MultiplayerCharacterCustomizationManager.Instance.CharacterModels[index].CharacterModelPrefab.gameObject.name + "(Clone)";
                            DebugSetInputFieldsValues(OutfitsEditor.EditingItem.Offsets[OutfitsEditor.EditingCharacterModel]);
                            Destroy(gm);
                        });
                    }, 0.5f);

                    m.GetObject<Button>(31).onClick.AddListener(delegate
                    {
                        if (OutfitsEditor.EditingItem == null || string.IsNullOrEmpty(OutfitsEditor.EditingCharacterModel))
                            return;

                        OutfitsEditor.ModelOffsetCopy = OutfitsEditor.EditingItem.Offsets[OutfitsEditor.EditingCharacterModel];
                    });

                    m.GetObject<Button>(32).onClick.AddListener(delegate
                    {
                        if (OutfitsEditor.EditingItem == null || string.IsNullOrEmpty(OutfitsEditor.EditingCharacterModel) || OutfitsEditor.ModelOffsetCopy == null)
                            return;

                        ModelOffset edititngOffset = OutfitsEditor.EditingItem.Offsets[OutfitsEditor.EditingCharacterModel];
                        edititngOffset.OffsetPosition = OutfitsEditor.ModelOffsetCopy.OffsetPosition;
                        edititngOffset.OffsetEulerAngles = OutfitsEditor.ModelOffsetCopy.OffsetEulerAngles;
                        edititngOffset.OffsetLocalScale = OutfitsEditor.ModelOffsetCopy.OffsetLocalScale;
                        DebugSetInputFieldsValues(edititngOffset);

                        FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
                        if (player != null)
                        {
                            OutfitsWearer ow = player.GetComponent<OutfitsWearer>();
                            if (ow != null)
                            {
                                ow.SpawnItems();
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

        #region Skin editor

        private bool m_HasInitializedEditor;

        private Dropdown m_CustomSkinsDropdown;
        private Button m_NewSkinItemButton;

        private Dropdown m_WeaponsDropdown;
        private InputField m_SkinAvailableForIds;
        private InputField m_SkinNameField;
        private InputField m_SkinDescriptionField;
        private InputField m_SkinAuthorField;

        private Dropdown m_SkinBehavioursDropdown;
        private Toggle m_UseSkinsInMultiplayerToggle;
        private Toggle m_VanillaBowstringsToggle;
        private Toggle m_ItemIsDevOneToggle;
        private Toggle m_EnableFireAnimationToggle;

        private Toggle m_UseFavColorWhenLaserToggle;
        private Dropdown m_ForceColorLaserDropdown;
        private Toggle m_UseFavColorWhenFireToggle;
        private Dropdown m_ForceColorFireDropdown;
        private InputField m_ColorMultiplierField;
        private InputField m_ColorSaturationField;

        private InputField m_SPLaserModelField;
        private Button m_SPLaserModelOffsetButton;
        private InputField m_MPLaserModelField;
        private Button m_MPLaserModelOffsetButton;
        private InputField m_SPFireModelField;
        private Button m_SPFireModelOffsetButton;
        private InputField m_MPFireModelField;
        private Button m_MPFireModelOffsetButton;

        private InputField m_ParentToField;
        private InputField m_MinVersionField;

        private InputField m_AssetBundleField;

        private InputField m_CustomVFXEnvCollisionField;

        private InputField[] m_PositionFields;
        private Button m_CopyPositionFromSpawnedSkinButton;
        private InputField[] m_RotationFields;
        private Button m_CopyRotationFromSpawnedSkinButton;
        private InputField[] m_ScaleFields;
        private Button m_CopyScaleFromSpawnedSkinButton;
        private Button m_ApplyOffsetsButton;

        private Button m_CopyOffsetButton;
        private Button m_PasteOffsetButton;

        private Button m_ReimportAllButton;
        private Button m_SaveButton;

        private Button m_ModFolderButton;
        private Button m_SkinsFileFolderButton;

        private Button m_SaveSkinSeparately;
        private Button m_LoadSeparatelySavedSkin;

        private Button m_UpdateAuthorName;
        private Transform m_UpdateAuthorNamePanel;
        private InputField m_OldAuthorName;
        private InputField m_NewAuthorName;
        private Button m_DoneUpdatingAuthorName;

        public void SetSkinEditingMenuActive(bool value)
        {
            if (IsOutfitSelection)
            {
                return;
            }

            if (!m_HasInitializedEditor)
            {
                m_UpdateAuthorName = MyModdedObject.GetObject<Button>(87);
                m_UpdateAuthorName.onClick.AddListener(ToggleAuthorNameUpdatePanel);
                m_UpdateAuthorNamePanel = MyModdedObject.GetObject<Transform>(88);
                m_UpdateAuthorNamePanel.gameObject.SetActive(false);
                m_OldAuthorName = MyModdedObject.GetObject<InputField>(89);
                m_NewAuthorName = MyModdedObject.GetObject<InputField>(90);
                m_DoneUpdatingAuthorName = MyModdedObject.GetObject<Button>(91);
                m_DoneUpdatingAuthorName.onClick.AddListener(UpdateAuthorName);

                m_SaveSkinSeparately = MyModdedObject.GetObject<Button>(79);
                m_SaveSkinSeparately.onClick.AddListener(SaveSkinSeparately);
                m_LoadSeparatelySavedSkin = MyModdedObject.GetObject<Button>(80);
                m_LoadSeparatelySavedSkin.onClick.AddListener(LoadSeparatedSkinFile);

                m_CustomSkinsDropdown = MyModdedObject.GetObject<Dropdown>(51);
                m_CustomSkinsDropdown.onValueChanged.AddListener(EditSkin);
                m_NewSkinItemButton = MyModdedObject.GetObject<Button>(25);
                m_NewSkinItemButton.onClick.AddListener(CreateNewSkin);

                m_WeaponsDropdown = MyModdedObject.GetObject<Dropdown>(26);
                m_SkinAvailableForIds = MyModdedObject.GetObject<InputField>(27);
                m_SkinNameField = MyModdedObject.GetObject<InputField>(28);
                m_SkinDescriptionField = MyModdedObject.GetObject<InputField>(29);
                m_SkinAuthorField = MyModdedObject.GetObject<InputField>(30);

                m_SkinBehavioursDropdown = MyModdedObject.GetObject<Dropdown>(31);
                m_UseSkinsInMultiplayerToggle = MyModdedObject.GetObject<Toggle>(38);
                m_VanillaBowstringsToggle = MyModdedObject.GetObject<Toggle>(39);
                m_ItemIsDevOneToggle = MyModdedObject.GetObject<Toggle>(40);
                m_EnableFireAnimationToggle = MyModdedObject.GetObject<Toggle>(41);

                List<Dropdown.OptionData> colors = HumanFactsManager.Instance.GetColorDropdownOptions();
                //colors.Insert(0, new Dropdown.OptionData("Auto"));

                m_UseFavColorWhenLaserToggle = MyModdedObject.GetObject<Toggle>(34);
                m_ForceColorLaserDropdown = MyModdedObject.GetObject<Dropdown>(35);
                m_ForceColorLaserDropdown.options = colors;
                m_UseFavColorWhenFireToggle = MyModdedObject.GetObject<Toggle>(32);
                m_ForceColorFireDropdown = MyModdedObject.GetObject<Dropdown>(33);
                m_ForceColorFireDropdown.options = colors;
                m_ColorMultiplierField = MyModdedObject.GetObject<InputField>(36);
                m_ColorSaturationField = MyModdedObject.GetObject<InputField>(37);

                m_SPLaserModelField = MyModdedObject.GetObject<InputField>(42);
                m_SPFireModelField = MyModdedObject.GetObject<InputField>(44);
                m_MPLaserModelField = MyModdedObject.GetObject<InputField>(46);
                m_MPFireModelField = MyModdedObject.GetObject<InputField>(48);

                m_ParentToField = MyModdedObject.GetObject<InputField>(70);
                m_MinVersionField = MyModdedObject.GetObject<InputField>(71);

                m_AssetBundleField = MyModdedObject.GetObject<InputField>(72);

                m_SPLaserModelOffsetButton = MyModdedObject.GetObject<Button>(43);
                m_SPLaserModelOffsetButton.onClick.AddListener(delegate
                {
                    EditOffset(false, false);
                });
                m_SPFireModelOffsetButton = MyModdedObject.GetObject<Button>(45);
                m_SPFireModelOffsetButton.onClick.AddListener(delegate
                {
                    EditOffset(true, false);
                });
                m_MPLaserModelOffsetButton = MyModdedObject.GetObject<Button>(47);
                m_MPLaserModelOffsetButton.onClick.AddListener(delegate
                {
                    EditOffset(false, true);
                });
                m_MPFireModelOffsetButton = MyModdedObject.GetObject<Button>(49);
                m_MPFireModelOffsetButton.onClick.AddListener(delegate
                {
                    EditOffset(true, true);
                });

                m_PositionFields = new InputField[]
                {
                     MyModdedObject.GetObject<InputField>(52),
                       MyModdedObject.GetObject<InputField>(53),
                         MyModdedObject.GetObject<InputField>(54),
                };
                m_CopyPositionFromSpawnedSkinButton = MyModdedObject.GetObject<Button>(55);
                m_CopyPositionFromSpawnedSkinButton.onClick.AddListener(delegate
                {
                    CopyVectorFromSkinModel(0);
                });

                m_RotationFields = new InputField[]
                {
                     MyModdedObject.GetObject<InputField>(56),
                       MyModdedObject.GetObject<InputField>(57),
                         MyModdedObject.GetObject<InputField>(58),
                };
                m_CopyRotationFromSpawnedSkinButton = MyModdedObject.GetObject<Button>(59);
                m_CopyRotationFromSpawnedSkinButton.onClick.AddListener(delegate
                {
                    CopyVectorFromSkinModel(1);
                });

                m_ScaleFields = new InputField[]
                {
                     MyModdedObject.GetObject<InputField>(60),
                       MyModdedObject.GetObject<InputField>(61),
                         MyModdedObject.GetObject<InputField>(62),
                };
                m_CopyScaleFromSpawnedSkinButton = MyModdedObject.GetObject<Button>(63);
                m_CopyScaleFromSpawnedSkinButton.onClick.AddListener(delegate
                {
                    CopyVectorFromSkinModel(2);
                });
                m_ApplyOffsetsButton = MyModdedObject.GetObject<Button>(64);
                m_ApplyOffsetsButton.onClick.AddListener(SetOffsetValues);
                m_ApplyOffsetsButton.interactable = false;

                m_CopyOffsetButton = MyModdedObject.GetObject<Button>(65);
                m_CopyOffsetButton.onClick.AddListener(CopyOffset);
                m_PasteOffsetButton = MyModdedObject.GetObject<Button>(66);
                m_PasteOffsetButton.onClick.AddListener(PasteOffset);
                m_PasteOffsetButton.interactable = false;

                m_SaveButton = MyModdedObject.GetObject<Button>(50);
                m_SaveButton.onClick.AddListener(SaveEditingSkin);
                m_ReimportAllButton = MyModdedObject.GetObject<Button>(73);
                m_ReimportAllButton.onClick.AddListener(delegate
                {
                    WeaponSkinsController c = GetController<WeaponSkinsController>();
                    if (c != null && PersonalizationMenu.SkinsSelection != null)
                    {
                        c.ReImportCustomSkins();
                        PersonalizationMenu.SkinsSelection.SetMenuActive(true);
                    }
                });

                m_CustomVFXEnvCollisionField = MyModdedObject.GetObject<InputField>(77);

                m_ModFolderButton = MyModdedObject.GetObject<Button>(74);
                m_ModFolderButton.onClick.AddListener(OpenModDirectory);
                m_SkinsFileFolderButton = MyModdedObject.GetObject<Button>(75);
                m_SkinsFileFolderButton.onClick.AddListener(OpenSkinsFileDirectory);

                m_HasInitializedEditor = true;
            }

            m_SkinEditorTranform.gameObject.SetActive(value);
            RefreshOptions(false);

            if (!value)
                return;

            if (s_CurrentlyEditingItemIndex == -1)
            {
                EditSkin(m_CustomSkinsDropdown.options.Count - 1);
            }
            else
            {
                EditSkin(s_CurrentlyEditingItemIndex);
            }
        }

        public void ShowSkinEditor()
        {
            SetSkinEditingMenuActive(true);
        }

        public void HideSkinEditor()
        {
            SetSkinEditingMenuActive(false);
        }

        public void RefreshOptions(bool setLastIndex = false)
        {
            m_CustomSkinsDropdown.options = WeaponSkinsController.CustomSkinsData.GetOptions();
            if (setLastIndex) m_CustomSkinsDropdown.value = m_CustomSkinsDropdown.options.Count - 1;
        }

        public void RefreshFields()
        {
            if (CurrentlyEditingItem == null)
            {
                return;
            }

            m_SkinNameField.text = CurrentlyEditingItem.Name;
            m_SkinDescriptionField.text = CurrentlyEditingItem.Description;
            m_SkinAuthorField.text = CurrentlyEditingItem.Author;
            m_SkinAvailableForIds.text = CurrentlyEditingItem.OnlyAvailableFor;

            switch (CurrentlyEditingItem.OfWeaponType)
            {
                case WeaponType.Sword:
                    m_WeaponsDropdown.value = 0;
                    break;
                case WeaponType.Bow:
                    m_WeaponsDropdown.value = 1;
                    break;
                case WeaponType.Hammer:
                    m_WeaponsDropdown.value = 2;
                    break;
                case WeaponType.Spear:
                    m_WeaponsDropdown.value = 3;
                    break;
            }

            m_UseFavColorWhenLaserToggle.isOn = CurrentlyEditingItem.ApplyFavColorOnLaser;
            m_UseFavColorWhenFireToggle.isOn = CurrentlyEditingItem.ApplyFavColorOnFire;
            m_ColorMultiplierField.text = CurrentlyEditingItem.Multiplier.ToString();
            m_ColorSaturationField.text = CurrentlyEditingItem.Saturation.ToString();
            m_SkinBehavioursDropdown.value = CurrentlyEditingItem.BehaviourIndex;

            m_ParentToField.text = CurrentlyEditingItem.ParentTo;
            m_MinVersionField.text = CurrentlyEditingItem.MinVersion == null ? OverhaulVersion.ModVersion.ToString() : CurrentlyEditingItem.MinVersion.ToString();

            m_AssetBundleField.text = string.IsNullOrEmpty(CurrentlyEditingItem.AssetBundleFileName) ? OverhaulAssetsController.ModAssetBundle_Skins : CurrentlyEditingItem.AssetBundleFileName;

            m_SPLaserModelField.text = CurrentlyEditingItem.SingleplayerLaserModelName;
            m_SPFireModelField.text = CurrentlyEditingItem.SingleplayerFireModelName;
            m_MPLaserModelField.text = CurrentlyEditingItem.MultiplayerLaserModelName;
            m_MPFireModelField.text = CurrentlyEditingItem.MultiplayerFireModelName;

            m_CustomVFXEnvCollisionField.text = CurrentlyEditingItem.CollideWithEnvironmentVFXAssetName;

            m_UseSkinsInMultiplayerToggle.isOn = CurrentlyEditingItem.ApplySingleplayerModelInMultiplayer;
            m_VanillaBowstringsToggle.isOn = CurrentlyEditingItem.UseVanillaBowstrings;
            m_EnableFireAnimationToggle.isOn = CurrentlyEditingItem.AnimateFire;
            m_ItemIsDevOneToggle.isOn = CurrentlyEditingItem.IsDeveloperItem;

            m_ForceColorFireDropdown.value = CurrentlyEditingItem.ForcedFavColorFireIndex + 1;
            m_ForceColorLaserDropdown.value = CurrentlyEditingItem.ForcedFavColorLaserIndex + 1;
        }

        public void CreateNewSkin()
        {
            EditSkin(WeaponSkinsController.CustomSkinsData.AllCustomSkins.Count);
        }

        public void EditSkin(int index)
        {
            m_ApplyOffsetsButton.interactable = false;

            bool createdNew = false;
            WeaponSkinsImportedItemDefinition toEdit;
            if (!WeaponSkinsController.CustomSkinsData.AllCustomSkins.IsNullOrEmpty() && index < WeaponSkinsController.CustomSkinsData.AllCustomSkins.Count)
            {
                toEdit = WeaponSkinsController.CustomSkinsData.AllCustomSkins[index];
            }
            else
            {
                createdNew = true;
                toEdit = WeaponSkinsImportedItemDefinition.GetNew(true);
            }

            CurrentlyEditingItem = toEdit;
            CurrentlyEditingOffset = null;
            RefreshOptions(createdNew);
            RefreshFields();

            s_CurrentlyEditingItemIndex = createdNew ? m_CustomSkinsDropdown.options.Count - 1 : index;
        }

        public void SaveEditingSkin()
        {
            if (CurrentlyEditingItem == null)
            {
                return;
            }

            WeaponType newWeaponType = WeaponType.Sword;
            switch (m_WeaponsDropdown.value)
            {
                case 1:
                    newWeaponType = WeaponType.Bow;
                    break;
                case 2:
                    newWeaponType = WeaponType.Hammer;
                    break;
                case 3:
                    newWeaponType = WeaponType.Spear;
                    break;
            }

            string newSkinName = m_SkinNameField.text;
            string newSkinDesc = m_SkinDescriptionField.text;
            string newSkinAuthor = m_SkinAuthorField.text;
            string newSkinExclusiveIds = m_SkinAvailableForIds.text;

            bool skinUseFavColorWhenLaser = m_UseFavColorWhenLaserToggle.isOn;
            bool skinUseFavColorWhenFire = m_UseFavColorWhenFireToggle.isOn;
            bool colorMultiplierConvertSuccessful = float.TryParse(m_ColorMultiplierField.text, out float newColorMultiplier);
            bool colorSaturationConvertSuccessful = float.TryParse(m_ColorSaturationField.text, out float newColorSaturation);

            bool useSpModelsInMp = m_UseSkinsInMultiplayerToggle.isOn;
            bool useVanillaBowstrings = m_VanillaBowstringsToggle.isOn;
            bool isDevItem = m_ItemIsDevOneToggle.isOn;
            bool animateFire = m_EnableFireAnimationToggle.isOn;

            CurrentlyEditingItem.OfWeaponType = newWeaponType;

            CurrentlyEditingItem.Name = newSkinName;
            CurrentlyEditingItem.Description = newSkinDesc;
            CurrentlyEditingItem.Author = newSkinAuthor;
            CurrentlyEditingItem.OnlyAvailableFor = newSkinExclusiveIds;
            CurrentlyEditingItem.BehaviourIndex = m_SkinBehavioursDropdown.value;

            CurrentlyEditingItem.ApplyFavColorOnLaser = skinUseFavColorWhenLaser;
            CurrentlyEditingItem.ForcedFavColorLaserIndex = m_ForceColorLaserDropdown.value - 1;
            CurrentlyEditingItem.ApplyFavColorOnFire = skinUseFavColorWhenFire;
            CurrentlyEditingItem.ForcedFavColorFireIndex = m_ForceColorFireDropdown.value - 1;
            CurrentlyEditingItem.Multiplier = colorMultiplierConvertSuccessful ? newColorMultiplier : 1f;
            CurrentlyEditingItem.Saturation = colorSaturationConvertSuccessful ? newColorSaturation : 0.75f;

            CurrentlyEditingItem.UseVanillaBowstrings = useVanillaBowstrings;
            CurrentlyEditingItem.ApplySingleplayerModelInMultiplayer = useSpModelsInMp;
            CurrentlyEditingItem.IsDeveloperItem = isDevItem;
            CurrentlyEditingItem.AnimateFire = animateFire;

            CurrentlyEditingItem.CollideWithEnvironmentVFXAssetName = m_CustomVFXEnvCollisionField.text;

            if (!OverhaulAssetsController.DoesAssetBundleExist(m_AssetBundleField.text)) OverhaulDialogues.CreateDialogue("Asset bundle not found!", m_AssetBundleField.text + " doesn't exist in mod folder.", 4f, new Vector2(300, 200), new OverhaulDialogues.Button[] { });
            CurrentlyEditingItem.AssetBundleFileName = m_AssetBundleField.text;

            CurrentlyEditingItem.ParentTo = m_ParentToField.text;
            bool successMinVersionParsing = Version.TryParse(m_MinVersionField.text, out Version minVersion);
            if (!successMinVersionParsing)
            {
                minVersion = (Version)OverhaulVersion.ModVersion.Clone();
            }
            CurrentlyEditingItem.MinVersion = minVersion;

            bool hasSPLaserModel = true;
            try
            {
                OverhaulAssetsController.PreloadAsset<GameObject>(m_SPLaserModelField.text, CurrentlyEditingItem.AssetBundleFileName);
            }
            catch
            {
                hasSPLaserModel = false;
            }
            CurrentlyEditingItem.SingleplayerLaserModelName = hasSPLaserModel ? m_SPLaserModelField.text : "-";

            bool hasSPFireModel = true;
            try
            {
                OverhaulAssetsController.PreloadAsset<GameObject>(m_SPFireModelField.text, CurrentlyEditingItem.AssetBundleFileName);
            }
            catch
            {
                hasSPFireModel = false;
            }
            CurrentlyEditingItem.SingleplayerFireModelName = hasSPFireModel ? m_SPFireModelField.text : "-";

            if (newWeaponType == WeaponType.Sword && !useSpModelsInMp)
            {
                bool hasMPLaserModel = true;
                try
                {
                    OverhaulAssetsController.PreloadAsset<GameObject>(m_MPLaserModelField.text, CurrentlyEditingItem.AssetBundleFileName);
                }
                catch
                {
                    hasMPLaserModel = false;
                }
                CurrentlyEditingItem.MultiplayerLaserModelName = hasMPLaserModel ? m_MPLaserModelField.text : "-";

                bool hasMPFireModel = true;
                try
                {
                    OverhaulAssetsController.PreloadAsset<GameObject>(m_MPFireModelField.text, CurrentlyEditingItem.AssetBundleFileName);
                }
                catch
                {
                    hasMPFireModel = false;
                }
                CurrentlyEditingItem.MultiplayerFireModelName = hasMPFireModel ? m_MPFireModelField.text : "-";
            }

            WeaponSkinsController.CustomSkinsData.SaveSkins();
            RefreshOptions(false);
            RefreshFields();

            OverhaulDialogues.CreateDialogue("Saved skin", CurrentlyEditingItem.Name + "\nLogs:\nParsed color multiplier: " + colorMultiplierConvertSuccessful + "\nParsed color saturation: " + colorSaturationConvertSuccessful + "\nParsed min version: " + successMinVersionParsing, 4f, new Vector2(300, 200), new OverhaulDialogues.Button[] { });

            WeaponSkinsController.SkinsDataIsDirty = true;
            FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
            if (player == null || player.GetComponent<WeaponSkinsWearer>() == null)
            {
                return;
            }

            WeaponSkinsWearer w = player.GetComponent<WeaponSkinsWearer>();
            w.SpawnSkins();
        }

        public void RefreshOffsetFields()
        {
            ModelOffset offset = CurrentlyEditingOffset;
            if (offset == null)
            {
                return;
            }

            m_PositionFields[0].text = offset.OffsetPosition[0].ToString();
            m_PositionFields[1].text = offset.OffsetPosition[1].ToString();
            m_PositionFields[2].text = offset.OffsetPosition[2].ToString();

            m_RotationFields[0].text = offset.OffsetEulerAngles[0].ToString();
            m_RotationFields[1].text = offset.OffsetEulerAngles[1].ToString();
            m_RotationFields[2].text = offset.OffsetEulerAngles[2].ToString();

            m_ScaleFields[0].text = offset.OffsetLocalScale[0].ToString();
            m_ScaleFields[1].text = offset.OffsetLocalScale[1].ToString();
            m_ScaleFields[2].text = offset.OffsetLocalScale[2].ToString();
        }

        public void SetOffsetValues()
        {
            if (CurrentlyEditingOffset == null)
            {
                return;
            }

            try
            {
                Vector3 position = new Vector3(float.Parse(m_PositionFields[0].text), float.Parse(m_PositionFields[1].text), float.Parse(m_PositionFields[2].text));
                Vector3 rotation = new Vector3(float.Parse(m_RotationFields[0].text), float.Parse(m_RotationFields[1].text), float.Parse(m_RotationFields[2].text));
                Vector3 scale = new Vector3(float.Parse(m_ScaleFields[0].text), float.Parse(m_ScaleFields[1].text), float.Parse(m_ScaleFields[2].text));

                CurrentlyEditingOffset.OffsetPosition = position;
                CurrentlyEditingOffset.OffsetEulerAngles = rotation;
                CurrentlyEditingOffset.OffsetLocalScale = scale;

                FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
                if (player == null || player.GetComponent<WeaponSkinsWearer>() == null)
                {
                    return;
                }

                WeaponSkinsController.SkinsDataIsDirty = true;
                WeaponSkinsWearer w = player.GetComponent<WeaponSkinsWearer>();
                w.SpawnSkins();
            }
            catch
            {
                OverhaulDialogues.CreateDialogue("Error occurred while applying new offsets", "Skin (spawned model): " + CurrentlyEditingItem.Name + " wasn't updated", 4f, new Vector2(300, 200), new OverhaulDialogues.Button[] { });
                return;
            }

            OverhaulDialogues.CreateDialogue("Successfully applied new offsets", "Refreshed offsets for skin: " + CurrentlyEditingItem.Name, 4f, new Vector2(300, 200), new OverhaulDialogues.Button[] { });
        }

        public void EditOffset(bool fire, bool multiplayer)
        {
            if (CurrentlyEditingItem == null)
            {
                return;
            }

            if (!fire && !multiplayer)
            {
                CurrentlyEditingOffset = CurrentlyEditingItem.SingleplayerLaserModelOffset;
            }
            else if (fire && !multiplayer)
            {
                CurrentlyEditingOffset = CurrentlyEditingItem.SingleplayerFireModelOffset;
            }
            else if (!fire && multiplayer)
            {
                CurrentlyEditingOffset = CurrentlyEditingItem.MultiplayerLaserModelOffset;
            }
            else if (fire && multiplayer)
            {
                CurrentlyEditingOffset = CurrentlyEditingItem.MultiplayerFireModelOffset;
            }

            m_ApplyOffsetsButton.interactable = true;
            RefreshOffsetFields();
        }

        public void CopyVectorFromSkinModel(byte index)
        {
            FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
            if (CurrentlyEditingItem == null || CurrentlyEditingOffset == null || player == null || player.GetComponent<WeaponSkinsWearer>() == null)
            {
                return;
            }

            WeaponSkinsWearer w = player.GetComponent<WeaponSkinsWearer>();
            Transform t = w.GetTransform();
            if (t == null)
            {
                return;
            }

            switch (index)
            {
                case 0:
                    m_PositionFields[0].text = t.localPosition[0].ToString();
                    m_PositionFields[1].text = t.localPosition[1].ToString();
                    m_PositionFields[2].text = t.localPosition[2].ToString();
                    break;
                case 1:
                    m_RotationFields[0].text = t.localEulerAngles[0].ToString();
                    m_RotationFields[1].text = t.localEulerAngles[1].ToString();
                    m_RotationFields[2].text = t.localEulerAngles[2].ToString();
                    break;
                case 2:
                    m_ScaleFields[0].text = t.localScale[0].ToString();
                    m_ScaleFields[1].text = t.localScale[1].ToString();
                    m_ScaleFields[2].text = t.localScale[2].ToString();
                    break;
            }
        }

        public void CopyOffset()
        {
            if (CurrentlyEditingOffset == null)
            {
                return;
            }

            WeaponSkinsEditor.CopiedOffset = CurrentlyEditingOffset;
            m_PasteOffsetButton.interactable = true;
        }

        public void PasteOffset()
        {
            if (CurrentlyEditingOffset == null || WeaponSkinsEditor.CopiedOffset == null || Equals(CurrentlyEditingOffset, WeaponSkinsEditor.CopiedOffset))
            {
                return;
            }

            CurrentlyEditingOffset.OffsetPosition = WeaponSkinsEditor.CopiedOffset.OffsetPosition;
            CurrentlyEditingOffset.OffsetEulerAngles = WeaponSkinsEditor.CopiedOffset.OffsetEulerAngles;
            CurrentlyEditingOffset.OffsetLocalScale = WeaponSkinsEditor.CopiedOffset.OffsetLocalScale;
            RefreshOffsetFields();
        }

        public void OpenModDirectory()
        {
            _ = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = OverhaulMod.Core.ModDirectory,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public void OpenSkinsFileDirectory()
        {
            _ = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = OverhaulMod.Core.ModDirectory + "Assets/Download/Permanent",
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public void SaveSkinSeparately()
        {
            if (CurrentlyEditingItem == null)
            {
                return;
            }

            string path = OverhaulMod.Core.ModDirectory + "Assets/Download/Permanent";
            string content = JsonConvert.SerializeObject(CurrentlyEditingItem);
            bool success = OverhaulCore.TryWriteText(path + "/" + CurrentlyEditingItem.Name.Replace(' ', '_') + ".json", content, out Exception exc);
            if (!success)
            {
                OverhaulDialogues.CreateDialogue("Error occurred while saving skin", exc.ToString(), 4f, new Vector2(300, 400), new OverhaulDialogues.Button[] { });
                return;
            }

            _ = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public void LoadSeparatedSkinFile()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog()
            {
                InitialDirectory = (OverhaulMod.Core.ModDirectory + "Assets/Download/Permanent").Replace("/", "\\"),
                Filter = "Json Files (*.json) | *.json",
                RestoreDirectory = true
            };

            //User didn't select a file so return a default value  
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            UnityEngine.Debug.Log(dlg.FileName);

            bool success = OverhaulCore.TryReadText(dlg.FileName, out string content, out Exception exc);
            if (!success)
            {
                OverhaulDialogues.CreateDialogue("Error occurred while loading skin", "Output file path: " + dlg.FileName + "\n" + exc.ToString(), 4f, new Vector2(300, 400), new OverhaulDialogues.Button[] { });
                return;
            }

            WeaponSkinsImportedItemDefinition loadedSkin = JsonConvert.DeserializeObject<WeaponSkinsImportedItemDefinition>(content);
            if (string.IsNullOrEmpty(loadedSkin.Name))
            {
                OverhaulDialogues.CreateDialogue("Error occurred while loading skin", "Some fields are null or loaded incorrectly", 4f, new Vector2(300, 400), new OverhaulDialogues.Button[] { });
                return;
            }

            WeaponSkinsController.CustomSkinsData.AllCustomSkins.Add(loadedSkin);
            EditSkin(WeaponSkinsController.CustomSkinsData.AllCustomSkins.Count - 1);
            OverhaulDialogues.CreateDialogue("Successfully loaded skin", "Now you have to click on \"Save all\" button, then reload all skins", 4f, new Vector2(300, 300), new OverhaulDialogues.Button[] { });
        }

        public void ToggleAuthorNameUpdatePanel()
        {
            m_UpdateAuthorNamePanel.gameObject.SetActive(!m_UpdateAuthorNamePanel.gameObject.activeSelf);
        }

        public void UpdateAuthorName()
        {
            string toReplace = m_OldAuthorName.text;
            string replaceWith = m_NewAuthorName.text;

            foreach (WeaponSkinsImportedItemDefinition item in WeaponSkinsController.CustomSkinsData.AllCustomSkins)
            {
                item.Author = item.Author.Replace(toReplace, replaceWith);
            }
            m_UpdateAuthorNamePanel.gameObject.SetActive(false);
        }

        #endregion

        public void OpenMenuFromSettings()
        {
            if (GameUIRoot.Instance == null)
                return;

            OverhaulPauseMenu menu = GetController<OverhaulPauseMenu>();
            ParametersMenu paramsMenu = GetController<ParametersMenu>();
            EscMenu escMenu = GameUIRoot.Instance.EscMenu;
            if (menu == null || paramsMenu == null || escMenu == null)
            {
                OverhaulDialogues.CreateDialogue("Error occurred while applying new offsets", "Skin (spawned model): " + CurrentlyEditingItem.Name + " wasn't updated", 4f, new Vector2(300, 200), new OverhaulDialogues.Button[] { });
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
            m_HashtableTest.Clear();
            base.OnDisposed();
        }

        private bool getController()
        {
            if (m_Controller != null)
                return true;

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
            if (IsDisposedOrDestroyed() || MyModdedObject == null)
                return;

            base.gameObject.SetActive(value);
            ShowCursor = value;

            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if (mover != null)
            {
                if (GameUIRoot.Instance != null && GameUIRoot.Instance.CloneUI != null)
                {
                    if (value)
                        GameUIRoot.Instance.CloneUI.Hide();
                    else if (!SettingsManager.Instance.ShouldHideGameUI())
                        GameUIRoot.Instance.CloneUI.Show();
                }

                CharacterModel characterModel = mover.GetCharacterModel();
                if (characterModel != null)
                {
                    if (!GameModeManager.IsMultiplayer()) mover.InstantlySetTorsoTiltX(0);
                    if (characterModel.transform.childCount != 0) characterModel.transform.GetChild(0).localEulerAngles = IsOutfitSelection ? value ? new Vector3(0, 180, 0) : Vector3.zero : value ? new Vector3(0, 90, 0) : Vector3.zero;
                }
            }

            if (IsOutfitSelection)
            {
                PlayerStatusBehaviour.SetOwnStatus(value ? PlayerStatusType.EquippingAccessories : PlayerStatusType.Idle);
                if (value)
                    PopulateSkins(WeaponType.None);
            }
            else
                PlayerStatusBehaviour.SetOwnStatus(value ? PlayerStatusType.SwitchingSkins : PlayerStatusType.Idle);

            if (!value || IsOutfitSelection)
                return;

            PopulateWeapons();
            RefreshSkinUpdatesText();
            SetSkinEditingMenuActive(false);
            MyModdedObject.GetObject<Toggle>(7).isOn = WeaponSkinsController.AllowEnemiesWearSkins;

            //m_EditSkinButton.interactable = OverhaulFeatureAvailabilitySystem.IsFeatureUnlocked(OverhaulFeatureID.PermissionToManageSkins);
            m_RefreshDatabaseButton.interactable = m_EditSkinButton.interactable;

            Transform container = GetContainer(true);
            if (container != null) TransformUtils.DestroyAllChildren(container);
            if (!m_HasRefreshedUpdates)
            {
                m_HasRefreshedUpdates = true;
                RefreshSkinUpdates();
            }
        }

        public void RefreshSkinUpdatesText()
        {
            m_UpdateSkinsText.text = WeaponSkinsUpdater.GetUpdateButtonText();
            m_FileVersionText.text = WeaponSkinsUpdater.GetFullStateString();
        }

        public void RefreshSkinUpdates()
        {
            if (!WeaponSkinsUpdater.IsAbleToRefreshUpdates)
                return;

            WeaponSkinsUpdater.RefreshUpdates(onRefreshSkinUpdates);
            m_UpdateSkinsButton.interactable = false;
            RefreshSkinUpdatesText();
        }

        private void onRefreshSkinUpdates()
        {
            //m_UpdateSkinsButton.interactable = !WeaponSkinsController.HasUpdatedSkins;
            m_UpdateSkinsButton.interactable = true;
            RefreshSkinUpdatesText();
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
                return;

            TransformUtils.DestroyAllChildren(GetContainer(false));
            foreach (WeaponType type in WeaponSkinsController.SupportedWeapons)
                PopulateWeapon(type);

            FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
            if (player != null)
            {
                WeaponType wType = player.GetEquippedWeaponType();
                if (WeaponSkinsController.IsWeaponSupported(wType))
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
                return;

            ModdedObject newPrefab = Instantiate<ModdedObject>(GetPrefab(false), GetContainer(false));
            newPrefab.gameObject.SetActive(true);
            newPrefab.GetObject<Text>(1).text = LocalizationManager.Instance.GetTranslatedString(weaponType.ToString());
            PersonalizationMenuSecondEntryBehaviour b = newPrefab.gameObject.AddComponent<PersonalizationMenuSecondEntryBehaviour>();
            b.SetMenu(this);
            b.SetWeaponType(weaponType);
            b.SetSelected(false, true);
        }

        public void PopulateSkins(WeaponType weaponType)
        {
            if (IsDisposedOrDestroyed() || !getController())
                return;

            if (!IsOutfitSelection)
            {
                if (m_SearchByDropdown.value == 0) m_SearchBox.text = string.Empty;
                FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
                if (mover != null && mover.HasCharacterModel() && mover.HasWeapon(weaponType))
                {
                    if (!GameModeManager.IsMultiplayer())
                    {
                        mover.SetEquippedWeaponType(weaponType);
                    }
                }
                PersonalizationMenuSecondEntryBehaviour.SelectSpecific(weaponType);
            }

            _ = StaticCoroutineRunner.StartStaticCoroutine(populateSkinsCoroutine(weaponType));
        }

        private IEnumerator endPopulatingSkinsCoroutine()
        {
            if (!IsOutfitSelection)
                SearchSkins(m_SearchBox.text);

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
                m_AccessoryItems = OutfitsController.AllOutfitItems;
                if (m_AccessoryItems.IsNullOrEmpty())
                {
                    yield return StaticCoroutineRunner.StartStaticCoroutine(endPopulatingSkinsCoroutine());
                    yield break;
                }
                m_AccessoryItems = m_AccessoryItems.OrderBy(f => f.Name).ToList();

                foreach (OutfitItem aitem in m_AccessoryItems)
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
                    newPrefab.GetComponent<Animation>().enabled = !string.IsNullOrEmpty(aitem.UnlockedFor);
                    PersonalizationMenuEntryBehaviour b = newPrefab.gameObject.AddComponent<PersonalizationMenuEntryBehaviour>();
                    b.IsOutfitSelection = true;
                    b.Initialize();
                    b.SetMenu(this);
                    b.SetSkin(itemName, aitem.Author, !string.IsNullOrEmpty(aitem.UnlockedFor));
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
                PersonalizationMenuEntryBehaviour b = newPrefab.gameObject.AddComponent<PersonalizationMenuEntryBehaviour>();
                b.SkinItem = skin as WeaponSkinItemDefinitionV2;
                b.Initialize();
                b.SetMenu(this);
                b.SetWeaponType(weaponType);
                b.SetSkin(skinName, (skin as WeaponSkinItemDefinitionV2).AuthorDiscord, !string.IsNullOrEmpty(skin.GetExclusivePlayerID()));
                b.TrySelect();
                b.GetComponent<Button>().interactable = skin.IsUnlocked(false);
                b.GetComponent<Animation>().enabled = !string.IsNullOrEmpty(skin.GetExclusivePlayerID());
                if ((skin as WeaponSkinItemDefinitionV2).IsDeveloperItem && !(skin as WeaponSkinItemDefinitionV2).IsDevItemUnlocked)
                    b.gameObject.SetActive(false);

                itemsSpawned++;
                SetFillProgress(itemsSpawned / (float)m_Items.Length);

                if(itemsSpawned % 3 == 0) yield return null;
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

        public void ToggleSearchOnlyExclusive()
        {
            m_SearchOnlyExclusive = !m_SearchOnlyExclusive;
            SearchSkins(m_SearchBox.text);
            RefreshSearchExclusiveSkinsButton();
        }

        public void RefreshSearchExclusiveSkinsButton()
        {
            m_SearchOnlyExclusiveButton.GetComponent<Image>().color = m_SearchOnlyExclusive ? Color.white : Color.gray;
            m_SearchOnlyExclusiveButton.OnDeselect(null);
        }

        public void ResetSearchBox(int i) => ResetSearchBox();
        public void ResetSearchBox()
        {
            m_SearchBox.text = string.Empty;
        }

        public void SearchSkins(string input)
        {
            List<PersonalizationMenuEntryBehaviour> spawned = PersonalizationMenuEntryBehaviour.GetSpawnedButtons();
            if (spawned.IsNullOrEmpty())
                return;

            int i = 0;
            do
            {
                PersonalizationMenuEntryBehaviour b = spawned[i];
                if (b)
                {
                    bool condition1 = !m_SearchOnlyExclusive || b.GetSkinIsExclusive();
                    b.gameObject.SetActive(false);
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        b.gameObject.SetActive(condition1);
                        i++;
                        continue;
                    }
                    else
                    {
                        b.gameObject.SetActive((m_SearchByDropdown.value == 0 ? b.GetSkinName() : b.GetSkinAuthor()).Contains(input.ToLower()) && condition1);
                    }
                }
                i++;
            } while (i < spawned.Count);
        }

        public void SelectSkin(WeaponType weaponType, string skinName)
        {
            if (m_Controller == null || m_Controller.Interface == null || string.IsNullOrEmpty(skinName))
                return;

            WeaponSkinsController.SkinsDataIsDirty = true;

            if (!skinName.Equals("Default"))
            {
                // Check this skin is unlocked
                IWeaponSkinItemDefinition item = m_Controller.Interface.GetSkinItem(weaponType, skinName, ItemFilter.None, out _);
                if (item == null || !item.IsUnlocked(false))
                    return;
            }

            switch (weaponType)
            {
                case WeaponType.Sword:
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Player.WeaponSkins.Sword", true), skinName);
                    break;
                case WeaponType.Bow:
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Player.WeaponSkins.Bow", true), skinName);
                    break;
                case WeaponType.Hammer:
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Player.WeaponSkins.Hammer", true), skinName);
                    break;
                case WeaponType.Spear:
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Player.WeaponSkins.Spear", true), skinName);
                    break;
            }
            RefreshDefaultSkinButton();

            PersonalizationMenuEntryBehaviour.SelectSpecific();

            if (!getController())
                return;

            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if (mover != null)
            {
                WeaponSkinsController.RobotToPlayAnimationOn = mover;
                m_Controller.ApplySkinsOnCharacter(mover);
            }
            ShowSkinInfo(weaponType, skinName);

            OverhaulModdedPlayerInfo info = OverhaulModdedPlayerInfo.GetLocalPlayerInfo();
            if (info != null && info.HasReceivedData)
                info.RefreshData();

            StartCooldown();
        }

        public void ShowDescriptionTooltip(WeaponType type, string skinName, float yPos = 0f)
        {
            Transform t = MyModdedObject.GetObject<Transform>(16).parent;
            t.gameObject.SetActive(false);

            if (m_Controller == null || m_Controller.Interface == null || type == WeaponType.None)
                return;

            IWeaponSkinItemDefinition item = m_Controller.Interface.GetSkinItem(type, skinName, ItemFilter.Everything, out _);
            if (item == null)
                return;

            MyModdedObject.GetObject<Transform>(81).gameObject.SetActive(!string.IsNullOrEmpty(item.GetExclusivePlayerID()) && !item.IsUnlocked(OverhaulVersion.IsDebugBuild));
            MyModdedObject.GetObject<Transform>(82).gameObject.SetActive(string.IsNullOrEmpty(item.GetExclusivePlayerID()));
            MyModdedObject.GetObject<Transform>(86).gameObject.SetActive(!MyModdedObject.GetObject<Transform>(81).gameObject.activeSelf && !MyModdedObject.GetObject<Transform>(82).gameObject.activeSelf);
            MyModdedObject.GetObject<Text>(16).text = string.IsNullOrEmpty((item as WeaponSkinItemDefinitionV2).Description) ? OverhaulLocalizationController.GetTranslation("NoDesc") : (item as WeaponSkinItemDefinitionV2).Description;
            t.gameObject.SetActive(true);
            t.position = new Vector3(t.position.x, yPos, t.position.z);
        }

        public void SetDefaultSkin()
        {
            if (m_SelectedWeapon == default)
                return;

            SelectSkin(m_SelectedWeapon, "Default");
        }

        public void ShowSkinInfo(WeaponType type, string skinName)
        {
            MyModdedObject.GetObject<Transform>(13).gameObject.SetActive(false);
            MyModdedObject.GetObject<Text>(8).text = string.Empty;
            m_Description.text = OverhaulLocalizationController.GetTranslation("NoDesc");
            if (type == WeaponType.None || string.IsNullOrEmpty(skinName))
                return;

            IWeaponSkinItemDefinition item = m_Controller.Interface.GetSkinItem(type, skinName, ItemFilter.Everything, out _);
            if (item == null)
                return;

            MyModdedObject.GetObject<Text>(8).text = (item as WeaponSkinItemDefinitionV2).HasNameOverride ? (item as WeaponSkinItemDefinitionV2).OverrideName : skinName;

            MyModdedObject.GetObject<Transform>(9).gameObject.SetActive(item.GetModel(false, false) != null);
            MyModdedObject.GetObject<Transform>(10).gameObject.SetActive(item.GetModel(false, true) != null || item.GetWeaponType() != WeaponType.Sword || (item as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer);
            MyModdedObject.GetObject<Transform>(11).gameObject.SetActive(item.GetModel(true, false) != null || item.GetWeaponType() == WeaponType.Bow);
            MyModdedObject.GetObject<Transform>(12).gameObject.SetActive(item.GetModel(true, true) != null || item.GetWeaponType() != WeaponType.Sword || item.GetWeaponType() == WeaponType.Bow || (item as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer);
            MyModdedObject.GetObject<Transform>(13).gameObject.SetActive(true);

            if (!string.IsNullOrEmpty((item as WeaponSkinItemDefinitionV2).Description))
                m_Description.text = (item as WeaponSkinItemDefinitionV2).Description;
        }

        public void SetAllowEnemiesUseSkins(bool value)
        {
            SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Player.WeaponSkins.EnemiesUseSkins", true), value);
        }

        public void DebugSetInputFieldsValues(ModelOffset transform)
        {
            if (!IsOutfitSelection)
                return;

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
            if (m_LoadIndicatorFill == null)
                return;

            m_LoadIndicatorFill.fillAmount = progress;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SetMenuActive(false);

            if (m_LoadIndicatorTransform != null) m_LoadIndicatorTransform.gameObject.SetActive(IsPopulatingSkins);
            if (m_SkinsUpdateRefreshButtonCooldownFill != null) m_SkinsUpdateRefreshButtonCooldownFill.fillAmount = WeaponSkinsUpdater.IsDownloadingUpdateFiles ? WeaponSkinsUpdater.GetUpdateFilesDownloadProgress() : (m_UpdateSkinsButton.interactable ? WeaponSkinsUpdater.CooldownFillAmount : 0f);
        }
    }
}