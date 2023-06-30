using OverhaulAPI;
using System;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinsImportedItemDefinition
    {
        public static WeaponSkinsImportedItemDefinition GetNew()
        {
            return new WeaponSkinsImportedItemDefinition()
            {
                Name = "New skin",
                Description = "lorem ipsum",

                Author = WeaponSkinsController.ATVCatDiscord,
                OnlyAvailableFor = string.Empty,
                IsDeveloperItem = false,

                OfWeaponType = WeaponType.Sword,
                BehaviourIndex = 0,

                ApplySingleplayerModelInMultiplayer = false,
                UseVanillaBowstrings = false,

                ApplyFavColorOnLaser = true,
                ForcedFavColorLaserIndex = -1,
                ApplyFavColorOnFire = false,
                ForcedFavColorFireIndex = 5,
                AnimateFire = false,

                Multiplier = 1f,
                Saturation = 0.75f,

                AssetBundleFileName = OverhaulAssetsController.ModAssetBundle_Skins,

                SingleplayerLaserModelName = "SwordSkinDarkPast",
                SingleplayerLaserModelOffset = ModelOffset.GetDefaultOffset(),
                SingleplayerFireModelName = "SwordSkinDarkPastFire",
                SingleplayerFireModelOffset = ModelOffset.GetDefaultOffset(),
                MultiplayerFireModelName = "SwordSkinDarkPastLBSFire",
                MultiplayerFireModelOffset = ModelOffset.GetDefaultOffset(),
                MultiplayerLaserModelName = "SwordSkinDarkPastLBS",
                MultiplayerLaserModelOffset = ModelOffset.GetDefaultOffset()
            };
        }

        public static WeaponSkinsImportedItemDefinition PortOld(WeaponSkinItemDefinitionV2 item)
        {
            return new WeaponSkinsImportedItemDefinition()
            {
                Name = (item as IWeaponSkinItemDefinition).GetItemName(),
                Description = item.Description,

                Author = item.AuthorDiscord,
                OnlyAvailableFor = (item as IWeaponSkinItemDefinition).GetExclusivePlayerID(),
                IsDeveloperItem = item.IsDeveloperItem,

                OfWeaponType = (item as IWeaponSkinItemDefinition).GetWeaponType(),
                BehaviourIndex = 0,

                ApplySingleplayerModelInMultiplayer = item.UseSingleplayerVariantInMultiplayer,
                UseVanillaBowstrings = item.UseVanillaBowStrings,

                ApplyFavColorOnLaser = !item.DontUseCustomColorsWhenNormal,
                ForcedFavColorLaserIndex = item.IndexOfForcedNormalVanillaColor,
                ApplyFavColorOnFire = !item.DontUseCustomColorsWhenFire,
                ForcedFavColorFireIndex = item.IndexOfForcedFireVanillaColor,
                AnimateFire = (item as IWeaponSkinItemDefinition).GetModel(true, false) != null && (item as IWeaponSkinItemDefinition).GetModel(true, false).Model != null && (item as IWeaponSkinItemDefinition).GetModel(true, false).Model.GetComponent<WeaponSkinFireAnimator>() != null,

                Multiplier = item.Multiplier,
                Saturation = item.Saturation,

                AssetBundleFileName = string.IsNullOrEmpty(item.OverrideAssetBundle) ? OverhaulAssetsController.ModAssetBundle_Skins : item.OverrideAssetBundle,

                SingleplayerLaserModelName = (item as IWeaponSkinItemDefinition).GetModel(false, false) != null && (item as IWeaponSkinItemDefinition).GetModel(false, false).Model != null ? (item as IWeaponSkinItemDefinition).GetModel(false, false).Model.name : "ImperialSword",
                SingleplayerLaserModelOffset = (item as IWeaponSkinItemDefinition).GetModel(false, false) != null && (item as IWeaponSkinItemDefinition).GetModel(false, false).Model != null ? (item as IWeaponSkinItemDefinition).GetModel(false, false).Offset : ModelOffset.GetDefaultOffset(),

                SingleplayerFireModelName = (item as IWeaponSkinItemDefinition).GetModel(true, false) != null && (item as IWeaponSkinItemDefinition).GetModel(true, false).Model != null ? (item as IWeaponSkinItemDefinition).GetModel(true, false).Model.name : "ImperialSword",
                SingleplayerFireModelOffset = (item as IWeaponSkinItemDefinition).GetModel(true, false) != null && (item as IWeaponSkinItemDefinition).GetModel(true, false).Model != null ? (item as IWeaponSkinItemDefinition).GetModel(true, false).Offset : ModelOffset.GetDefaultOffset(),

                MultiplayerFireModelName = (item as IWeaponSkinItemDefinition).GetModel(true, true) != null && (item as IWeaponSkinItemDefinition).GetModel(true, true).Model != null ? (item as IWeaponSkinItemDefinition).GetModel(true, true).Model.name : "ImperialSword",
                MultiplayerFireModelOffset = (item as IWeaponSkinItemDefinition).GetModel(true, true) != null && (item as IWeaponSkinItemDefinition).GetModel(true, true).Model != null ? (item as IWeaponSkinItemDefinition).GetModel(true, true).Offset : ModelOffset.GetDefaultOffset(),

                MultiplayerLaserModelName = (item as IWeaponSkinItemDefinition).GetModel(false, true) != null && (item as IWeaponSkinItemDefinition).GetModel(false, true).Model != null ? (item as IWeaponSkinItemDefinition).GetModel(false, true).Model.name : "ImperialSword",
                MultiplayerLaserModelOffset = (item as IWeaponSkinItemDefinition).GetModel(false, true) != null && (item as IWeaponSkinItemDefinition).GetModel(false, true).Model != null ? (item as IWeaponSkinItemDefinition).GetModel(false, true).Offset : ModelOffset.GetDefaultOffset(),

                MinVersion = Version.Parse("0.2.10.36")
            };
        }

        public static WeaponSkinsImportedItemDefinition GetNew(bool save)
        {
            WeaponSkinsImportedItemDefinition result = GetNew();
            WeaponSkinsController.CustomSkinsData.AllCustomSkins.Add(result);
            if (save) WeaponSkinsController.CustomSkinsData.SaveSkins();
            return result;
        }

        public bool CanBeAdded() => MinVersion == null || OverhaulVersion.ModVersion >= MinVersion;

        public string Name;
        public string Description;

        public string Author;
        public string OnlyAvailableFor;
        public bool IsDeveloperItem;

        public WeaponType OfWeaponType;
        public int BehaviourIndex;

        public bool ApplySingleplayerModelInMultiplayer;
        public bool UseVanillaBowstrings;

        public bool ApplyFavColorOnLaser;
        public int ForcedFavColorLaserIndex;
        public bool ApplyFavColorOnFire;
        public int ForcedFavColorFireIndex;
        public bool AnimateFire;

        public float Multiplier;
        public float Saturation;

        public string ParentTo;
        public Version MinVersion;

        public string AssetBundleFileName;

        public string SingleplayerLaserModelName;
        public ModelOffset SingleplayerLaserModelOffset;

        public string SingleplayerFireModelName;
        public ModelOffset SingleplayerFireModelOffset;

        public string MultiplayerLaserModelName;
        public ModelOffset MultiplayerLaserModelOffset;

        public string MultiplayerFireModelName;
        public ModelOffset MultiplayerFireModelOffset;

        public string CollideWithEnvironmentVFXAssetName;
        public int CountOfPreparedPooledPrefabObjects;
    }
}
