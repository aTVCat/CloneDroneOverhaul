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
        public static readonly WeaponType[] SupportedWeapons = new WeaponType[]
        {
            WeaponType.Sword,
            WeaponType.Bow,
            WeaponType.Hammer,
            WeaponType.Spear
        };
        public static bool IsWeaponSupported(WeaponType weaponType) => SupportedWeapons.Contains(weaponType);

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
                return;

            foreach (string path in m_CustomAssetBundlesWithSkins)
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
                    continue;
            }
        }

        public const string VFX_ChangeSkinID = "WeaponSkinChangedVFX";
        public static FirstPersonMover RobotToPlayAnimationOn;

        [OverhaulSettingAttribute("Player.WeaponSkins.Sword", "Default", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedSwordSkin;
        [OverhaulSettingAttribute("Player.WeaponSkins.Bow", "Default", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedBowSkin;
        [OverhaulSettingAttribute("Player.WeaponSkins.Hammer", "Default", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedHammerSkin;
        [OverhaulSettingAttribute("Player.WeaponSkins.Spear", "Default", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedSpearSkin;

        [OverhaulSettingAttribute("Player.WeaponSkins.EnemiesUseSkins", false, !OverhaulVersion.IsDebugBuild)]
        public static bool AllowEnemiesWearSkins;
        [OverhaulSettingAttribute("Player.WeaponSkins.NoticedSkinsButton", false, !OverhaulVersion.IsDebugBuild)]
        public static bool HasNoticedSkinsButton;

        public static bool IsFirstPersonMoverSupported(FirstPersonMover firstPersonMover) =>
            !GameModeManager.IsInLevelEditor() &&
            firstPersonMover != null &&
            !firstPersonMover.IsMindSpaceCharacter &&
            firstPersonMover.CharacterCategory != EnemyCategory.FleetAnalysisBots &&
            firstPersonMover.CharacterCategory != EnemyCategory.FleetCommanders;

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
                return;

            ApplySkinsOnCharacter(firstPersonMover);
        }

        public void ReImportCustomSkins(bool reloadData = true)
        {
            m_WeaponSkins.Clear();
            if (reloadData)
            {
                if (CustomSkinsData != null && !CustomSkinsData.IsDisposed)
                    CustomSkinsData.Unload();

                CustomSkinsData = OverhaulDataBase.GetData<CustomWeaponSkinsData>("ImportedSkins", true, "Download/Permanent", HasUpdatedSkins);
            }

            foreach (WeaponSkinsImportedItemDefinition customSkin in CustomSkinsData.AllCustomSkins)
            {
                string assetBundle = string.IsNullOrEmpty(customSkin.AssetBundleFileName) ? OverhaulAssetsController.ModAssetBundle_Skins : customSkin.AssetBundleFileName;
                if (assetBundle != OverhaulAssetsController.ModAssetBundle_Skins && !m_CustomAssetBundlesWithSkins.Contains(assetBundle))
                    m_CustomAssetBundlesWithSkins.Add(assetBundle);

                if (!OverhaulAssetsController.DoesAssetBundleExist(assetBundle))
                {
                    WeaponSkinsUpdater.DownloadAssetBundleThenAddSkin(customSkin, assetBundle);
                    continue;
                }
                ImportSkin(customSkin, assetBundle);
            }
        }

        public void ImportSkin(WeaponSkinsImportedItemDefinition customSkin, string assetBundle)
        {
            if (customSkin == null || !customSkin.CanBeAdded())
                return;

            try
            {
                AddSkinQuick(customSkin.OfWeaponType, customSkin.Name, customSkin.Author, customSkin.SingleplayerLaserModelName, customSkin.SingleplayerFireModelName, customSkin.MultiplayerLaserModelName, customSkin.MultiplayerFireModelName, assetBundle);
                WeaponSkinItemDefinitionV2 item = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
                item.IsImportedSkin = true;
                item.ReparentToBodypart = customSkin.ParentTo;
                item.OverrideAssetBundle = assetBundle;
                item.CollideWithEnvironmentVFXAssetName = customSkin.CollideWithEnvironmentVFXAssetName;
                AddSkinCustomVFXQuick();
                SetSkinDescriptionQuick(customSkin.Description);
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
                    case 2:
                        AddBehaviourToAllSkinModelsQuick<MultipartWeaponBehaviour>();
                        break;
                }

            }
            catch (Exception exc)
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
            string singleplayerNormalModel,
            string singleplayerFireModel,
            string multiplayerNormalModel,
            string multiplayerFireModel,
            string assetBundle)
        {
            WeaponSkinItemDefinitionV2 skin = Interface.NewSkinItem(weaponType, name, ItemFilter.None) as WeaponSkinItemDefinitionV2;
            if (!string.IsNullOrEmpty(singleplayerNormalModel) && singleplayerNormalModel != "-") (skin as IWeaponSkinItemDefinition).SetModel(OverhaulAssetsController.GetAsset<GameObject>(singleplayerNormalModel, assetBundle), null, false, false);
            if (!string.IsNullOrEmpty(singleplayerFireModel) && singleplayerFireModel != "-") (skin as IWeaponSkinItemDefinition).SetModel(OverhaulAssetsController.GetAsset<GameObject>(singleplayerFireModel, assetBundle), null, true, false);
            if (!string.IsNullOrEmpty(multiplayerNormalModel) && multiplayerNormalModel != "-") (skin as IWeaponSkinItemDefinition).SetModel(OverhaulAssetsController.GetAsset<GameObject>(multiplayerNormalModel, assetBundle), null, false, true);
            if (!string.IsNullOrEmpty(multiplayerFireModel) && multiplayerFireModel != "-") (skin as IWeaponSkinItemDefinition).SetModel(OverhaulAssetsController.GetAsset<GameObject>(multiplayerFireModel, assetBundle), null, true, true);
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
            if ((item as IWeaponSkinItemDefinition).GetModel(fire, multiplayer) != null) (item as IWeaponSkinItemDefinition).GetModel(fire, multiplayer).Offset = offset;
        }

        public void SetSkinModelVariant(GameObject model,
            byte variant,
            bool fire,
            bool multiplayer)
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
        public void SetSkinColorParametersQuick(bool applyFavColorNormal = true,
            int forcedColorIndexNormal = -1,
            bool applyFavColorFire = false,
            int forcedColorIndexFire = 5,
            float saturation = 0.75f,
            float multipler = 1f,
            bool applyAnimToFireModel = false)
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
        public void SetSkinMiscParametersQuick(bool singleplayerVariantInMultiplayer = false,
            bool vanillaBowStrings = true,
            bool isDevItem = false)
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
        public void SetSkinDescriptionQuick(string description)
        {
            WeaponSkinItemDefinitionV2 item1 = m_WeaponSkins[m_WeaponSkins.Count - 1] as WeaponSkinItemDefinitionV2;
            item1.Description = description;
            return;
        }

        public void AddBehaviourToAllSkinModelsQuick<T>() where T : WeaponSkinBehaviour
        {
            if (m_WeaponSkins[m_WeaponSkins.Count - 1] is WeaponSkinItemDefinitionV2 item)
            {
                if ((item as IWeaponSkinItemDefinition).GetModel(false, false) != null) (item as IWeaponSkinItemDefinition).GetModel(false, false).Model.AddComponent<T>().OnPreLoad();
                if ((item as IWeaponSkinItemDefinition).GetWeaponType() != WeaponType.Bow && (item as IWeaponSkinItemDefinition).GetModel(false, true) != null) (item as IWeaponSkinItemDefinition).GetModel(false, true).Model.AddComponent<T>().OnPreLoad();
                if ((item as IWeaponSkinItemDefinition).GetWeaponType() == WeaponType.Sword && (item as IWeaponSkinItemDefinition).GetModel(true, false) != null) (item as IWeaponSkinItemDefinition).GetModel(true, false).Model.AddComponent<T>().OnPreLoad();
                if ((item as IWeaponSkinItemDefinition).GetWeaponType() == WeaponType.Sword && (item as IWeaponSkinItemDefinition).GetModel(true, true) != null) (item as IWeaponSkinItemDefinition).GetModel(true, true).Model.AddComponent<T>().OnPreLoad();
            }
        }

        private void addSkins()
        {
            if (!OverhaulSessionController.GetKey<bool>("hasAddedSkins"))
            {
                OverhaulSessionController.SetKey("hasAddedSkins", true);
                ;
                PooledPrefabController.TurnObjectIntoPooledPrefab<VFXWeaponSkinSwitch>(OverhaulAssetsController.GetAsset("VFX_SwitchSkin", OverhaulAssetPart.WeaponSkins).transform, 5, VFX_ChangeSkinID);
                ReImportCustomSkins(true);
            }
        }

        public void ApplySkinsOnCharacter(Character character)
        {
            if (character == null || !(character is FirstPersonMover))
                return;

            WeaponSkinsWearer wearer = character.GetComponent<WeaponSkinsWearer>();
            if (wearer == null)
            {
                _ = character.gameObject.AddComponent<WeaponSkinsWearer>();
                return;
            }
            wearer.SpawnSkins();
        }

        public static WeaponVariant GetVariant(bool fire, bool multiplayer)
        {
            if (!fire && !multiplayer)
                return WeaponVariant.Default;

            else if (!fire && multiplayer)
                return WeaponVariant.DefaultMultiplayer;

            else if (fire && !multiplayer)
                return WeaponVariant.Fire;

            return WeaponVariant.FireMultiplayer;
        }

        public static void ReloadAllModels()
        {
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
            foreach (IWeaponSkinItemDefinition def in m_WeaponSkins)
            {
                string assetBundle = string.IsNullOrEmpty((def as WeaponSkinItemDefinitionV2).OverrideAssetBundle) ? OverhaulAssetsController.ModAssetBundle_Skins : (def as WeaponSkinItemDefinitionV2).OverrideAssetBundle;
                if (!allAssetBundles.Contains(assetBundle))
                    allAssetBundles.Add(assetBundle);

                skinsChecked++;
                OverhaulLoadingScreen.Instance.SetScreenFill(skinsChecked / (float)m_WeaponSkins.Count);
                if (skinsChecked % 20 == 0) yield return null;
            }

            OverhaulLoadingScreen.Instance.SetScreenText("Unloading skin files...");
            OverhaulLoadingScreen.Instance.SetScreenFill(0f);
            yield return new WaitForSecondsRealtime(0.5f);

            skinsChecked = 0;
            foreach (string assetBundle in allAssetBundles)
            {
                _ = OverhaulAssetsController.TryUnloadAssetBundle(assetBundle, true);
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
                    GameObject gm = OverhaulAssetsController.GetAsset(nameOfModel, OverhaulAssetPart.WeaponSkins);
                    m1.SetModelVariant(gm, 0);
                }

                WeaponSkinModel m2 = def.GetModel(true, false);
                if (m2 != null && m2.Model != null)
                {
                    string nameOfModel = m2.Model.name;
                    GameObject gm = OverhaulAssetsController.GetAsset(nameOfModel, OverhaulAssetPart.WeaponSkins);
                    m2.SetModelVariant(gm, 0);
                }

                WeaponSkinModel m3 = def.GetModel(false, true);
                if (m3 != null && m3.Model != null)
                {
                    string nameOfModel = m3.Model.name;
                    GameObject gm = OverhaulAssetsController.GetAsset(nameOfModel, OverhaulAssetPart.WeaponSkins);
                    m3.SetModelVariant(gm, 0);
                }

                WeaponSkinModel m4 = def.GetModel(true, true);
                if (m4 != null && m4.Model != null)
                {
                    string nameOfModel = m4.Model.name;
                    GameObject gm = OverhaulAssetsController.GetAsset(nameOfModel, OverhaulAssetPart.WeaponSkins);
                    m4.SetModelVariant(gm, 0);
                }

                progress++;
                OverhaulLoadingScreen.Instance.SetScreenFill(progress / (float)total);
            }

            OverhaulLoadingScreen.Instance.SetScreenText("Finishing...");
            OverhaulLoadingScreen.Instance.SetScreenFill(0f);
            yield return null;

            WeaponSkinsController c = GetController<WeaponSkinsController>();
            if (c == null)
            {
                OverhaulLoadingScreen.Instance.SetScreenActive(false);
                yield break;
            }

            WeaponSkinsController.HasUpdatedSkins = true;
            //c.ReImportCustomSkins();

            CustomSkinsData = OverhaulDataBase.GetData<CustomWeaponSkinsData>("ImportedSkins", true, "Download/Permanent", true);
            m_WeaponSkins.Clear();

            int imported = 0;
            foreach (WeaponSkinsImportedItemDefinition customSkin in CustomSkinsData.AllCustomSkins)
            {
                string assetBundle = string.IsNullOrEmpty(customSkin.AssetBundleFileName) ? OverhaulAssetsController.ModAssetBundle_Skins : customSkin.AssetBundleFileName;
                if (assetBundle != OverhaulAssetsController.ModAssetBundle_Skins && !m_CustomAssetBundlesWithSkins.Contains(assetBundle))
                    m_CustomAssetBundlesWithSkins.Add(assetBundle);

                c.ImportSkin(customSkin, assetBundle);

                imported++;
                OverhaulLoadingScreen.Instance.SetScreenFill(imported / (float)CustomSkinsData.AllCustomSkins.Count);
                yield return null;
            }

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
            PersonalizationMenu.SkinsSelection.SetMenuActive(false);
            yield break;
        }

        public static void PortOldSkins()
        {
            List<WeaponSkinsImportedItemDefinition> importedSkins = new List<WeaponSkinsImportedItemDefinition>();
            foreach (IWeaponSkinItemDefinition item in m_WeaponSkins)
            {
                if (!(item as WeaponSkinItemDefinitionV2).IsImportedSkin)
                    importedSkins.Add(WeaponSkinsImportedItemDefinition.PortOld(item as WeaponSkinItemDefinitionV2));
            }
            CustomSkinsData.AllCustomSkins.AddRange(importedSkins);

            for (int i = m_WeaponSkins.Count - 1; i > -1; i--)
            {
                if (!(m_WeaponSkins[i] as WeaponSkinItemDefinitionV2).IsImportedSkin)
                    m_WeaponSkins.RemoveAt(i);
            }

            GetController<WeaponSkinsController>().ReImportCustomSkins();
        }

        public static string GetSkinsFileVersion() => OverhaulCore.ReadText(OverhaulMod.Core.ModDirectory + "SkinsVersion.txt");

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
                    if (weaponSkinItem.GetWeaponType() == weaponType && weaponSkinItem.GetItemName() == skinName)
                        result = weaponSkinItem;
                    else
                        error = ItemNullResult.LockedExclusive;
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
                                    result.Add(weaponSkinItem);
                                break;
                            case WeaponType.Bow:
                                if (itemName == EquippedBowSkin)
                                    result.Add(weaponSkinItem);
                                break;
                            case WeaponType.Hammer:
                                if (itemName == EquippedHammerSkin)
                                    result.Add(weaponSkinItem);
                                break;
                            case WeaponType.Spear:
                                if (itemName == EquippedSpearSkin)
                                    result.Add(weaponSkinItem);
                                break;
                        }
                    }
                }
            }
            else
                foreach (IWeaponSkinItemDefinition weaponSkinItem in m_WeaponSkins)
                    if ((weaponSkinItem.IsUnlocked(OverhaulVersion.IsDebugBuild) && type == WeaponType.None) || (weaponSkinItem.GetWeaponType() == type && (filter == ItemFilter.Everything || weaponSkinItem.GetFilter() == filter)))
                        result.Add(weaponSkinItem);

            return result.ToArray();
        }

        IWeaponSkinItemDefinition[] IWeaponSkinsControllerV2.GetSkinItems(FirstPersonMover firstPersonMover) => Interface.GetSkinItems(ItemFilter.Equipped);
    }
}