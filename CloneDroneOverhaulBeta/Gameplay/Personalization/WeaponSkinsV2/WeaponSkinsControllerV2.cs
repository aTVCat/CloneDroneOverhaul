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
        public static string EquipedSwordSkin;
        [OverhaulSettingAttribute("Player.WeaponSkins.Bow", "Default", !OverhaulVersion.IsDebugBuild)]
        public static string EquipedBowSkin;
        [OverhaulSettingAttribute("Player.WeaponSkins.Hammer", "Default", !OverhaulVersion.IsDebugBuild)]
        public static string EquipedHammerSkin;
        [OverhaulSettingAttribute("Player.WeaponSkins.Spear", "Default", !OverhaulVersion.IsDebugBuild)]
        public static string EquipedSpearSkin;

        public override void Initialize()
        {
            base.Initialize();

            Interface = this;
            addSkins();
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
                ModelOffset darkPastSwordSkinOffset = new ModelOffset(new Vector3(0, -0.05f, -0.8f), new Vector3(0, 90, 90), Vector3.one);
                IWeaponSkinItemDefinition darkPastSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Dark Past", ItemFilter.None);
                darkPastSwordSkin.SetModel(AssetsController.GetAsset("SwordSkinDarkPast", OverhaulAssetsPart.WeaponSkins),
                    darkPastSwordSkinOffset,
                    false,
                    false);

                // Voilet violence sword
                ModelOffset violetViolenceSkinOffset = new ModelOffset(new Vector3(-0.75f, 0.65f, -0.85f), new Vector3(0, 90, 90), Vector3.one * 0.525f);
                IWeaponSkinItemDefinition violetViolenceSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Violet Violence", ItemFilter.Exclusive);
                violetViolenceSwordSkin.SetModel(AssetsController.GetAsset("VioletViolenceGS", OverhaulAssetsPart.WeaponSkins),
                    violetViolenceSkinOffset,
                    false,
                    false);
                violetViolenceSwordSkin.SetModel(AssetsController.GetAsset("VioletViolenceGS", OverhaulAssetsPart.WeaponSkins),
                    violetViolenceSkinOffset,
                    false,
                    true);
                violetViolenceSwordSkin.SetModel(AssetsController.GetAsset("VioletViolenceGS", OverhaulAssetsPart.WeaponSkins),
                    violetViolenceSkinOffset,
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
                                if(itemName == EquipedSwordSkin)
                                {
                                    result.Add(weaponSkinItem);
                                }
                                break;
                            case WeaponType.Bow:
                                if (itemName == EquipedBowSkin)
                                {
                                    result.Add(weaponSkinItem);
                                }
                                break;
                            case WeaponType.Hammer:
                                if (itemName == EquipedHammerSkin)
                                {
                                    result.Add(weaponSkinItem);
                                }
                                break;
                            case WeaponType.Spear:
                                if (itemName == EquipedSpearSkin)
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