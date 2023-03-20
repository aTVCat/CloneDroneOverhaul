using OverhaulAPI;
using System.Collections.Generic;
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

        private static readonly List<IWeaponSkinItemDefinition> m_WeaponSkins = new List<IWeaponSkinItemDefinition>();

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

        public override void Initialize()
        {
            base.Initialize();

            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.PlayerSetAsFirstPersonMover, ApplySkinsOnCharacter);

            Interface = this;
            addSkins();
        }

        protected override void OnDisposed()
        {
            OverhaulEventManager.RemoveEventListener<FirstPersonMover>(OverhaulGameplayCoreController.PlayerSetAsFirstPersonMover, ApplySkinsOnCharacter);
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
            {
                return;
            }

            ApplySkinsOnCharacter(firstPersonMover);
        }

        private void addSkins()
        {
            if (!OverhaulSessionController.GetKey<bool>("hasAddedSkins"))
            {
                OverhaulSessionController.SetKey("hasAddedSkins", true);

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

                // Dark past sword
                ModelOffset darkPastSwordSkinOffset = new ModelOffset(new Vector3(-0.2f, -0.25f, -1f), new Vector3(0, 90, 90), Vector3.one);
                IWeaponSkinItemDefinition darkPastSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Dark Past", ItemFilter.None);
                darkPastSwordSkin.SetModel(AssetsController.GetAsset("SwordSkinDarkPast", OverhaulAssetsPart.WeaponSkins),
                    darkPastSwordSkinOffset,
                    false,
                    false);
                darkPastSwordSkin.SetModel(AssetsController.GetAsset("SwordSkinDarkPast", OverhaulAssetsPart.WeaponSkins),
                    darkPastSwordSkinOffset,
                    true,
                    false);

                // Voilet violence sword
                ModelOffset violetViolenceSkinOffset = new ModelOffset(new Vector3(-0.75f, 0.65f, -0.85f), new Vector3(0, 90, 90), Vector3.one * 0.525f);
                ModelOffset violetViolenceSkinOffset2 = new ModelOffset(new Vector3(0.72f, -0.65f, -0.85f), new Vector3(0, -90, -90), Vector3.one * 0.525f);
                IWeaponSkinItemDefinition violetViolenceSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Violet Violence", ItemFilter.Exclusive);
                violetViolenceSwordSkin.SetModel(AssetsController.GetAsset("VioletViolence", OverhaulAssetsPart.WeaponSkins),
                    violetViolenceSkinOffset,
                    false,
                    false);
                violetViolenceSwordSkin.SetModel(AssetsController.GetAsset("VioletViolence", OverhaulAssetsPart.WeaponSkins),
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
                violetViolenceSwordSkin.SetExclusivePlayerID("193564D7A14F9C33");

                // Steel
                ModelOffset steelSwordSkinOffset = new ModelOffset(new Vector3(-1.14f, -1.14f, 0.7f), Vector3.zero, Vector3.one);
                IWeaponSkinItemDefinition steelSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "The Steel", ItemFilter.None);
                steelSwordSkin.SetModel(AssetsController.GetAsset("P_Steel", OverhaulAssetsPart.WeaponSkins),
                    steelSwordSkinOffset,
                    false,
                    false);

                // Dark past hammer
                ModelOffset darkPastHammerSkinOffset = new ModelOffset(new Vector3(-2, -0.05f, 0.12f), new Vector3(0, 0, 270), Vector3.one);
                IWeaponSkinItemDefinition darkPastHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Dark Past", ItemFilter.None);
                darkPastHammerSkin.SetModel(AssetsController.GetAsset("HammerSkinDarkPast", OverhaulAssetsPart.WeaponSkins),
                    darkPastHammerSkinOffset,
                    false,
                    false);

                // Dark past hammer
                ModelOffset toyHammerSkinOffset = new ModelOffset(new Vector3(-1.2f, -0.05f, 0.12f), new Vector3(0, 0, 270), Vector3.one);
                IWeaponSkinItemDefinition toyHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Toy", ItemFilter.None);
                toyHammerSkin.SetModel(AssetsController.GetAsset("ToyHammer", OverhaulAssetsPart.WeaponSkins),
                    toyHammerSkinOffset,
                    false,
                    false);
                toyHammerSkin.SetModel(AssetsController.GetAsset("ToyHammer", OverhaulAssetsPart.WeaponSkins),
                    toyHammerSkinOffset,
                    true,
                    false);
                toyHammerSkin.SetModel(AssetsController.GetAsset("ToyHammer", OverhaulAssetsPart.WeaponSkins),
                    toyHammerSkinOffset,
                    true,
                    true);
                toyHammerSkin.SetModel(AssetsController.GetAsset("ToyHammer", OverhaulAssetsPart.WeaponSkins),
                    toyHammerSkinOffset,
                    false,
                    true);

                // Duxa Bow 
                ModelOffset testBowSkinOffset = new ModelOffset(new Vector3(0.65f, -1.66f, 0.65f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBowSkin = Interface.NewSkinItem(WeaponType.Bow, "Plasmatic", ItemFilter.None);
                testBowSkin.SetModel(AssetsController.GetAsset("PlamaticBow", OverhaulAssetsPart.WeaponSkins),
                    testBowSkinOffset,
                    false,
                    false);

                ModelOffset testBow2SkinOffset = new ModelOffset(new Vector3(0.35f, -1.5f, 0.45f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBow2Skin = Interface.NewSkinItem(WeaponType.Bow, "Glass", ItemFilter.None);
                testBow2Skin.SetModel(AssetsController.GetAsset("GlassBow", OverhaulAssetsPart.WeaponSkins),
                    testBow2SkinOffset,
                    false,
                    false);

                ModelOffset testBow3SkinOffset = new ModelOffset(new Vector3(0.05f, -0.7f, 0f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBow3Skin = Interface.NewSkinItem(WeaponType.Bow, "Iron", ItemFilter.None);
                testBow3Skin.SetModel(AssetsController.GetAsset("IronBow", OverhaulAssetsPart.WeaponSkins),
                    testBow3SkinOffset,
                    false,
                    false);
                testBow3Skin.GetModel(false, false).Model.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, -0.4f);

                ModelOffset testBow4SkinOffset = new ModelOffset(new Vector3(0.25f, 0f, -0.3f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBow4Skin = Interface.NewSkinItem(WeaponType.Bow, "Dark Past", ItemFilter.None);
                testBow4Skin.SetModel(AssetsController.GetAsset("DarkPastBow", OverhaulAssetsPart.WeaponSkins),
                    testBow4SkinOffset,
                    false,
                    false);

                ModelOffset spearDarkPastSkinOffset = new ModelOffset(new Vector3(1.375f, -4.35f, 0.15f), new Vector3(0, 270, 0), Vector3.one);
                IWeaponSkinItemDefinition spearDarkPastSkin = Interface.NewSkinItem(WeaponType.Spear, "Dark Past", ItemFilter.None);
                spearDarkPastSkin.SetModel(AssetsController.GetAsset("DarkPastSpear", OverhaulAssetsPart.WeaponSkins),
                    spearDarkPastSkinOffset,
                    false,
                    false);
            }
        }

        public void ApplySkinsOnCharacter(Character character)
        {
            if(character == null || !(character is FirstPersonMover))
            {
                return;
            }
            FirstPersonMover firstPersonMover = character as FirstPersonMover;
            WeaponSkinsWearer wearer = firstPersonMover.GetComponent<WeaponSkinsWearer>();
            if(wearer == null)
            {
                wearer = firstPersonMover.gameObject.AddComponent<WeaponSkinsWearer>();
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

            foreach(IWeaponSkinItemDefinition weaponSkinItem in m_WeaponSkins)
            {
                if (weaponSkinItem.IsUnlocked(false))
                {
                    if ((filter == ItemFilter.Everything || weaponSkinItem.GetFilter() == filter) && weaponSkinItem.GetWeaponType() == weaponType && weaponSkinItem.GetItemName() == skinName)
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

        IWeaponSkinItemDefinition[] IWeaponSkinsControllerV2.GetSkinItems(ItemFilter filter)
        {
            List<IWeaponSkinItemDefinition> result = new List<IWeaponSkinItemDefinition>();
            if(filter == ItemFilter.Equipped)
            {
                foreach (IWeaponSkinItemDefinition weaponSkinItem in m_WeaponSkins)
                {
                    if (weaponSkinItem.IsUnlocked(OverhaulVersion.IsDebugBuild))
                    {
                        string itemName = weaponSkinItem.GetItemName();
                        switch (weaponSkinItem.GetWeaponType())
                        {
                            case WeaponType.Sword:
                                if(itemName == EquippedSwordSkin)
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
                    if (weaponSkinItem.IsUnlocked(OverhaulVersion.IsDebugBuild) && (filter == ItemFilter.Everything || weaponSkinItem.GetFilter() == filter))
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