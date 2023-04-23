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

        public static WeaponSkinsImportedItemDefinition GetNew(bool save)
        {
            WeaponSkinsImportedItemDefinition result = GetNew();
            WeaponSkinsController.CustomSkinsData.AllCustomSkins.Add(result);
            if (save) WeaponSkinsController.CustomSkinsData.SaveSkins();
            return result;
        }

        public bool CanBeAdded()
        {
            return MinVersion == null || OverhaulVersion.ModVersion >= MinVersion;
        }

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

        public string SingleplayerLaserModelName;
        public ModelOffset SingleplayerLaserModelOffset;

        public string SingleplayerFireModelName;
        public ModelOffset SingleplayerFireModelOffset;

        public string MultiplayerLaserModelName;
        public ModelOffset MultiplayerLaserModelOffset;

        public string MultiplayerFireModelName;
        public ModelOffset MultiplayerFireModelOffset;
    }
}
