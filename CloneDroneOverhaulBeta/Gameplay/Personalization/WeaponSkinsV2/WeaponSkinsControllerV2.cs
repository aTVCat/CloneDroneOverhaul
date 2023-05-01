using CDOverhaul.HUD;
using OverhaulAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinsController : OverhaulGameplayController, IWeaponSkinsControllerV2
    {
        public IWeaponSkinsControllerV2 Interface
        {
            get;
            private set;
        }

        public static CustomWeaponSkinsData CustomSkinsData
        {
            get;
            private set;
        }

        public static readonly WeaponType[] SupportedWeapons = new WeaponType[]
        {
            WeaponType.Sword,
            WeaponType.Bow,
            WeaponType.Hammer,
            WeaponType.Spear
        };
        public static bool IsWeaponSupported(WeaponType weaponType) => SupportedWeapons.Contains(weaponType);
        public static WeaponType[] GetSupportedWeapons() => SupportedWeapons;

        public static bool HasUpdatedSkins;
        public static bool SkinsDataIsDirty;

        public const string ATVCatDiscord = "A TVCat#9940";
        public const string TabiDiscord = "[₮₳฿ł]#4233";
        public const string CaptainMeowDiscord = "капитан кошачьих#7399";
        public const string Igrok_X_XPDiscord = "Igrok_x_xp#2966";
        public const string SonicGlebDiscord = "SonicGleb#4862";
        public const string WaterDiscord = "Water#2977";
        public const string LostCatDiscord = "TheLostCat#8845";
        public const string ZoloRDiscord = "ZoloR#3380";
        public const string SharpDiscord = "Luceferus#2219";
        public const string HumanDiscord = "Human#8570";
        public const string HizDiscord = "TheHiz#6138";
        public const string KegaDiscord = "Mr. КеГ#3924";
        public const string DGKDiscord = "dukogpom#0969";
        public const string PsinaDiscord = "Psina#8702";
        public const string And = " and ";

        private static readonly List<IWeaponSkinItemDefinition> m_WeaponSkins = new List<IWeaponSkinItemDefinition>();
        private static readonly List<string> m_CustomAssetBundlesWithSkins = new List<string>();
        public static void DeleteCustomAssetBundleFiles()
        {
            if (m_CustomAssetBundlesWithSkins.IsNullOrEmpty())
            {
                return;
            }

            foreach(string path in m_CustomAssetBundlesWithSkins)
            {
                string fullPath = OverhaulMod.Core.ModDirectory + path;
                if (File.Exists(fullPath))
                {
                    try
                    {
                        string backupPath = OverhaulMod.Core.ModDirectory + "Assets/Download/Backup/" + path;
                        if (File.Exists(backupPath))
                        {
                            File.Delete(backupPath);
                        }
                        File.Move(fullPath, backupPath);
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        public const string VFX_ChangeSkinID = "WeaponSkinChangedVFX";
        public static FirstPersonMover RobotToPlayAnimationOn;

        [OverhaulSettingAttribute("Player.WeaponSkins.Sword", "Default", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedSwordSkin;
        [OverhaulSettingAttribute("Player.WeaponSkins.SwordVar", 0, !OverhaulVersion.IsDebugBuild)]
        public static int EquippedSwordSkinVariant;

        [OverhaulSettingAttribute("Player.WeaponSkins.Bow", "Default", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedBowSkin;
        [OverhaulSettingAttribute("Player.WeaponSkins.BowVar", 0, !OverhaulVersion.IsDebugBuild)]
        public static int EquippedBowSkinVariant;

        [OverhaulSettingAttribute("Player.WeaponSkins.Hammer", "Default", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedHammerSkin;
        [OverhaulSettingAttribute("Player.WeaponSkins.HammerVar", 0, !OverhaulVersion.IsDebugBuild)]
        public static int EquippedBowHammerVariant;

        [OverhaulSettingAttribute("Player.WeaponSkins.Spear", "Default", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedSpearSkin;
        [OverhaulSettingAttribute("Player.WeaponSkins.SpearVar", 0, !OverhaulVersion.IsDebugBuild)]
        public static int EquippedBowSpearVariant;

        [OverhaulSettingAttribute("Player.WeaponSkins.EnemiesUseSkins", false, !OverhaulVersion.IsDebugBuild)]
        public static bool AllowEnemiesWearSkins;

        [OverhaulSettingAttribute("Player.WeaponSkins.NoticedSkinsButton", false, !OverhaulVersion.IsDebugBuild)]
        public static bool HasNoticedSkinsButton;

        public static bool IsFirstPersonMoverSupported(FirstPersonMover firstPersonMover)
        {
            return !GameModeManager.IsInLevelEditor() && firstPersonMover != null && !firstPersonMover.IsMindSpaceCharacter && firstPersonMover.CharacterCategory != EnemyCategory.FleetAnalysisBots && firstPersonMover.CharacterCategory != EnemyCategory.FleetCommanders;
        }

        public override void Initialize()
        {
            base.Initialize();

            _ = OverhaulEventsController.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.PlayerSetAsFirstPersonMover, ApplySkinsOnCharacter);

            Interface = this;
            addSkins();
        }

        protected override void OnDisposed()
        {
            OverhaulEventsController.RemoveEventListener<FirstPersonMover>(OverhaulGameplayCoreController.PlayerSetAsFirstPersonMover, ApplySkinsOnCharacter);
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
            {
                return;
            }

            ApplySkinsOnCharacter(firstPersonMover);
        }

        public void ReImportCustomSkins(bool reloadData = true)
        {
            if(reloadData) CustomSkinsData = OverhaulDataBase.GetData<CustomWeaponSkinsData>("ImportedSkins", true, "Download/Permanent", HasUpdatedSkins);
            for (int i = m_WeaponSkins.Count - 1; i > -1; i--)
            {
                if (m_WeaponSkins[i] == null)
                {
                    continue;
                }

                WeaponSkinItemDefinitionV2 item = m_WeaponSkins[i] as WeaponSkinItemDefinitionV2;
                if (item.IsImportedSkin)
                {
                    m_WeaponSkins.RemoveAt(i);
                }
            }

            foreach (WeaponSkinsImportedItemDefinition customSkin in CustomSkinsData.AllCustomSkins)
            {
                string assetBundle = string.IsNullOrEmpty(customSkin.AssetBundleFileName) ? AssetsController.ModAssetBundle_Skins : customSkin.AssetBundleFileName;
                if(assetBundle != AssetsController.ModAssetBundle_Skins && !m_CustomAssetBundlesWithSkins.Contains(assetBundle))
                {
                    m_CustomAssetBundlesWithSkins.Add(assetBundle);
                }
                if (!AssetsController.HasAssetBundle(assetBundle))
                {
                    WeaponSkinsUpdater.DownloadAssetBundleThenAddSkin(customSkin, assetBundle);
                    return;
                }
                ImportSkin(customSkin, assetBundle);
            }
        }

        public void ImportSkin(WeaponSkinsImportedItemDefinition customSkin, string assetBundle)
        {
            if (customSkin == null || !customSkin.CanBeAdded())
            {
                return;
            }

            try
            {
                AddSkinQuick(customSkin.OfWeaponType, customSkin.Name, customSkin.Author, customSkin.SingleplayerLaserModelName, customSkin.SingleplayerFireModelName, customSkin.MultiplayerLaserModelName, customSkin.MultiplayerFireModelName, assetBundle);
                WeaponSkinItemDefinitionV2 item = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
                item.IsImportedSkin = true;
                item.ReparentToBodypart = customSkin.ParentTo;
                item.OverrideAssetBundle = assetBundle;
                item.CollideWithEnvironmentVFXAssetName = customSkin.CollideWithEnvironmentVFXAssetName;
                AddSkinCustomVFXQuick();
                SetSkinDescriptionQuick(null, customSkin.Description);
                SetSkinColorParametersQuick(customSkin.ApplyFavColorOnLaser, customSkin.ForcedFavColorLaserIndex, customSkin.ApplyFavColorOnFire, customSkin.ForcedFavColorFireIndex, customSkin.Saturation, customSkin.Multiplier, customSkin.AnimateFire);
                SetSkinExclusiveQuick(customSkin.OnlyAvailableFor);
                SetSkinMiscParametersQuick(customSkin.ApplySingleplayerModelInMultiplayer, customSkin.UseVanillaBowstrings, customSkin.IsDeveloperItem);
                SetSkinModelOffsetQuick(customSkin.SingleplayerLaserModelOffset, false, false);
                SetSkinModelOffsetQuick(customSkin.SingleplayerFireModelOffset, true, false);
                SetSkinModelOffsetQuick(customSkin.MultiplayerLaserModelOffset, false, true);
                SetSkinModelOffsetQuick(customSkin.MultiplayerFireModelOffset, true, true);

                switch (customSkin.BehaviourIndex)
                {
                    case 1:
                        AddBehaviourToAllSkinModelsQuick<MCBowSkinBehaviour>();
                        break;
                }

            }
            catch(Exception exc)
            {
                OverhaulDialogues.CreateDialogue("Cannot import skin", customSkin.Name + " cannot be imported.\nDetails: " + exc, 15, new Vector2(400, 500), null);
            }
        }

        /// <summary>
        /// Add a skin definition
        /// </summary>
        /// <param name="weaponType"></param>
        /// <param name="name"></param>
        /// <param name="author"></param>
        /// <param name="singleplayerNormalModel"></param>
        /// <param name="singleplayerFireModel"></param>
        /// <param name="multiplayerNormalModel"></param>
        /// <param name="multiplayerFireModel"></param>
        public void AddSkinQuick(WeaponType weaponType,
            string name,
            string author,
            string singleplayerNormalModel = null,
            string singleplayerFireModel = null,
            string multiplayerNormalModel = null,
            string multiplayerFireModel = null,
            string assetBundle = AssetsController.ModAssetBundle_Skins)
        {
            WeaponSkinItemDefinitionV2 skin = Interface.NewSkinItem(weaponType, name, ItemFilter.None) as WeaponSkinItemDefinitionV2;
            if (!string.IsNullOrEmpty(singleplayerNormalModel)) (skin as IWeaponSkinItemDefinition).SetModel(AssetsController.GetAsset<GameObject>(singleplayerNormalModel, assetBundle), null, false, false);
            if (!string.IsNullOrEmpty(singleplayerFireModel)) (skin as IWeaponSkinItemDefinition).SetModel(AssetsController.GetAsset<GameObject>(singleplayerFireModel, assetBundle), null, true, false);
            if (!string.IsNullOrEmpty(multiplayerNormalModel)) (skin as IWeaponSkinItemDefinition).SetModel(AssetsController.GetAsset<GameObject>(multiplayerNormalModel, assetBundle), null, false, true);
            if (!string.IsNullOrEmpty(multiplayerFireModel)) (skin as IWeaponSkinItemDefinition).SetModel(AssetsController.GetAsset<GameObject>(multiplayerFireModel, assetBundle), null, true, true);
            skin.AuthorDiscord = author;
        }

        /// <summary>
        /// Set recently skin model offset
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="fire"></param>
        /// <param name="multiplayer"></param>
        public void SetSkinModelOffsetQuick(ModelOffset offset,
            bool fire,
            bool multiplayer)
        {
            WeaponSkinItemDefinitionV2 item = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
            (item as IWeaponSkinItemDefinition).GetModel(fire, multiplayer).Offset = offset;
        }

        public void SetSkinModelVariant(GameObject model, byte variant, bool fire, bool multiplayer)
        {
            WeaponSkinItemDefinitionV2 item = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
            WeaponSkinModel theModel = (item as IWeaponSkinItemDefinition).GetModel(fire, multiplayer);
            theModel.SetModelVariant(model, variant);
        }

        public void AddSkinCustomVFXQuick()
        {
            WeaponSkinItemDefinitionV2 item = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
            WeaponSkinsCustomVFXController.PrepareCustomVFXForSkin(item);
        }

        /// <summary>
        /// Mark recently added skin as exclusive
        /// </summary>
        /// <param name="playerIds"></param>
        public void SetSkinExclusiveQuick(string playerIds)
        {
            WeaponSkinItemDefinitionV2 item = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
            (item as IWeaponSkinItemDefinition).SetExclusivePlayerID(playerIds);
            (item as IWeaponSkinItemDefinition).SetFilter(ItemFilter.Exclusive);
        }

        /// <summary>
        /// Set color parameters of recently added skin
        /// </summary>
        /// <param name="applyFavColorNormal"></param>
        /// <param name="forcedColorIndexNormal"></param>
        /// <param name="applyFavColorFire"></param>
        /// <param name="forcedColorIndexFire"></param>
        /// <param name="saturation"></param>
        public void SetSkinColorParametersQuick(bool applyFavColorNormal = true, int forcedColorIndexNormal = -1, bool applyFavColorFire = false, int forcedColorIndexFire = 5, float saturation = 0.75f, float multipler = 1f, bool applyAnimToFireModel = false)
        {
            WeaponSkinItemDefinitionV2 item = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
            item.Saturation = saturation;
            item.Multiplier = multipler;
            item.IndexOfForcedFireVanillaColor = forcedColorIndexFire;
            item.IndexOfForcedNormalVanillaColor = forcedColorIndexNormal;
            item.DontUseCustomColorsWhenFire = !applyFavColorFire;
            item.DontUseCustomColorsWhenNormal = !applyFavColorNormal;
            if (applyAnimToFireModel && (item as IWeaponSkinItemDefinition).GetModel(true, false) != null) _ = (item as IWeaponSkinItemDefinition).GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
            if (applyAnimToFireModel && (item as IWeaponSkinItemDefinition).GetModel(true, true) != null) _ = (item as IWeaponSkinItemDefinition).GetModel(true, true).Model.AddComponent<WeaponSkinFireAnimator>();
        }

        /// <summary>
        /// Set misc parameters of recently added skin
        /// </summary>
        /// <param name="singleplayerVariantInMultiplayer"></param>
        /// <param name="vanillaBowStrings"></param>
        public void SetSkinMiscParametersQuick(bool singleplayerVariantInMultiplayer = false, bool vanillaBowStrings = true, bool isDevItem = false)
        {
            WeaponSkinItemDefinitionV2 item = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
            item.UseSingleplayerVariantInMultiplayer = singleplayerVariantInMultiplayer;
            item.UseVanillaBowStrings = vanillaBowStrings;
            item.IsDeveloperItem = isDevItem;
            if (isDevItem)
            {
                SetSkinExclusiveQuick("883CC7F4CA3155A3");
                SetSkinDescriptionQuick("DevItem");
            }
        }

        /// <summary>
        /// Set recently added skin description
        /// </summary>
        /// <param name="descriptionFilename"></param>
        public void SetSkinDescriptionQuick(string descriptionFilename = "", string descString = null)
        {
            if (!string.IsNullOrEmpty(descString))
            {
                WeaponSkinItemDefinitionV2 item1 = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
                item1.Description = descString;
                return;
            }

            string path = OverhaulMod.Core.ModDirectory + "Assets/WeaponSkinsDescriptions/" + descriptionFilename + ".txt";
            bool fileExists = File.Exists(path);
            if (!fileExists)
            {
                return;
            }

            // Make @loc[lang ID] separator
            StreamReader r = File.OpenText(path);
            string desc = r.ReadToEnd();
            r.Close();

            WeaponSkinItemDefinitionV2 item = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
            item.Description = desc;
        }

        public void AddBehaviourToAllSkinModelsQuick<T>() where T : WeaponSkinBehaviour
        {
            WeaponSkinItemDefinitionV2 item = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
            if ((item as IWeaponSkinItemDefinition).GetModel(false, false) != null) (item as IWeaponSkinItemDefinition).GetModel(false, false).Model.AddComponent<T>().OnPreLoad();
            if ((item as IWeaponSkinItemDefinition).GetModel(false, true) != null) (item as IWeaponSkinItemDefinition).GetModel(false, true).Model.AddComponent<T>().OnPreLoad();
            if ((item as IWeaponSkinItemDefinition).GetModel(true, false) != null) (item as IWeaponSkinItemDefinition).GetModel(true, false).Model.AddComponent<T>().OnPreLoad();
            if ((item as IWeaponSkinItemDefinition).GetModel(true, true) != null) (item as IWeaponSkinItemDefinition).GetModel(true, true).Model.AddComponent<T>().OnPreLoad();
        }

        #region Skins
        private void addSkins()
        {
            if (!OverhaulSessionController.GetKey<bool>("hasAddedSkins"))
            {
                OverhaulSessionController.SetKey("hasAddedSkins", true);

                CustomSkinsData = OverhaulDataBase.GetData<CustomWeaponSkinsData>("ImportedSkins", true, "Download/Permanent");
                PooledPrefabController.TurnObjectIntoPooledPrefab<VFXWeaponSkinSwitch>(AssetsController.GetAsset("VFX_SwitchSkin", OverhaulAssetsPart.WeaponSkins).transform, 5, VFX_ChangeSkinID);
                ReImportCustomSkins();

                return;
                // Detailed sword
                ModelOffset swordDetailedSkinOffset = new ModelOffset(new Vector3(0, 0, -0.7f),
                    new Vector3(0, 270, 270),
                    new Vector3(0.55f, 0.55f, 0.55f));
                IWeaponSkinItemDefinition swordDetailedSkin = Interface.NewSkinItem(WeaponType.Sword, "Detailed", ItemFilter.None);
                swordDetailedSkin.SetModel(AssetsController.GetAsset("SwordSkinDetailedOne", OverhaulAssetsPart.WeaponSkins),
                    swordDetailedSkinOffset,
                    false,
                    false); // Normal singleplayer model
                swordDetailedSkin.SetModel(AssetsController.GetAsset("SwordSkinDetailedOne_Fire", OverhaulAssetsPart.WeaponSkins),
                    swordDetailedSkinOffset,
                    true,
                    false); // Fire singleplayer model
                (swordDetailedSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = WaterDiscord;
                (swordDetailedSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;

                // Dark past sword
                ModelOffset darkPastSwordSkinOffset = new ModelOffset(new Vector3(-0.2f, -0.25f, -1f), new Vector3(0, 90, 90), Vector3.one);
                ModelOffset darkPastSwordSkinOffset2 = new ModelOffset(new Vector3(-0.05f, 0.05f, -0.15f), new Vector3(90f, 0f, 0f), Vector3.one);
                IWeaponSkinItemDefinition darkPastSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Dark Past", ItemFilter.None);
                darkPastSwordSkin.SetModel(AssetsController.GetAsset("SwordSkinDarkPast", OverhaulAssetsPart.WeaponSkins),
                    darkPastSwordSkinOffset,
                    false,
                    false);
                darkPastSwordSkin.SetModel(AssetsController.GetAsset("SwordSkinDarkPastFire", OverhaulAssetsPart.WeaponSkins),
                    darkPastSwordSkinOffset,
                    true,
                    false);
                darkPastSwordSkin.SetModel(AssetsController.GetAsset("SwordSkinDarkPastLBS", OverhaulAssetsPart.WeaponSkins),
                    darkPastSwordSkinOffset2,
                    false,
                    true);
                darkPastSwordSkin.SetModel(AssetsController.GetAsset("SwordSkinDarkPastLBSFire", OverhaulAssetsPart.WeaponSkins),
                    darkPastSwordSkinOffset2,
                    true,
                    true);
                (darkPastSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SonicGlebDiscord;
                (darkPastSwordSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;

                ModelOffset redMetalSwordSkinOffset = new ModelOffset(new Vector3(0.45f, 0.05f, -2.55f), new Vector3(0, 90, 90), Vector3.one);
                IWeaponSkinItemDefinition redMetalSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Red Metal", ItemFilter.None);
                redMetalSwordSkin.SetModel(AssetsController.GetAsset("RedMetalSword", OverhaulAssetsPart.WeaponSkins),
                    redMetalSwordSkinOffset,
                    false,
                    false);
                redMetalSwordSkin.SetModel(AssetsController.GetAsset("RedMetalSwordFire", OverhaulAssetsPart.WeaponSkins),
                    redMetalSwordSkinOffset,
                    true,
                    false);
                (redMetalSwordSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenFire = true;
                (redMetalSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;
                (redMetalSwordSkin as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer = true;

                ModelOffset terraBladeSkinOffset = new ModelOffset(new Vector3(0.48f, -1.1f, -0.55f), new Vector3(90, 2, 0), Vector3.one * 0.7f);
                IWeaponSkinItemDefinition terraBladeSkin = Interface.NewSkinItem(WeaponType.Sword, "TerraBlade", ItemFilter.None);
                terraBladeSkin.SetModel(AssetsController.GetAsset("P_TerraBlade", OverhaulAssetsPart.WeaponSkins),
                    terraBladeSkinOffset,
                    false,
                    false);
                terraBladeSkin.SetModel(AssetsController.GetAsset("P_TerraBladeFire", OverhaulAssetsPart.WeaponSkins),
                    terraBladeSkinOffset,
                    true,
                    false);
                //terraBladeSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                (terraBladeSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenFire = true;
                (terraBladeSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenNormal = true;
                (terraBladeSkin as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer = true;
                (terraBladeSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = Igrok_X_XPDiscord;

                ModelOffset impSwordSkinOffset = new ModelOffset(new Vector3(-2.8f, -0.005f, -0.425f), new Vector3(90, 0, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition impSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Imperial", ItemFilter.None);
                impSwordSkin.SetModel(AssetsController.GetAsset("ImperialSword", OverhaulAssetsPart.WeaponSkins),
                    impSwordSkinOffset,
                    false,
                    false);
                impSwordSkin.SetModel(AssetsController.GetAsset("ImperialSwordFire", OverhaulAssetsPart.WeaponSkins),
                    impSwordSkinOffset,
                    true,
                    false);
                _ = impSwordSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                (impSwordSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenFire = true;
                (impSwordSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenNormal = true;
                (impSwordSkin as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer = true;
                (impSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = string.Empty;

                ModelOffset pojSkinOffset = new ModelOffset(new Vector3(-0.15f, 0.36f, -1.2f), new Vector3(90, 2, 0), new Vector3(0.32f, 0.32f, 0.48f));
                IWeaponSkinItemDefinition pojSkin = Interface.NewSkinItem(WeaponType.Sword, "Pearl Of Justice", ItemFilter.Exclusive);
                pojSkin.SetModel(AssetsController.GetAsset("PearlOfJusticeSword", OverhaulAssetsPart.WeaponSkins),
                    pojSkinOffset,
                    false,
                    true);
                pojSkin.SetModel(AssetsController.GetAsset("PearlOfJusticeMiniSword", OverhaulAssetsPart.WeaponSkins),
                    pojSkinOffset,
                    false,
                    false);
                pojSkin.SetModel(AssetsController.GetAsset("PearlOfJusticeSword", OverhaulAssetsPart.WeaponSkins),
                    pojSkinOffset,
                    true,
                    true);
                pojSkin.SetModel(AssetsController.GetAsset("PearlOfJusticeMiniSword", OverhaulAssetsPart.WeaponSkins),
                    pojSkinOffset,
                    true,
                    false);
                pojSkin.SetExclusivePlayerID("193564D7A14F9C33 F08DA308234126FB 78E35D43F7CA4E5");
                _ = pojSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                _ = pojSkin.GetModel(true, true).Model.AddComponent<WeaponSkinFireAnimator>();
                (pojSkin as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer = true;
                (pojSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (pojSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = ZoloRDiscord;
                SetSkinDescriptionQuick("JusticePearl");

                ModelOffset yamatoSkinOffset = new ModelOffset(new Vector3(0.25f, 0.15f, -0.05f), new Vector3(0, 90, 90), Vector3.one * 0.4f);
                ModelOffset yamatoSkinOffsetM = new ModelOffset(new Vector3(0.3f, 0.225f, 0f), new Vector3(0, 90, 90), new Vector3(0.65f, 0.5f, 0.5f));
                IWeaponSkinItemDefinition yamatoSkin = Interface.NewSkinItem(WeaponType.Sword, "Yamato", ItemFilter.None);
                yamatoSkin.SetModel(AssetsController.GetAsset("YamatoSword", OverhaulAssetsPart.WeaponSkins),
                    yamatoSkinOffset,
                    false,
                    false);
                yamatoSkin.SetModel(AssetsController.GetAsset("YamatoSwordFire", OverhaulAssetsPart.WeaponSkins),
                    yamatoSkinOffset,
                    true,
                    false);
                yamatoSkin.SetModel(AssetsController.GetAsset("YamatoSword", OverhaulAssetsPart.WeaponSkins),
                    yamatoSkinOffsetM,
                    false,
                    true);
                yamatoSkin.SetModel(AssetsController.GetAsset("YamatoSwordFire", OverhaulAssetsPart.WeaponSkins),
                    yamatoSkinOffsetM,
                    true,
                    true);
                (yamatoSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                ModelOffset AmenohabakiriOffset = new ModelOffset(new Vector3(0f, 0.02f, -0.4f), new Vector3(90, 0, 0), new Vector3(0.4f, 0.35f, 0.5f));
                ModelOffset AmenohabakiriOffsetM = new ModelOffset(new Vector3(0f, 0.02f, -0.4f), new Vector3(90, 0, 0), new Vector3(0.42f, 0.42f, 0.52f));
                IWeaponSkinItemDefinition AmenohabakiriSkin = Interface.NewSkinItem(WeaponType.Sword, "Amenohabakiri", ItemFilter.None);
                AmenohabakiriSkin.SetModel(AssetsController.GetAsset("AmenohabakiriSword", OverhaulAssetsPart.WeaponSkins),
                    AmenohabakiriOffset,
                    false,
                    false);
                AmenohabakiriSkin.SetModel(AssetsController.GetAsset("EnmaSword", OverhaulAssetsPart.WeaponSkins),
                    AmenohabakiriOffset,
                    true,
                    false);
                AmenohabakiriSkin.SetModel(AssetsController.GetAsset("AmenohabakiriSword", OverhaulAssetsPart.WeaponSkins),
                    AmenohabakiriOffsetM,
                    false,
                    true);
                AmenohabakiriSkin.SetModel(AssetsController.GetAsset("EnmaSword", OverhaulAssetsPart.WeaponSkins),
                    AmenohabakiriOffsetM,
                    true,
                    true);
                (AmenohabakiriSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                ModelOffset ancientSwordSkinOffset = new ModelOffset(new Vector3(-0.35f, -0.55f, -1f), new Vector3(90, 2, 0), Vector3.one * 0.45f);
                IWeaponSkinItemDefinition ancientSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Ancient", ItemFilter.Exclusive);
                ancientSwordSkin.SetModel(AssetsController.GetAsset("AncientSword", OverhaulAssetsPart.WeaponSkins),
                    ancientSwordSkinOffset,
                    false,
                    false);
                ancientSwordSkin.SetModel(AssetsController.GetAsset("AncientSwordFire", OverhaulAssetsPart.WeaponSkins),
                    ancientSwordSkinOffset,
                    true,
                    false);
                (ancientSwordSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (ancientSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = TabiDiscord;

                ModelOffset hellFireSwordSkinOffset = new ModelOffset(new Vector3(0.06f, 0.01f, -1.1f), new Vector3(0, 92, 90), Vector3.one * 0.55f);
                IWeaponSkinItemDefinition hellFireSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Infernal", ItemFilter.None);
                hellFireSwordSkin.SetModel(AssetsController.GetAsset("P_HellFireSwordNormal", OverhaulAssetsPart.WeaponSkins),
                    hellFireSwordSkinOffset,
                    false,
                    false);
                hellFireSwordSkin.SetModel(AssetsController.GetAsset("P_HellFireSword", OverhaulAssetsPart.WeaponSkins),
                    hellFireSwordSkinOffset,
                    true,
                    false);
                (hellFireSwordSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenFire = true;
                (hellFireSwordSkin as WeaponSkinItemDefinitionV2).Saturation = 0.6f;
                (hellFireSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;
                (hellFireSwordSkin as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer = true;

                // Voilet violence sword
                ModelOffset violetViolenceSkinOffset = new ModelOffset(new Vector3(-0.75f, 0.625f, -0.85f), new Vector3(0, 90, 90), Vector3.one * 0.525f);
                ModelOffset violetViolenceSkinOffset2 = new ModelOffset(new Vector3(0.72f, -0.625f, -0.85f), new Vector3(0, -90, -90), Vector3.one * 0.525f);
                IWeaponSkinItemDefinition violetViolenceSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Violet Violence", ItemFilter.Exclusive);
                violetViolenceSwordSkin.SetModel(AssetsController.GetAsset("VioletViolence", OverhaulAssetsPart.WeaponSkins),
                    violetViolenceSkinOffset,
                    false,
                    false);
                violetViolenceSwordSkin.SetModel(AssetsController.GetAsset("VioletViolenceF", OverhaulAssetsPart.WeaponSkins),
                    violetViolenceSkinOffset,
                    true,
                    false);
                violetViolenceSwordSkin.SetModel(AssetsController.GetAsset("VioletViolenceGS", OverhaulAssetsPart.WeaponSkins),
                    violetViolenceSkinOffset2,
                    false,
                    true);
                violetViolenceSwordSkin.SetModel(AssetsController.GetAsset("VioletViolenceGSF", OverhaulAssetsPart.WeaponSkins),
                    violetViolenceSkinOffset2,
                    true,
                    true);
                violetViolenceSwordSkin.SetExclusivePlayerID("193564D7A14F9C33 78E35D43F7CA4E5 CEC4D8826697A677");
                _ = violetViolenceSwordSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                _ = violetViolenceSwordSkin.GetModel(true, true).Model.AddComponent<WeaponSkinFireAnimator>();
                (violetViolenceSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = TabiDiscord + And + Igrok_X_XPDiscord;
                SetSkinDescriptionQuick("VioletVio");
                //(violetViolenceSwordSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;

                ModelOffset frostmourneSkinOffset = new ModelOffset(new Vector3(0f, -0.03f, 1.3f), new Vector3(270, 180, 0), Vector3.one * 0.3f);
                ModelOffset frostmourneSkinOffsetM = new ModelOffset(new Vector3(0f, -0.03f, 1.5f), new Vector3(270, 180, 0), Vector3.one * 0.35f);
                IWeaponSkinItemDefinition frostmourneSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Frostmourne", ItemFilter.Exclusive);
                frostmourneSwordSkin.SetModel(AssetsController.GetAsset("FrostmourneSword", OverhaulAssetsPart.WeaponSkins),
                    frostmourneSkinOffset,
                    false,
                    false);
                frostmourneSwordSkin.SetModel(AssetsController.GetAsset("FrostmourneSwordFire", OverhaulAssetsPart.WeaponSkins),
                    frostmourneSkinOffset,
                    true,
                    false);
                frostmourneSwordSkin.SetModel(AssetsController.GetAsset("FrostmourneSword", OverhaulAssetsPart.WeaponSkins),
                    frostmourneSkinOffsetM,
                    false,
                    true);
                frostmourneSwordSkin.SetModel(AssetsController.GetAsset("FrostmourneSwordFire", OverhaulAssetsPart.WeaponSkins),
                    frostmourneSkinOffsetM,
                    true,
                    true);
                frostmourneSwordSkin.SetExclusivePlayerID("193564D7A14F9C33 FEA5A0978276D0FB 78E35D43F7CA4E5");
                _ = frostmourneSwordSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                _ = frostmourneSwordSkin.GetModel(false, false).Model.AddComponent<WeaponSkinFireAnimator>();
                (frostmourneSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;
                (frostmourneSwordSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (frostmourneSwordSkin as WeaponSkinItemDefinitionV2).IndexOfForcedNormalVanillaColor = 3;

                ModelOffset tgbSkinOffset = new ModelOffset(new Vector3(0f, 0f, -0.15f), new Vector3(90, 0, 0), Vector3.one * 0.2f);
                ModelOffset tgbSkinOffsetM = new ModelOffset(new Vector3(0f, 0f, -0.15f), new Vector3(90, 0, 0), Vector3.one * 0.23f);
                IWeaponSkinItemDefinition tgbSkin = Interface.NewSkinItem(WeaponType.Sword, "The Guardians Blade", ItemFilter.Exclusive);
                tgbSkin.SetModel(AssetsController.GetAsset("TheGuardiansBladeSword", OverhaulAssetsPart.WeaponSkins),
                    tgbSkinOffset,
                    false,
                    false);
                tgbSkin.SetModel(AssetsController.GetAsset("TheGuardiansBladeSwordFire", OverhaulAssetsPart.WeaponSkins),
                    tgbSkinOffset,
                    true,
                    false);
                tgbSkin.SetModel(AssetsController.GetAsset("TheGuardiansBladeSword", OverhaulAssetsPart.WeaponSkins),
                    tgbSkinOffsetM,
                    false,
                    true);
                tgbSkin.SetModel(AssetsController.GetAsset("TheGuardiansBladeSwordFire", OverhaulAssetsPart.WeaponSkins),
                    tgbSkinOffsetM,
                    true,
                    true);
                tgbSkin.SetExclusivePlayerID("FEA5A0978276D0FB 78E35D43F7CA4E5");
                (tgbSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;
                (tgbSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (tgbSkin as WeaponSkinItemDefinitionV2).IndexOfForcedNormalVanillaColor = 3;

                ModelOffset seSkinOffset = new ModelOffset(new Vector3(0f, -0.0275f, -0.35f), new Vector3(270, 180, 0), new Vector3(0.17f, 0.17f, 0.25f));
                ModelOffset seSkinOffsetM = new ModelOffset(new Vector3(0f, -0.0325f, -0.35f), new Vector3(270, 180, 0), new Vector3(0.2f, 0.2f, 0.3f));
                IWeaponSkinItemDefinition seSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Soul Eater", ItemFilter.Exclusive);
                seSwordSkin.SetModel(AssetsController.GetAsset("SoulEaterSword", OverhaulAssetsPart.WeaponSkins),
                    seSkinOffset,
                    false,
                    false);
                seSwordSkin.SetModel(AssetsController.GetAsset("SoulEaterSwordFire", OverhaulAssetsPart.WeaponSkins),
                    seSkinOffset,
                    true,
                    false);
                seSwordSkin.SetModel(AssetsController.GetAsset("SoulEaterSword", OverhaulAssetsPart.WeaponSkins),
                    seSkinOffsetM,
                    false,
                    true);
                seSwordSkin.SetModel(AssetsController.GetAsset("SoulEaterSwordFire", OverhaulAssetsPart.WeaponSkins),
                    seSkinOffsetM,
                    true,
                    true);
                _ = seSwordSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                seSwordSkin.SetExclusivePlayerID("193564D7A14F9C33 FEA5A0978276D0FB 78E35D43F7CA4E5");
                (seSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = ZoloRDiscord;
                (seSwordSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                SetSkinDescriptionQuick("SoulEater");

                ModelOffset LightSkinOffset = new ModelOffset(new Vector3(0f, 0f, 0.8f), new Vector3(90f, 0f, 0f), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition LightSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Light", ItemFilter.None);
                LightSwordSkin.SetModel(AssetsController.GetAsset("LightSword", OverhaulAssetsPart.WeaponSkins),
                    LightSkinOffset,
                    false,
                    false);
                LightSwordSkin.SetModel(AssetsController.GetAsset("LightSwordFire", OverhaulAssetsPart.WeaponSkins),
                    LightSkinOffset,
                    true,
                    false);
                (LightSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = TabiDiscord;
                (LightSwordSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;

                ModelOffset nezerHillOffset = new ModelOffset(new Vector3(0f, -0.055f, -0.2f), new Vector3(270, 180, 0), Vector3.one);
                IWeaponSkinItemDefinition nezerHillSkin = Interface.NewSkinItem(WeaponType.Sword, "Nether Hill", ItemFilter.None);
                nezerHillSkin.SetModel(AssetsController.GetAsset("NezerHillSword", OverhaulAssetsPart.WeaponSkins),
                   nezerHillOffset,
                    false,
                    false);
                nezerHillSkin.SetModel(AssetsController.GetAsset("NezerHillSwordFire", OverhaulAssetsPart.WeaponSkins),
                   nezerHillOffset,
                    true,
                    false);
                (nezerHillSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = ZoloRDiscord;
                (nezerHillSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (nezerHillSkin as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer = true;

                ModelOffset jetsamOffset = new ModelOffset(new Vector3(0f, 0f, -0.25f), new Vector3(90, 0, 0), new Vector3(0.3f, 0.3f, 0.34f));
                ModelOffset jetsamOffsetM = new ModelOffset(new Vector3(0f, 0f, -0.15f), new Vector3(90, 0, 0), new Vector3(0.3f, 0.3f, 0.34f));
                IWeaponSkinItemDefinition jetsamSkin = Interface.NewSkinItem(WeaponType.Sword, "Jet Sam", ItemFilter.None);
                jetsamSkin.SetModel(AssetsController.GetAsset("JetSamSwordV2", OverhaulAssetsPart.WeaponSkins),
                    jetsamOffset,
                    false,
                    false);
                jetsamSkin.SetModel(AssetsController.GetAsset("JetSamSwordV2Fire", OverhaulAssetsPart.WeaponSkins),
                    jetsamOffset,
                    true,
                    false);
                jetsamSkin.SetModel(AssetsController.GetAsset("JetSamSwordV2", OverhaulAssetsPart.WeaponSkins),
                    jetsamOffsetM,
                    false,
                    true);
                jetsamSkin.SetModel(AssetsController.GetAsset("JetSamSwordV2Fire", OverhaulAssetsPart.WeaponSkins),
                    jetsamOffsetM,
                    true,
                    true);
                (jetsamSkin as WeaponSkinItemDefinitionV2).OverrideName = "Jetstream";
                (jetsamSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenNormal = true;
                (jetsamSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (jetsamSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SharpDiscord;

                ModelOffset gvostOffset = new ModelOffset(new Vector3(0f, 0f, -0.25f), new Vector3(90, 0, 0), Vector3.one * 0.4f);
                ModelOffset gvostOffsetM = new ModelOffset(new Vector3(0f, 0f, -0.25f), new Vector3(90, 0, 0), Vector3.one * 0.45f);
                IWeaponSkinItemDefinition gvostSkin = Interface.NewSkinItem(WeaponType.Sword, "Spike", ItemFilter.None);
                gvostSkin.SetModel(AssetsController.GetAsset("GvostSword", OverhaulAssetsPart.WeaponSkins),
                    gvostOffset,
                    false,
                    false);
                gvostSkin.SetModel(AssetsController.GetAsset("GvostSwordFire", OverhaulAssetsPart.WeaponSkins),
                    gvostOffset,
                    true,
                    false);
                gvostSkin.SetModel(AssetsController.GetAsset("GvostSword", OverhaulAssetsPart.WeaponSkins),
                    gvostOffsetM,
                    false,
                    true);
                gvostSkin.SetModel(AssetsController.GetAsset("GvostSwordFire", OverhaulAssetsPart.WeaponSkins),
                    gvostOffsetM,
                    true,
                    true);
                _ = gvostSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                (gvostSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (gvostSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SharpDiscord;

                ModelOffset fishOffset = new ModelOffset(new Vector3(0f, 0f, -0.35f), new Vector3(90, 0, 0), new Vector3(0.45f, 0.5f, 0.4f));
                ModelOffset fishOffsetM = new ModelOffset(new Vector3(0f, 0f, -0.2f), new Vector3(90, 0, 0), new Vector3(0.5f, 0.55f, 0.45f));
                IWeaponSkinItemDefinition fishSkin = Interface.NewSkinItem(WeaponType.Sword, "Fish", ItemFilter.None);
                fishSkin.SetModel(AssetsController.GetAsset("FishSword", OverhaulAssetsPart.WeaponSkins),
                    fishOffset,
                    false,
                    false);
                fishSkin.SetModel(AssetsController.GetAsset("FishSwordFire", OverhaulAssetsPart.WeaponSkins),
                    fishOffset,
                    true,
                    false);
                fishSkin.SetModel(AssetsController.GetAsset("FishSword", OverhaulAssetsPart.WeaponSkins),
                    fishOffsetM,
                    false,
                    true);
                fishSkin.SetModel(AssetsController.GetAsset("FishSwordFire", OverhaulAssetsPart.WeaponSkins),
                    fishOffsetM,
                    true,
                    true);
                (fishSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SharpDiscord;

                // Steel
                ModelOffset steelSwordSkinOffset = new ModelOffset(new Vector3(-1.14f, -1.14f, 0.7f), Vector3.zero, Vector3.one);
                IWeaponSkinItemDefinition steelSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "The Steel", ItemFilter.None);
                steelSwordSkin.SetModel(AssetsController.GetAsset("P_Steel", OverhaulAssetsPart.WeaponSkins),
                    steelSwordSkinOffset,
                    false,
                    false);
                steelSwordSkin.SetModel(AssetsController.GetAsset("P_Steel", OverhaulAssetsPart.WeaponSkins),
                    steelSwordSkinOffset,
                    true,
                    false);
                (steelSwordSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (steelSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = TabiDiscord;

                ModelOffset ghSwordSkinOffset = new ModelOffset(new Vector3(0f, -0.85f, -1.2f), new Vector3(90, 2, 0), new Vector3(0.65f, 0.7f, 0.65f));
                IWeaponSkinItemDefinition ghSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Ghost Runner", ItemFilter.None);
                ghSwordSkin.SetModel(AssetsController.GetAsset("GhostRunnerNormalSword", OverhaulAssetsPart.WeaponSkins),
                    ghSwordSkinOffset,
                    false,
                    false);
                ghSwordSkin.SetModel(AssetsController.GetAsset("GhostRunnerSword", OverhaulAssetsPart.WeaponSkins),
                    ghSwordSkinOffset,
                    true,
                    false);
                (ghSwordSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenFire = true;
                (ghSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = Igrok_X_XPDiscord;

                ModelOffset doomSwordSkinOffset = new ModelOffset(new Vector3(-0.05f, -0.025f, -0.933f), new Vector3(90, 2, 0), Vector3.one * 0.42f);
                IWeaponSkinItemDefinition doomSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Doom", ItemFilter.None);
                doomSwordSkin.SetModel(AssetsController.GetAsset("P_DoomSword", OverhaulAssetsPart.WeaponSkins),
                    doomSwordSkinOffset,
                    false,
                    false);
                doomSwordSkin.SetModel(AssetsController.GetAsset("P_DoomSwordFire", OverhaulAssetsPart.WeaponSkins),
                    doomSwordSkinOffset,
                    true,
                    false);
                _ = darkPastSwordSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                (doomSwordSkin as WeaponSkinItemDefinitionV2).OverrideName = "Slayer";
                (doomSwordSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 2;
                (doomSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;
                (doomSwordSkin as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer = true;

                // Dark past hammer
                ModelOffset darkPastHammerSkinOffset = new ModelOffset(new Vector3(-2, -0.05f, 0.12f), new Vector3(0, 0, 270), Vector3.one);
                IWeaponSkinItemDefinition darkPastHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Dark Past", ItemFilter.None);
                darkPastHammerSkin.SetModel(AssetsController.GetAsset("HammerSkinDarkPast", OverhaulAssetsPart.WeaponSkins),
                    darkPastHammerSkinOffset,
                    false,
                    false);
                darkPastHammerSkin.SetModel(AssetsController.GetAsset("HammerSkinDarkPastFire", OverhaulAssetsPart.WeaponSkins),
                    darkPastHammerSkinOffset,
                    true,
                    false);
                (darkPastHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SonicGlebDiscord;
                (darkPastHammerSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;

                ModelOffset garbageBotHammerSkinOffset = new ModelOffset(new Vector3(-1, -0.05f, 0.06f), new Vector3(0, 0, 270), Vector3.one);
                IWeaponSkinItemDefinition garbageBotHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Garbage Bot", ItemFilter.None);
                garbageBotHammerSkin.SetModel(AssetsController.GetAsset("GarbageBotHammer", OverhaulAssetsPart.WeaponSkins),
                    garbageBotHammerSkinOffset,
                    false,
                    false);
                garbageBotHammerSkin.SetModel(AssetsController.GetAsset("GarbageBotHammerFire", OverhaulAssetsPart.WeaponSkins),
                    garbageBotHammerSkinOffset,
                    true,
                    false);
                (garbageBotHammerSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 2;
                (garbageBotHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = LostCatDiscord;

                ModelOffset bsHammerSkinOffset = new ModelOffset(new Vector3(-1.2f, -0.04f, 0.15f), new Vector3(0, 0, 270), Vector3.one);
                IWeaponSkinItemDefinition bsHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Blue Shroom", ItemFilter.None);
                bsHammerSkin.SetModel(AssetsController.GetAsset("BlueShroomHammer", OverhaulAssetsPart.WeaponSkins),
                    bsHammerSkinOffset,
                    false,
                    false);
                bsHammerSkin.SetModel(AssetsController.GetAsset("BlueShroomHammerFire", OverhaulAssetsPart.WeaponSkins),
                    bsHammerSkinOffset,
                    true,
                    false);
                (bsHammerSkin as WeaponSkinItemDefinitionV2).OverrideName = "Hammush";
                (bsHammerSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (bsHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = LostCatDiscord;

                ModelOffset toyHammerSkinOffset = new ModelOffset(new Vector3(-1.2f, -0.05f, 0.12f), new Vector3(0, 0, 270), Vector3.one);
                IWeaponSkinItemDefinition toyHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Toy", ItemFilter.Exclusive);
                toyHammerSkin.SetModel(AssetsController.GetAsset("ToyHammer", OverhaulAssetsPart.WeaponSkins),
                    toyHammerSkinOffset,
                    false,
                    false);
                toyHammerSkin.SetModel(AssetsController.GetAsset("ToyHammer", OverhaulAssetsPart.WeaponSkins),
                    toyHammerSkinOffset,
                    true,
                    false);
                toyHammerSkin.SetExclusivePlayerID("8A75F77DD769072C 78E35D43F7CA4E5");
                (toyHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SonicGlebDiscord;
                (toyHammerSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;

                ModelOffset voidCoreSkinOffset = new ModelOffset(new Vector3(-2.7f, 0.05f, -0.15f), new Vector3(0, 0, 270), Vector3.one);
                IWeaponSkinItemDefinition voidCoreSkin = Interface.NewSkinItem(WeaponType.Hammer, "Void Core", ItemFilter.None);
                voidCoreSkin.SetModel(AssetsController.GetAsset("VoidCoreHammer", OverhaulAssetsPart.WeaponSkins),
                    voidCoreSkinOffset,
                    false,
                    false);
                voidCoreSkin.SetModel(AssetsController.GetAsset("VoidCoreHammerFire", OverhaulAssetsPart.WeaponSkins),
                    voidCoreSkinOffset,
                    true,
                    false);
                (voidCoreSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;
                (voidCoreSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenFire = true;
                (voidCoreSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenNormal = true; // Tester's exclusive

                ModelOffset relicHammerSkinOffset = new ModelOffset(new Vector3(-0.7f, 0f, -0.025f), new Vector3(0, 0, 270), Vector3.one * 0.3f);
                IWeaponSkinItemDefinition relicHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Relic", ItemFilter.None);
                relicHammerSkin.SetModel(AssetsController.GetAsset("P_RelicHammer", OverhaulAssetsPart.WeaponSkins),
                    relicHammerSkinOffset,
                    false,
                    false);
                relicHammerSkin.SetModel(AssetsController.GetAsset("P_RelicHammerFire", OverhaulAssetsPart.WeaponSkins),
                    relicHammerSkinOffset,
                    true,
                    false);
                (relicHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;
                (relicHammerSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (relicHammerSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenNormal = true;

                ModelOffset wbdHammerSkinOffset = new ModelOffset(new Vector3(-1f, 0.03f, 0.22f), new Vector3(0, 0, 265), Vector3.one * 0.9f);
                IWeaponSkinItemDefinition wbdHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Water Based Drink", ItemFilter.None);
                wbdHammerSkin.SetModel(AssetsController.GetAsset("WBD", OverhaulAssetsPart.WeaponSkins),
                    wbdHammerSkinOffset,
                    false,
                    false);
                wbdHammerSkin.SetModel(AssetsController.GetAsset("WBDFire", OverhaulAssetsPart.WeaponSkins),
                    wbdHammerSkinOffset,
                    true,
                    false);
                (wbdHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = LostCatDiscord + And + ATVCatDiscord;
                (wbdHammerSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenFire = true;

                ModelOffset impHammerSkinOffset = new ModelOffset(new Vector3(-1.59f, -1.845f, -0.36f), new Vector3(0, 0, 270), Vector3.one * 0.5f);
                ModelOffset impHammerSkin2Offset = new ModelOffset(new Vector3(-1.59f, 0.825f, -0.455f), new Vector3(0, 0, 270), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition impHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Imperial", ItemFilter.None);
                impHammerSkin.SetModel(AssetsController.GetAsset("ImperialHammer", OverhaulAssetsPart.WeaponSkins),
                     impHammerSkinOffset,
                    false,
                    false);
                impHammerSkin.SetModel(AssetsController.GetAsset("ImperialHammerFire", OverhaulAssetsPart.WeaponSkins),
                     impHammerSkin2Offset,
                    true,
                    false);
                (impHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = string.Empty;
                (impHammerSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenFire = true;
                (impHammerSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenNormal = true;

                ModelOffset iHammerSkinOffset = new ModelOffset(new Vector3(-1.2f, -0.05f, 0.245f), new Vector3(0, 0, 270), Vector3.one);
                IWeaponSkinItemDefinition iHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Igrok_x_xp", ItemFilter.Exclusive);
                iHammerSkin.SetModel(AssetsController.GetAsset("IgroksHammer", OverhaulAssetsPart.WeaponSkins),
                    iHammerSkinOffset,
                    false,
                    false);
                iHammerSkin.SetModel(AssetsController.GetAsset("IgroksHammerFire", OverhaulAssetsPart.WeaponSkins),
                    iHammerSkinOffset,
                    true,
                    false);
                iHammerSkin.SetExclusivePlayerID("193564D7A14F9C33 6488A250901CD65C 78E35D43F7CA4E5 FEA5A0978276D0FB");
                (iHammerSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenNormal = true;
                (iHammerSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (iHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                ModelOffset kgHammerSkinOffset = new ModelOffset(new Vector3(0.5f, 0.01f, -0.005f), new Vector3(0, 0, 270), new Vector3(0.5f, 0.575f, 0.5f));
                IWeaponSkinItemDefinition kgHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Kings Grin", ItemFilter.None);
                kgHammerSkin.SetModel(AssetsController.GetAsset("KingsGrinHammer", OverhaulAssetsPart.WeaponSkins),
                    kgHammerSkinOffset,
                    false,
                    false);
                kgHammerSkin.SetModel(AssetsController.GetAsset("KingsGrinHammerFire", OverhaulAssetsPart.WeaponSkins),
                    kgHammerSkinOffset,
                    true,
                    false);
                (kgHammerSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (kgHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = ZoloRDiscord;

                ModelOffset tgHammerSkinOffset = new ModelOffset(new Vector3(-1.22f, 0f, -0.025f), new Vector3(0, 0, 270), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition tgHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "The Guardians Hammer", ItemFilter.Exclusive);
                tgHammerSkin.SetModel(AssetsController.GetAsset("TheGuardiansHammer", OverhaulAssetsPart.WeaponSkins),
                    tgHammerSkinOffset,
                    false,
                    false);
                tgHammerSkin.SetModel(AssetsController.GetAsset("TheGuardiansHammerFire", OverhaulAssetsPart.WeaponSkins),
                   tgHammerSkinOffset,
                    true,
                    false);
                tgHammerSkin.SetExclusivePlayerID("FEA5A0978276D0FB 78E35D43F7CA4E5");
                (tgHammerSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (tgHammerSkin as WeaponSkinItemDefinitionV2).IndexOfForcedNormalVanillaColor = 3;
                (tgHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                ModelOffset hammesusSkinOffset = new ModelOffset(new Vector3(0.3f, -0.05f, 0.135f), new Vector3(0, 0, 270), Vector3.one * 0.45f);
                IWeaponSkinItemDefinition hammesusSkin = Interface.NewSkinItem(WeaponType.Hammer, "Hammesus", ItemFilter.None);
                hammesusSkin.SetModel(AssetsController.GetAsset("Hammesus", OverhaulAssetsPart.WeaponSkins),
                    hammesusSkinOffset,
                    false,
                    false);
                hammesusSkin.SetModel(AssetsController.GetAsset("HammesusFire", OverhaulAssetsPart.WeaponSkins),
                    hammesusSkinOffset,
                    true,
                    false);
                (hammesusSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (hammesusSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SharpDiscord;

                ModelOffset SkalkaSkinOffset = new ModelOffset(new Vector3(-0.2f, -0.05f, 0.05f), new Vector3(0, 0, 270), Vector3.one * 0.9f);
                IWeaponSkinItemDefinition SkalkaSkin = Interface.NewSkinItem(WeaponType.Hammer, "Battledore", ItemFilter.None);
                SkalkaSkin.SetModel(AssetsController.GetAsset("SkalkaHammer", OverhaulAssetsPart.WeaponSkins),
                    SkalkaSkinOffset,
                    false,
                    false);
                SkalkaSkin.SetModel(AssetsController.GetAsset("SkalkaHammerFire", OverhaulAssetsPart.WeaponSkins),
                    SkalkaSkinOffset,
                    true,
                    false);
                (SkalkaSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (SkalkaSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SharpDiscord + And + ATVCatDiscord;

                // Duxa Bow 
                ModelOffset testBowSkinOffset = new ModelOffset(new Vector3(0.65f, -1.66f, 0.65f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBowSkin = Interface.NewSkinItem(WeaponType.Bow, "Plasma", ItemFilter.None);
                testBowSkin.SetModel(AssetsController.GetAsset("PlamaticBow", OverhaulAssetsPart.WeaponSkins),
                    testBowSkinOffset,
                    false,
                    false);
                (testBowSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = TabiDiscord;

                ModelOffset testBow2SkinOffset = new ModelOffset(new Vector3(0.35f, -1.5f, 0.45f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBow2Skin = Interface.NewSkinItem(WeaponType.Bow, "Neon", ItemFilter.None);
                testBow2Skin.SetModel(AssetsController.GetAsset("GlassBow", OverhaulAssetsPart.WeaponSkins),
                    testBow2SkinOffset,
                    false,
                    false);
                (testBow2Skin as WeaponSkinItemDefinitionV2).AuthorDiscord = TabiDiscord;

                ModelOffset testBow3SkinOffset = new ModelOffset(new Vector3(0.05f, -0.7f, 0f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBow3Skin = Interface.NewSkinItem(WeaponType.Bow, "Futuristic Iron", ItemFilter.None);
                testBow3Skin.SetModel(AssetsController.GetAsset("IronBow", OverhaulAssetsPart.WeaponSkins),
                    testBow3SkinOffset,
                    false,
                    false);
                testBow3Skin.GetModel(false, false).Model.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, -0.4f);
                (testBow3Skin as WeaponSkinItemDefinitionV2).AuthorDiscord = TabiDiscord;

                ModelOffset testBow4SkinOffset = new ModelOffset(new Vector3(0.25f, 0f, -0.3f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBow4Skin = Interface.NewSkinItem(WeaponType.Bow, "Dark Past", ItemFilter.None);
                testBow4Skin.SetModel(AssetsController.GetAsset("DarkPastBow", OverhaulAssetsPart.WeaponSkins),
                    testBow4SkinOffset,
                    false,
                    false);
                (testBow4Skin as WeaponSkinItemDefinitionV2).AuthorDiscord = SonicGlebDiscord;

                ModelOffset testBow5SkinOffset = new ModelOffset(new Vector3(0f, -0.9f, 0.1f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBow5Skin = Interface.NewSkinItem(WeaponType.Bow, "Magika Bow", ItemFilter.None);
                testBow5Skin.SetModel(AssetsController.GetAsset("MagicBow", OverhaulAssetsPart.WeaponSkins),
                    testBow5SkinOffset,
                    false,
                    false);
                (testBow5Skin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                ModelOffset testBow6SkinOffset = new ModelOffset(new Vector3(-0.85f, -0.95f, 0.05f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBow6Skin = Interface.NewSkinItem(WeaponType.Bow, "Hunter", ItemFilter.None);
                testBow6Skin.SetModel(AssetsController.GetAsset("TacticalBow", OverhaulAssetsPart.WeaponSkins),
                    testBow6SkinOffset,
                    false,
                    false);
                (testBow6Skin as WeaponSkinItemDefinitionV2).AuthorDiscord = Igrok_X_XPDiscord;

                ModelOffset testBow7SkinOffset = new ModelOffset(new Vector3(0.15f, 0f, 0f), new Vector3(0, 270, 0), Vector3.one * 0.52f);
                IWeaponSkinItemDefinition testBow7Skin = Interface.NewSkinItem(WeaponType.Bow, "Devil's Horn", ItemFilter.None);
                testBow7Skin.SetModel(AssetsController.GetAsset("DevilHornBow", OverhaulAssetsPart.WeaponSkins),
                    testBow7SkinOffset,
                    false,
                    false);
                (testBow7Skin as WeaponSkinItemDefinitionV2).AuthorDiscord = ZoloRDiscord;
                (testBow7Skin as WeaponSkinItemDefinitionV2).UseVanillaBowStrings = true;

                ModelOffset testBow8SkinOffset = new ModelOffset(new Vector3(0.09f, 0f, -0.1f), new Vector3(0, 90, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBow8Skin = Interface.NewSkinItem(WeaponType.Bow, "Skull", ItemFilter.None);
                testBow8Skin.SetModel(AssetsController.GetAsset("SkullBow", OverhaulAssetsPart.WeaponSkins),
                    testBow8SkinOffset,
                    false,
                    false);
                (testBow8Skin as WeaponSkinItemDefinitionV2).AuthorDiscord = ZoloRDiscord;
                (testBow8Skin as WeaponSkinItemDefinitionV2).Saturation = 0.55f;
                (testBow8Skin as WeaponSkinItemDefinitionV2).UseVanillaBowStrings = true;

                ModelOffset spearDarkPastSkinOffset = new ModelOffset(new Vector3(1.375f, -4.35f, 0.15f), new Vector3(0, 270, 0), Vector3.one);
                IWeaponSkinItemDefinition spearDarkPastSkin = Interface.NewSkinItem(WeaponType.Spear, "Dark Past", ItemFilter.None);
                spearDarkPastSkin.SetModel(AssetsController.GetAsset("DarkPastSpear", OverhaulAssetsPart.WeaponSkins),
                    spearDarkPastSkinOffset,
                    false,
                    false);
                spearDarkPastSkin.SetModel(AssetsController.GetAsset("DarkPastSpearFire", OverhaulAssetsPart.WeaponSkins),
                    spearDarkPastSkinOffset,
                    true,
                    false);
                _ = spearDarkPastSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                (spearDarkPastSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SonicGlebDiscord;
                (spearDarkPastSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;

                ModelOffset ancientSpearSkinOffset = new ModelOffset(new Vector3(-0.8f, -1.1f, -0.5f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition ancientSpearSkin = Interface.NewSkinItem(WeaponType.Spear, "Ancient", ItemFilter.None);
                ancientSpearSkin.SetModel(AssetsController.GetAsset("AncientSpear", OverhaulAssetsPart.WeaponSkins),
                    ancientSpearSkinOffset,
                    false,
                    false);
                ancientSpearSkin.SetModel(AssetsController.GetAsset("AncientSpearFire", OverhaulAssetsPart.WeaponSkins),
                    ancientSpearSkinOffset,
                    true,
                    false);
                (ancientSpearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = TabiDiscord;
                (ancientSpearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;

                ModelOffset breadSkinOffset = new ModelOffset(new Vector3(0.95f, -1.1f, -0.08f), new Vector3(0, 270, 0), new Vector3(0.5f, 0.5f, 0.9f));
                IWeaponSkinItemDefinition breadPastSkin = Interface.NewSkinItem(WeaponType.Spear, "Le baguet", ItemFilter.None);
                breadPastSkin.SetModel(AssetsController.GetAsset("P_Baget", OverhaulAssetsPart.WeaponSkins),
                    breadSkinOffset,
                    false,
                    false);
                breadPastSkin.SetModel(AssetsController.GetAsset("P_BagetFire", OverhaulAssetsPart.WeaponSkins),
                    breadSkinOffset,
                    true,
                    false);
                (breadPastSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SonicGlebDiscord + And + ATVCatDiscord;

                ModelOffset spearGoldSkinOffset = new ModelOffset(new Vector3(1.4f, -2.425f, 0.935f), new Vector3(0, 90, 0), Vector3.one * 0.6f);
                IWeaponSkinItemDefinition spearGoldSkin = Interface.NewSkinItem(WeaponType.Spear, "Gold", ItemFilter.None);
                spearGoldSkin.SetModel(AssetsController.GetAsset("GoldSpear", OverhaulAssetsPart.WeaponSkins),
                    spearGoldSkinOffset,
                    false,
                    false);
                spearGoldSkin.SetModel(AssetsController.GetAsset("GoldSpearFire", OverhaulAssetsPart.WeaponSkins),
                    spearGoldSkinOffset,
                    true,
                    false);
                (spearGoldSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (spearGoldSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                ModelOffset glSpearSkinOffset = new ModelOffset(new Vector3(0.5f, -2.35f, 0.035f), new Vector3(0, -90, 0), new Vector3(0.75f, 0.75f, 1f));
                IWeaponSkinItemDefinition glSpearSkin = Interface.NewSkinItem(WeaponType.Spear, "Gladiator", ItemFilter.None);
                glSpearSkin.SetModel(AssetsController.GetAsset("GladiatorSpear", OverhaulAssetsPart.WeaponSkins),
                    glSpearSkinOffset,
                    false,
                    false);
                glSpearSkin.SetModel(AssetsController.GetAsset("GladiatorSpearFire", OverhaulAssetsPart.WeaponSkins),
                    glSpearSkinOffset,
                    true,
                    false);
                (glSpearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (glSpearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                ModelOffset bionicSpearSkinOffset = new ModelOffset(new Vector3(-0.45f, -7.1075f, -0.03f), new Vector3(0, -90, 0), Vector3.one * 0.6f);
                IWeaponSkinItemDefinition bionicSpearSkin = Interface.NewSkinItem(WeaponType.Spear, "Bionic", ItemFilter.None);
                bionicSpearSkin.SetModel(AssetsController.GetAsset("BionicSpear", OverhaulAssetsPart.WeaponSkins),
                    bionicSpearSkinOffset,
                    false,
                    false);
                bionicSpearSkin.SetModel(AssetsController.GetAsset("BionicSpear", OverhaulAssetsPart.WeaponSkins),
                    bionicSpearSkinOffset,
                    true,
                    false);
                _ = bionicSpearSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                (bionicSpearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 2;
                (bionicSpearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                ModelOffset hazardSpearSkinOffset = new ModelOffset(new Vector3(0.8f, -2.66f, -0.03f), new Vector3(0, 90, 0), new Vector3(0.75f, 0.75f, 1f));
                IWeaponSkinItemDefinition hazardSpearSkin = Interface.NewSkinItem(WeaponType.Spear, "Hazard", ItemFilter.None);
                hazardSpearSkin.SetModel(AssetsController.GetAsset("HazardSpear", OverhaulAssetsPart.WeaponSkins),
                    hazardSpearSkinOffset,
                    false,
                    false);
                hazardSpearSkin.SetModel(AssetsController.GetAsset("HazardSpearFire", OverhaulAssetsPart.WeaponSkins),
                    hazardSpearSkinOffset,
                    true,
                    false);
                _ = hazardSpearSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                (hazardSpearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 2;
                (hazardSpearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                ModelOffset byonetSpearSkinOffset = new ModelOffset(new Vector3(-0.5f, -0.1f, -0.03f), new Vector3(3, 269, 0), Vector3.one * 0.4f);
                IWeaponSkinItemDefinition byonetSpearSkin = Interface.NewSkinItem(WeaponType.Spear, "Byonet", ItemFilter.None);
                byonetSpearSkin.SetModel(AssetsController.GetAsset("ByonetSpear", OverhaulAssetsPart.WeaponSkins),
                    byonetSpearSkinOffset,
                    false,
                    false);
                byonetSpearSkin.SetModel(AssetsController.GetAsset("ByonetSpearFire", OverhaulAssetsPart.WeaponSkins),
                    byonetSpearSkinOffset,
                    true,
                    false);
                _ = byonetSpearSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                (byonetSpearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (byonetSpearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord + And + ATVCatDiscord;

                ModelOffset bsSpearSkinOffset = new ModelOffset(new Vector3(0.9f, -2.065f, -0.185f), new Vector3(0, -90, 0), new Vector3(0.75f, 0.75f, 1f));
                IWeaponSkinItemDefinition bsSpearSkin = Interface.NewSkinItem(WeaponType.Spear, "Blue Shroom", ItemFilter.None);
                bsSpearSkin.SetModel(AssetsController.GetAsset("BlueShroomSpear", OverhaulAssetsPart.WeaponSkins),
                    bsSpearSkinOffset,
                    false,
                    false);
                bsSpearSkin.SetModel(AssetsController.GetAsset("BlueShroomSpearFire", OverhaulAssetsPart.WeaponSkins),
                    bsSpearSkinOffset,
                    true,
                    false);
                (bsSpearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (bsSpearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = LostCatDiscord;

                ModelOffset ksSpearSkinOffset = new ModelOffset(new Vector3(0.45f, 0f, 0f), new Vector3(0, -90, 0), new Vector3(0.75f, 0.75f, 1f));
                IWeaponSkinItemDefinition ksSpearSkin = Interface.NewSkinItem(WeaponType.Spear, "Kings Skull", ItemFilter.None);
                ksSpearSkin.SetModel(AssetsController.GetAsset("KingsSkullSpear", OverhaulAssetsPart.WeaponSkins),
                    ksSpearSkinOffset,
                    false,
                    false);
                ksSpearSkin.SetModel(AssetsController.GetAsset("KingsSkullSpearFire", OverhaulAssetsPart.WeaponSkins),
                    ksSpearSkinOffset,
                    true,
                    false);
                (ksSpearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (ksSpearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = ZoloRDiscord;

                ModelOffset shSpearSkinOffset = new ModelOffset(new Vector3(0.7f, 0f, 0f), new Vector3(0, -90, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition shSpearSkin = Interface.NewSkinItem(WeaponType.Spear, "Shrilling", ItemFilter.None);
                shSpearSkin.SetModel(AssetsController.GetAsset("ShrillingSpear", OverhaulAssetsPart.WeaponSkins),
                    shSpearSkinOffset,
                    false,
                    false);
                shSpearSkin.SetModel(AssetsController.GetAsset("ShrillingSpearFire", OverhaulAssetsPart.WeaponSkins),
                    shSpearSkinOffset,
                    true,
                    false);
                (shSpearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (shSpearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = TabiDiscord;

                ModelOffset opspearSkinOffset = new ModelOffset(new Vector3(-0.35f, 0.05f, 0.05f), new Vector3(0, -90, 0), Vector3.one);
                IWeaponSkinItemDefinition opspearSkin = Interface.NewSkinItem(WeaponType.Spear, "Overhaul Prototype", ItemFilter.None);
                opspearSkin.SetModel(AssetsController.GetAsset("OLSpear", OverhaulAssetsPart.WeaponSkins),
                    opspearSkinOffset,
                    false,
                    false);
                opspearSkin.SetModel(AssetsController.GetAsset("OLSpearFire", OverhaulAssetsPart.WeaponSkins),
                   opspearSkinOffset,
                    true,
                    false);
                (opspearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (opspearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = ATVCatDiscord;

                ModelOffset ftspearSkinOffset = new ModelOffset(new Vector3(-0.55f, 0.025f, 0.025f), new Vector3(0, -90, 0), new Vector3(0.5f, 0.5f, 0.4f));
                IWeaponSkinItemDefinition ftspearSkin = Interface.NewSkinItem(WeaponType.Spear, "Forgotten Technology", ItemFilter.None);
                ftspearSkin.SetModel(AssetsController.GetAsset("ForgottenTechnologySpear", OverhaulAssetsPart.WeaponSkins),
                    ftspearSkinOffset,
                    false,
                    false);
                ftspearSkin.SetModel(AssetsController.GetAsset("ForgottenTechnologySpearFire", OverhaulAssetsPart.WeaponSkins),
                   ftspearSkinOffset,
                    true,
                    false);
                _ = ftspearSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                (ftspearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (ftspearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedNormalVanillaColor = 2;
                (ftspearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                AddSkinQuick(WeaponType.Spear, "Minecraft", Igrok_X_XPDiscord, "MCTridentSpear", "MCTridentSpearFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.85f, -0.05f, 0.05f), new Vector3(0f, 270f, 0f), new Vector3(1f, 1f, 1.25f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.85f, -0.05f, 0.05f), new Vector3(0f, 270f, 0f), new Vector3(1f, 1f, 1.25f)), true, false);

                AddSkinQuick(WeaponType.Spear, "Arrow", KegaDiscord, "ArrowSpear", "ArrowSpearFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.4f, 0.05f, -0.05f), new Vector3(0f, 270f, 0f), new Vector3(1f, 1f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.4f, 0.05f, -0.05f), new Vector3(0f, 270f, 0f), new Vector3(1f, 1f, 1f)), true, false);

                AddSkinQuick(WeaponType.Spear, "Plant", SharpDiscord, "PlantSpear", "PlantSpearFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.8f, 0f, 0f), new Vector3(0f, 270f, 0f), new Vector3(0.5f, 0.5f, 0.55f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.8f, 0f, 0f), new Vector3(0f, 270f, 0f), new Vector3(0.5f, 0.5f, 0.6f)), true, false);
                SetSkinColorParametersQuick(false, -1, false, 5);
                IWeaponSkinItemDefinition plantSpearItem = m_WeaponSkins[m_WeaponSkins.Count - 1];
                _ = plantSpearItem.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();

                AddSkinQuick(WeaponType.Spear, "Angernight", ZoloRDiscord, "AngerNightSpear", "AngerNightSpearFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.25f, -0.05f, -0.05f), new Vector3(0f, 270f, 0f), new Vector3(0.65f, 0.65f, 0.9f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.25f, -0.05f, -0.05f), new Vector3(0f, 270f, 0f), new Vector3(0.65f, 0.65f, 0.9f)), true, false);
                SetSkinColorParametersQuick(true, -1, true, -1, 0.8f, 0.9f, true);
                SetSkinExclusiveQuick("193564D7A14F9C33");

                AddSkinQuick(WeaponType.Spear, "Cutie", PsinaDiscord, "CutieSpear", "CutieSpearFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.7f, -0.05f, -0.046f), new Vector3(0f, 270f, 0f), new Vector3(1f, 1f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.7f, -0.05f, -0.046f), new Vector3(0f, 270f, 0f), new Vector3(1f, 1f, 1f)), true, false);
                SetSkinColorParametersQuick(false, -1, false, 5);

                AddSkinQuick(WeaponType.Sword, "Minecraft", HumanDiscord, "MCSword", "MCSwordFire", "MCSwordLBS", "MCSwordFireLBS");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.1f, 0.045f, 0.125f), new Vector3(90f, 45f, 0f), new Vector3(1.325f, 1.325f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.1f, 0.045f, 0.125f), new Vector3(90f, 45f, 0f), new Vector3(1.325f, 1.325f, 1f)), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.1f, 0.045f, 0.125f), new Vector3(90f, 45f, 0f), new Vector3(1.325f, 1.325f, 1f)), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.1f, 0.045f, 0.125f), new Vector3(90f, 45f, 0f), new Vector3(1.325f, 1.325f, 1f)), true, true);

                AddSkinQuick(WeaponType.Sword, "Minecraft-Golden", HumanDiscord, "MCGoldenSword", "MCGoldenSwordFire", "MCGoldenSwordLBS", "MCGoldenSwordLBSFire");
                SetSkinExclusiveQuick("47A1CD84FD538A2E 7729A4C45405BF0E");
                SetSkinColorParametersQuick(false, -1, false, -1);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.1f, 0.045f, 0.125f), new Vector3(90f, 45f, 0f), new Vector3(1.325f, 1.325f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.1f, 0.045f, 0.125f), new Vector3(90f, 45f, 0f), new Vector3(1.325f, 1.325f, 1f)), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.1f, 0.045f, 0.125f), new Vector3(90f, 45f, 0f), new Vector3(1.325f, 1.325f, 1f)), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.1f, 0.045f, 0.125f), new Vector3(90f, 45f, 0f), new Vector3(1.325f, 1.325f, 1f)), true, true);

                AddSkinQuick(WeaponType.Bow, "Minecraft", HumanDiscord, "MCBow_1");
                AddBehaviourToAllSkinModelsQuick<MCBowSkinBehaviour>();
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.45f, 0.05f, 0.075f), new Vector3(0f, 0f, 135f), new Vector3(1.15f, 1.15f, 1f)), false, false);

                AddSkinQuick(WeaponType.Bow, "Nether Eye", ZoloRDiscord, "NetherEyeBow");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.25f, -0.025f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.5f, 0.5f, 0.5f)), false, false);
                SetSkinColorParametersQuick(false, -1);

                AddSkinQuick(WeaponType.Bow, "Banana", SharpDiscord + And + ATVCatDiscord, "BananaBow");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.05f, -0.025f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.25f, 0.25f, 0.25f)), false, false);

                AddSkinQuick(WeaponType.Bow, "Vasilek's", PsinaDiscord, "Vasilek'sBow");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.025f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.24f, 0.24f, 0.24f)), false, false);

                AddSkinQuick(WeaponType.Bow, "Extreme Acidity", CaptainMeowDiscord, "HighAcidityBow");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.05f, -0.025f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.5f, 0.525f, 0.5f)), false, false);
                SetSkinColorParametersQuick(false, -1);
                //SetSkinExclusiveQuick("FEA5A0978276D0FB 8A75F77DD769072C 7729A4C45405BF0E 193564D7A14F9C33 6488A250901CD65C CEC4D8826697A677 47A1CD84FD538A2E 931EF1496FB7986D 78E35D43F7CA4E5"); // Scrapped // upd 07 04 2023 - it is no longer scrapped since model got updated

                AddSkinQuick(WeaponType.Bow, "Cryprin", ZoloRDiscord, "CryprinBow");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.05f, 0.07f, -0.03f), new Vector3(0f, 0f, 0f), new Vector3(0.5f, 0.625f, 0.555f)), false, false);
                SetSkinColorParametersQuick(true, -1, true, -1, 0.8f, 0.9f, true);
                SetSkinExclusiveQuick("193564D7A14F9C33");

                AddSkinQuick(WeaponType.Sword, "Machette", HizDiscord + And + ATVCatDiscord, "MachetteSword", "MachetteSwordFire", "MachetteSword", "MachetteSwordFire");
                SetSkinColorParametersQuick(true, -1, false, -1);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.1f, 0.015f, -0.2f), new Vector3(90f, 0f, 0f), new Vector3(0.75f, 0.75f, 0.4f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.1f, 0.015f, -0.2f), new Vector3(90f, 0f, 0f), new Vector3(0.75f, 0.75f, 0.4f)), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.1f, 0.015f, -0.2f), new Vector3(90f, 0f, 0f), new Vector3(0.75f, 0.85f, 0.4f)), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.1f, 0.015f, -0.2f), new Vector3(90f, 0f, 0f), new Vector3(0.75f, 0.85f, 0.4f)), true, true);

                AddSkinQuick(WeaponType.Sword, "BBR_M-1", DGKDiscord, "BBR_S", "BBR_SFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.05f, 0.05f, 0.225f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.05f, 0.05f, 0.225f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), true, false);
                SetSkinDescriptionQuick("BBR");
                SetSkinColorParametersQuick(true, -1, false, -1, 0.75f, 1.25f, false);

                AddSkinQuick(WeaponType.Sword, "BBR_M-2", DGKDiscord, "BBR_MSword", "BBR_MSwordFire", "BBR_MSword", "BBR_MSwordFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.05f, 0.05f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.9f, 0.9f, 0.7f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.05f, 0.05f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.9f, 0.9f, 0.7f)), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.05f, 0.05f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 0.8f)), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.05f, 0.05f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 0.8f)), true, true);
                SetSkinDescriptionQuick("BBR_M");
                SetSkinColorParametersQuick(true, -1, false, -1, 0.75f, 1.25f, false);

                AddSkinQuick(WeaponType.Sword, "BBR_M-1_SE", DGKDiscord, "BBR_M-1_SESP", "BBR_M-1_SEFireSP", "BBR_M-1_SELBS", "BBR_M-1_SEFireLBS");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), true, true);
                SetSkinColorParametersQuick(true, -1, false, -1, 0.75f, 1.2f, false);
                SetSkinExclusiveQuick("862796793F71FD07");
                SetSkinDescriptionQuick("BBR_M_SE");

                AddSkinQuick(WeaponType.Hammer, "BBR_HM-1", DGKDiscord, "BBR_HM-1Hammer", "BBR_HM-1HammerFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.3f, 0f, -0.05f), new Vector3(0f, 0f, 270f), new Vector3(0.9f, 0.9f, 0.85f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.3f, 0f, -0.05f), new Vector3(0f, 0f, 270f), new Vector3(0.9f, 0.9f, 0.85f)), true, false);
                SetSkinColorParametersQuick(true, -1, false, -1, 0.75f, 1.25f, false);
                SetSkinDescriptionQuick("BBR_HM");

                AddSkinQuick(WeaponType.Hammer, "Delta Axe", SonicGlebDiscord, "DeltaAxe", "DeltaAxeFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.3f, 0f, 0f), new Vector3(0f, 0f, 270f), new Vector3(1f, 1f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.3f, 0f, 0f), new Vector3(0f, 0f, 270f), new Vector3(1f, 1f, 1f)), true, false);
                SetSkinColorParametersQuick(true, -1, false, -1, 0.75f, 1.2f, false);
                SetSkinExclusiveQuick("8A75F77DD769072C");

                AddSkinQuick(WeaponType.Hammer, "Delta Axe -Dev", SonicGlebDiscord, "DeltaAxe-Dev", "DeltaAxe-DevFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.3f, 0f, 0f), new Vector3(0f, 0f, 270f), new Vector3(1f, 1f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.3f, 0f, 0f), new Vector3(0f, 0f, 270f), new Vector3(1f, 1f, 1f)), true, false);
                SetSkinColorParametersQuick(false, -1, false, -1, 0.75f, 1.2f, false);
                SetSkinMiscParametersQuick(true, false, true);

                AddSkinQuick(WeaponType.Sword, "The destroyer of evil", SharpDiscord, "LinkSword", "LinkSwordFire", "LinkSword", "LinkSwordFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f) * 0.5f), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f) * 0.5f), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f) * 0.6f), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f) * 0.6f), true, true);
                SetSkinColorParametersQuick(true, -1, false, -1);

                AddSkinQuick(WeaponType.Sword, "Demon Blood", CaptainMeowDiscord, "DemonBlood", "DemonBloodFire", "DemonBlood", "DemonBloodFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0.01f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1.4f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0.01f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1.4f)), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.015f, 0.01f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1.1f, 1.1f, 1.5f)), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.015f, 0.01f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1.1f, 1.1f, 1.5f)), true, true);
                SetSkinColorParametersQuick(false, -1, false, -1, 0.75f);
                SetSkinExclusiveQuick("FEA5A0978276D0FB 8A75F77DD769072C 7729A4C45405BF0E 193564D7A14F9C33 6488A250901CD65C CEC4D8826697A677 47A1CD84FD538A2E 931EF1496FB7986D 78E35D43F7CA4E5");

                AddSkinQuick(WeaponType.Sword, "Justice & Splendor", CaptainMeowDiscord, "JusticeAndSplendor", "JusticeAndSplendorFire", "JusticeAndSplendor", "JusticeAndSplendorFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0.0125f, -0.1f), new Vector3(90f, 0f, 0f), new Vector3(1.05f, 0.95f, 1.05f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0.0125f, -0.1f), new Vector3(90f, 0f, 0f), new Vector3(1.05f, 0.95f, 1.05f)), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.015f, 0.01f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1.05f, 1.05f, 1.05f)), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(-0.015f, 0.01f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1.05f, 1.05f, 1.05f)), true, true);
                SetSkinColorParametersQuick(false, -1, false, -1, 0.75f);
                //SetSkinExclusiveQuick("FEA5A0978276D0FB 883CC7F4CA3155A3");
                (m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2).OverrideName = "Justice";

                AddSkinQuick(WeaponType.Sword, "Smuggling", WaterDiscord, "SmugglingSword", "SmugglingSwordFire", "SmugglingSword", "SmugglingSwordFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f) * 1.15f), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f) * 1.15f), true, true);
                SetSkinColorParametersQuick(true, -1, false, -1, 0.7f, 1.2f);
                SetSkinExclusiveQuick("6488A250901CD65C");

                AddSkinQuick(WeaponType.Sword, "Cutie", PsinaDiscord, "CutieSword", "CutieSwordFire", "CutieSword", "CutieSwordFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0.025f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0.025f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0.025f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f) * 1.15f), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0.025f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f) * 1.15f), true, true);
                SetSkinColorParametersQuick(false, -1, false, -1);

                AddSkinQuick(WeaponType.Sword, "Muramasa", PsinaDiscord, "MuramasaSword", "MuramasaFire", "MuramasaSword", "MuramasaFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.9f, 0.9f, 0.9f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.9f, 0.9f, 0.9f)), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), true, true);
                SetSkinColorParametersQuick(false, -1, false, 5, 0.75f, 0.9f);

                AddSkinQuick(WeaponType.Sword, "Meatley", PsinaDiscord, "MeatleySword", "MeatleySwordFire", "MeatleySword", "MeatleySwordFire");
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f)), true, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f) * 1.15f), false, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0f, 0f, 0f), new Vector3(90f, 0f, 0f), new Vector3(1f, 1f, 1f) * 1.15f), true, true);
                SetSkinColorParametersQuick(false, -1, false, -1);

                AddSkinQuick(WeaponType.Hammer, "Time Corruption", ZoloRDiscord + And + ATVCatDiscord, "TimeCorruptionHammer", "TimeCorruptionHammerFire");
                SetSkinColorParametersQuick(false, -1, false, -1);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.175f, 0.05f, -0.05f), new Vector3(0f, 0f, 270f), new Vector3(0.75f, 0.92f, 0.92f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.175f, 0.05f, -0.05f), new Vector3(0f, 0f, 270f), new Vector3(0.75f, 0.92f, 0.92f)), true, false);

                AddSkinQuick(WeaponType.Hammer, "Hammush 2", PsinaDiscord, "Hammush2Hammer", "Hammush2HammerFire");
                SetSkinColorParametersQuick(false, -1, false, -1);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.5f, 0f, 0f), new Vector3(0f, 0f, 270f), new Vector3(1f, 1f, 1f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.5f, 0f, 0f), new Vector3(0f, 0f, 270f), new Vector3(1f, 1f, 1f)), true, false);

                AddSkinQuick(WeaponType.Hammer, "Prynezis", ZoloRDiscord, "PrynezisHammer", "PrynezisFire");
                SetSkinColorParametersQuick(true, -1, true, -1, 0.8f, 0.9f, true);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.75f, 0f, 0f), new Vector3(0f, 0f, 270f), new Vector3(0.7f, 0.7f, 0.7f)), false, false);
                SetSkinModelOffsetQuick(new ModelOffset(new Vector3(0.75f, 0f, 0f), new Vector3(0f, 0f, 270f), new Vector3(0.7f, 0.7f, 0.7f)), true, false);
                SetSkinExclusiveQuick("193564D7A14F9C33");

                ReImportCustomSkins();
            }
        }

        #endregion

        public void ApplySkinsOnCharacter(Character character)
        {
            if (character == null || !(character is FirstPersonMover))
            {
                return;
            }
            FirstPersonMover firstPersonMover = character as FirstPersonMover;
            WeaponSkinsWearer wearer = firstPersonMover.GetComponent<WeaponSkinsWearer>();
            if (wearer == null)
            {
                _ = firstPersonMover.gameObject.AddComponent<WeaponSkinsWearer>();
                return;
            }
            wearer.SpawnSkins();
        }

        public static WeaponVariant GetVariant(bool fire, bool multiplayer)
        {
            if (!fire && !multiplayer)
            {
                return WeaponVariant.Default;
            }
            else if (!fire && multiplayer)
            {
                return WeaponVariant.DefaultMultiplayer;
            }
            else if (fire && !multiplayer)
            {
                return WeaponVariant.Fire;
            }
            return WeaponVariant.FireMultiplayer;
        }

        public static void ReloadAllModels()
        {
            //AssetLoader.ClearCache();
            _ = StaticCoroutineRunner.StartStaticCoroutine(reloadAllModelsCoroutine());
        }

        private static IEnumerator reloadAllModelsCoroutine()
        {
            int total = m_WeaponSkins.Count;
            int progress = 0;

            OverhaulLoadingScreen.Instance.SetScreenText("Reloading models...");
            OverhaulLoadingScreen.Instance.SetScreenFill(0f);
            OverhaulLoadingScreen.Instance.SetScreenActive(true);

            yield return new WaitForSecondsRealtime(1f);

            OverhaulLoadingScreen.Instance.SetScreenText("Getting skin files...");
            List<string> allAssetBundles = new List<string>();
            int skinsChecked = 0;
            foreach(IWeaponSkinItemDefinition def in m_WeaponSkins)
            {
                string assetBundle = string.IsNullOrEmpty((def as WeaponSkinItemDefinitionV2).OverrideAssetBundle) ? AssetsController.ModAssetBundle_Skins : (def as WeaponSkinItemDefinitionV2).OverrideAssetBundle;
                if (!allAssetBundles.Contains(assetBundle))
                {
                    allAssetBundles.Add(assetBundle);
                }

                skinsChecked++;
                OverhaulLoadingScreen.Instance.SetScreenFill(skinsChecked / (float)m_WeaponSkins.Count);
                yield return null;
            }

            OverhaulLoadingScreen.Instance.SetScreenText("Unloading skin files...");
            OverhaulLoadingScreen.Instance.SetScreenFill(0f);
            yield return new WaitForSecondsRealtime(0.5f);

            skinsChecked = 0;
            foreach (string assetBundle in allAssetBundles)
            {
                AssetsController.TryUnloadAssetBundle(assetBundle, true);
                skinsChecked++;
                OverhaulLoadingScreen.Instance.SetScreenFill(skinsChecked / (float)allAssetBundles.Count);
                yield return null;
            }

            OverhaulLoadingScreen.Instance.SetScreenText("Reloading models...");
            yield return new WaitForSecondsRealtime(0.5f);

            WeaponSkinsCustomVFXController.RemoveAllVFX();
            foreach (IWeaponSkinItemDefinition def in m_WeaponSkins)
            {
                OverhaulLoadingScreen.Instance.SetScreenText("Reloading: " + def.GetItemName());

                yield return null;
                WeaponSkinModel m1 = def.GetModel(false, false);
                if (m1 != null && m1.Model != null)
                {
                    string nameOfModel = m1.Model.name;
                    GameObject gm = AssetsController.GetAsset(nameOfModel, OverhaulAssetsPart.WeaponSkins);
                    m1.SetModelVariant(gm, 0);
                }

                yield return null;
                WeaponSkinModel m2 = def.GetModel(true, false);
                if (m2 != null && m2.Model != null)
                {
                    string nameOfModel = m2.Model.name;
                    GameObject gm = AssetsController.GetAsset(nameOfModel, OverhaulAssetsPart.WeaponSkins);
                    m2.SetModelVariant(gm, 0);
                }

                yield return null;
                WeaponSkinModel m3 = def.GetModel(false, true);
                if (m3 != null && m3.Model != null)
                {
                    string nameOfModel = m3.Model.name;
                    GameObject gm = AssetsController.GetAsset(nameOfModel, OverhaulAssetsPart.WeaponSkins);
                    m3.SetModelVariant(gm, 0);
                }

                yield return null;
                WeaponSkinModel m4 = def.GetModel(true, true);
                if (m4 != null && m4.Model != null)
                {
                    string nameOfModel = m4.Model.name;
                    GameObject gm = AssetsController.GetAsset(nameOfModel, OverhaulAssetsPart.WeaponSkins);
                    m4.SetModelVariant(gm, 0);
                }

                progress++;
                OverhaulLoadingScreen.Instance.SetScreenFill(progress / (float)total);
                yield return null;
            }

            OverhaulLoadingScreen.Instance.SetScreenText("Finishing...");
            OverhaulLoadingScreen.Instance.SetScreenFill(0f);

            WeaponSkinsController c = GetController<WeaponSkinsController>();
            if (c == null)
            {
                OverhaulLoadingScreen.Instance.SetScreenActive(false);
                yield break;
            }

            WeaponSkinsController.HasUpdatedSkins = true;
            c.ReImportCustomSkins();
            //if (WeaponSkinsMenu.SkinsSelection != null) WeaponSkinsMenu.SkinsSelection.SetUpdateButtonInteractableState(false);

            int charactersReloaded = 0;
            List<Character> characters = CharacterTracker.Instance.GetAllLivingCharacters();
            foreach (Character character in characters)
            {
                if (character != null && character is FirstPersonMover && IsFirstPersonMoverSupported(character as FirstPersonMover))
                {
                    WeaponSkinsController.SkinsDataIsDirty = true;
                    c.ApplySkinsOnCharacter(character);
                    OverhaulLoadingScreen.Instance.SetScreenFill(charactersReloaded / (float)characters.Count);
                }
                charactersReloaded++;
                yield return null;
            }

            OverhaulLoadingScreen.Instance.SetScreenActive(false);
            WeaponSkinsMenu.SkinsSelection.SetMenuActive(true);
            yield break;
        }

        public static void PortOldSkins()
        {
            List<WeaponSkinsImportedItemDefinition> importedSkins = new List<WeaponSkinsImportedItemDefinition>();
            foreach(IWeaponSkinItemDefinition item in m_WeaponSkins)
            {
                if(!(item as WeaponSkinItemDefinitionV2).IsImportedSkin)
                {
                    importedSkins.Add(WeaponSkinsImportedItemDefinition.PortOld((item as WeaponSkinItemDefinitionV2)));
                }
            }
            CustomSkinsData.AllCustomSkins.AddRange(importedSkins);

            for(int i = m_WeaponSkins.Count-1; i >-1; i--)
            {
                if (!(m_WeaponSkins[i] as WeaponSkinItemDefinitionV2).IsImportedSkin)
                {
                    m_WeaponSkins.RemoveAt(i);
                }
            }

            GetController<WeaponSkinsController>().ReImportCustomSkins();
        }

        public static string GetSkinsFileVersion()
        {
            return File.ReadAllText(OverhaulMod.Core.ModDirectory + "SkinsVersion.txt");
        }

        IWeaponSkinItemDefinition IWeaponSkinsControllerV2.NewSkinItem(WeaponType weapon, string skinName, ItemFilter filter)
        {
            IWeaponSkinItemDefinition newWeaponItem = new WeaponSkinItemDefinitionV2();
            newWeaponItem.SetItemName(skinName);
            newWeaponItem.SetWeaponType(weapon);
            newWeaponItem.SetFilter(filter);
            m_WeaponSkins.Add(newWeaponItem);
            return newWeaponItem;
        }

        IWeaponSkinItemDefinition IWeaponSkinsControllerV2.GetSkinItem(WeaponType weaponType, string skinName, ItemFilter filter, out ItemNullResult error)
        {
            IWeaponSkinItemDefinition result = null;
            error = ItemNullResult.Null;

            foreach (IWeaponSkinItemDefinition weaponSkinItem in m_WeaponSkins)
            {
                if (filter == ItemFilter.Everything || weaponSkinItem.IsUnlocked(false))
                {
                    if (weaponSkinItem.GetWeaponType() == weaponType && weaponSkinItem.GetItemName() == skinName)
                    {
                        result = weaponSkinItem;
                    }
                }
                else
                {
                    // Implement is locked by not exclusivity
                    error = ItemNullResult.LockedExclusive;
                }
            }

            return result;
        }

        IWeaponSkinItemDefinition[] IWeaponSkinsControllerV2.GetSkinItems(ItemFilter filter, WeaponType type = WeaponType.None)
        {
            List<IWeaponSkinItemDefinition> result = new List<IWeaponSkinItemDefinition>();
            if (filter == ItemFilter.Equipped)
            {
                foreach (IWeaponSkinItemDefinition weaponSkinItem in m_WeaponSkins)
                {
                    if (weaponSkinItem.IsUnlocked(OverhaulVersion.IsDebugBuild))
                    {
                        string itemName = weaponSkinItem.GetItemName();
                        switch (weaponSkinItem.GetWeaponType())
                        {
                            case WeaponType.Sword:
                                if (itemName == EquippedSwordSkin)
                                {
                                    result.Add(weaponSkinItem);
                                }
                                break;
                            case WeaponType.Bow:
                                if (itemName == EquippedBowSkin)
                                {
                                    result.Add(weaponSkinItem);
                                }
                                break;
                            case WeaponType.Hammer:
                                if (itemName == EquippedHammerSkin)
                                {
                                    result.Add(weaponSkinItem);
                                }
                                break;
                            case WeaponType.Spear:
                                if (itemName == EquippedSpearSkin)
                                {
                                    result.Add(weaponSkinItem);
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                foreach (IWeaponSkinItemDefinition weaponSkinItem in m_WeaponSkins)
                {
                    if ((weaponSkinItem.IsUnlocked(OverhaulVersion.IsDebugBuild) && type == WeaponType.None) || (weaponSkinItem.GetWeaponType() == type && (filter == ItemFilter.Everything || weaponSkinItem.GetFilter() == filter)))
                    {
                        result.Add(weaponSkinItem);
                    }
                }
            }
            return result.ToArray();
        }

        IWeaponSkinItemDefinition[] IWeaponSkinsControllerV2.GetSkinItems(FirstPersonMover firstPersonMover)
        {
            return Interface.GetSkinItems(ItemFilter.Equipped);
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}