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

        private const string ATVCatDiscord = "A TVCat#9940";
        private const string TabiDiscord = "[₮₳฿ł]#4233";
        private const string CaptainMeowDiscord = "капитан кошачьих#7399";
        private const string Igrok_X_XPDiscord = "Igrok_x_xp#2966";
        private const string SonicGlebDiscord = "SonicGleb#4862";
        private const string WaterDiscord = "Water#2977";
        private const string LostCatDiscord = "TheLostCat#8845";
        private const string ZoloRDiscord = "ZoloR#3380";
        private const string And = " and ";

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

        public static bool IsFirstPersonMoverSupported(FirstPersonMover firstPersonMover)
        {
            return !GameModeManager.IsInLevelEditor() && firstPersonMover != null && !firstPersonMover.IsMindSpaceCharacter;
        }

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
                (swordDetailedSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = WaterDiscord;
                (swordDetailedSkin as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer = true;
                (swordDetailedSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;

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
                (darkPastSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SonicGlebDiscord;
                (darkPastSwordSkin as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer = true;
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

                ModelOffset impSwordSkinOffset = new ModelOffset(new Vector3(-2.88f, 0f, -0.425f), new Vector3(90, 0, 0), Vector3.one * 0.5f);
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
                pojSkin.SetExclusivePlayerID("193564D7A14F9C33 F08DA308234126FB");
                _ = pojSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                _ = pojSkin.GetModel(true, true).Model.AddComponent<WeaponSkinFireAnimator>();
                (pojSkin as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer = true;
                (pojSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;
                (pojSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = ZoloRDiscord;

                ModelOffset ancientSwordSkinOffset = new ModelOffset(new Vector3(-0.35f, -0.55f, -1f), new Vector3(90, 2, 0), Vector3.one * 0.45f);
                IWeaponSkinItemDefinition ancientSwordSkin = Interface.NewSkinItem(WeaponType.Sword, "Ancient", ItemFilter.Exclusive);
                ancientSwordSkin.SetModel(AssetsController.GetAsset("AncientSword", OverhaulAssetsPart.WeaponSkins),
                    ancientSwordSkinOffset,
                    false,
                    false);
                ancientSwordSkin.SetModel(AssetsController.GetAsset("AncientSword", OverhaulAssetsPart.WeaponSkins),
                    ancientSwordSkinOffset,
                    true,
                    false);
                (ancientSwordSkin as WeaponSkinItemDefinitionV2).UseSingleplayerVariantInMultiplayer = true;
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
                ModelOffset violetViolenceSkinOffset = new ModelOffset(new Vector3(-0.75f, 0.65f, -0.85f), new Vector3(0, 90, 90), Vector3.one * 0.525f);
                ModelOffset violetViolenceSkinOffset2 = new ModelOffset(new Vector3(0.72f, -0.65f, -0.85f), new Vector3(0, -90, -90), Vector3.one * 0.525f);
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
                violetViolenceSwordSkin.SetExclusivePlayerID("193564D7A14F9C33");
                _ = violetViolenceSwordSkin.GetModel(true, false).Model.AddComponent<WeaponSkinFireAnimator>();
                _ = violetViolenceSwordSkin.GetModel(true, true).Model.AddComponent<WeaponSkinFireAnimator>();
                (violetViolenceSwordSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = TabiDiscord + And + Igrok_X_XPDiscord;
                //(violetViolenceSwordSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 5;                

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
                (steelSwordSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 2;
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
                doomSwordSkin.SetModel(AssetsController.GetAsset("P_DoomSword", OverhaulAssetsPart.WeaponSkins),
                    doomSwordSkinOffset,
                    true,
                    false);
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
                darkPastHammerSkin.SetModel(AssetsController.GetAsset("HammerSkinDarkPast", OverhaulAssetsPart.WeaponSkins),
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
                (toyHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SonicGlebDiscord;

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

                ModelOffset relicHammerSkinOffset = new ModelOffset(new Vector3(-1.275f, -0.075f, -0.05f), new Vector3(0, 0, 270), Vector3.one * 0.3f);
                IWeaponSkinItemDefinition relicHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Relic", ItemFilter.None);
                relicHammerSkin.SetModel(AssetsController.GetAsset("P_RelicHammer", OverhaulAssetsPart.WeaponSkins),
                    relicHammerSkinOffset,
                    false,
                    false);
                relicHammerSkin.SetModel(AssetsController.GetAsset("P_RelicHammer", OverhaulAssetsPart.WeaponSkins),
                    relicHammerSkinOffset,
                    true,
                    false);
                (relicHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;
                (relicHammerSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenFire = true;
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
                IWeaponSkinItemDefinition iHammerSkin = Interface.NewSkinItem(WeaponType.Hammer, "Igrok_x_xp", ItemFilter.None);
                iHammerSkin.SetModel(AssetsController.GetAsset("IgroksHammer", OverhaulAssetsPart.WeaponSkins),
                    iHammerSkinOffset,
                    false,
                    false);
                iHammerSkin.SetModel(AssetsController.GetAsset("IgroksHammer", OverhaulAssetsPart.WeaponSkins),
                    iHammerSkinOffset,
                    true,
                    false);
                (iHammerSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenNormal = true;
                (iHammerSkin as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenFire = true;
                (iHammerSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                // Duxa Bow 
                ModelOffset testBowSkinOffset = new ModelOffset(new Vector3(0.65f, -1.66f, 0.65f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition testBowSkin = Interface.NewSkinItem(WeaponType.Bow, "Plasmatic", ItemFilter.None);
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

                ModelOffset spearDarkPastSkinOffset = new ModelOffset(new Vector3(1.375f, -4.35f, 0.15f), new Vector3(0, 270, 0), Vector3.one);
                IWeaponSkinItemDefinition spearDarkPastSkin = Interface.NewSkinItem(WeaponType.Spear, "Dark Past", ItemFilter.None);
                spearDarkPastSkin.SetModel(AssetsController.GetAsset("DarkPastSpear", OverhaulAssetsPart.WeaponSkins),
                    spearDarkPastSkinOffset,
                    false,
                    false);
                spearDarkPastSkin.SetModel(AssetsController.GetAsset("DarkPastSpear", OverhaulAssetsPart.WeaponSkins),
                    spearDarkPastSkinOffset,
                    true,
                    false);
                (spearDarkPastSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = SonicGlebDiscord;

                ModelOffset ancientSpearSkinOffset = new ModelOffset(new Vector3(-0.8f, -1.1f, -0.5f), new Vector3(0, 270, 0), Vector3.one * 0.5f);
                IWeaponSkinItemDefinition ancientSpearSkin = Interface.NewSkinItem(WeaponType.Spear, "Ancient", ItemFilter.None);
                ancientSpearSkin.SetModel(AssetsController.GetAsset("AncientSpear", OverhaulAssetsPart.WeaponSkins),
                    ancientSpearSkinOffset,
                    false,
                    false);
                ancientSpearSkin.SetModel(AssetsController.GetAsset("AncientSpear", OverhaulAssetsPart.WeaponSkins),
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
                spearGoldSkin.SetModel(AssetsController.GetAsset("GoldSpear", OverhaulAssetsPart.WeaponSkins),
                    spearGoldSkinOffset,
                    true,
                    false);
                (spearGoldSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 2;
                (spearGoldSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                ModelOffset glSpearSkinOffset = new ModelOffset(new Vector3(0.5f, -2.35f, 0.035f), new Vector3(0, -90, 0), new Vector3(0.75f, 0.75f, 1f));
                IWeaponSkinItemDefinition glSpearSkin = Interface.NewSkinItem(WeaponType.Spear, "Gladiator", ItemFilter.None);
                glSpearSkin.SetModel(AssetsController.GetAsset("GladiatorSpear", OverhaulAssetsPart.WeaponSkins),
                    glSpearSkinOffset,
                    false,
                    false);
                glSpearSkin.SetModel(AssetsController.GetAsset("GladiatorSpear", OverhaulAssetsPart.WeaponSkins),
                    glSpearSkinOffset,
                    true,
                    false);
                (glSpearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 2;
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
                (bionicSpearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 2;
                (bionicSpearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;

                ModelOffset hazardSpearSkinOffset = new ModelOffset(new Vector3(0.8f, -2.66f, -0.03f), new Vector3(0, 90, 0), new Vector3(0.75f, 0.75f, 1f));
                IWeaponSkinItemDefinition hazardSpearSkin = Interface.NewSkinItem(WeaponType.Spear, "Hazard", ItemFilter.None);
                hazardSpearSkin.SetModel(AssetsController.GetAsset("HazardSpear", OverhaulAssetsPart.WeaponSkins),
                    hazardSpearSkinOffset,
                    false,
                    false);
                hazardSpearSkin.SetModel(AssetsController.GetAsset("HazardSpear", OverhaulAssetsPart.WeaponSkins),
                    hazardSpearSkinOffset,
                    true,
                    false);
                (hazardSpearSkin as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor = 2;
                (hazardSpearSkin as WeaponSkinItemDefinitionV2).AuthorDiscord = CaptainMeowDiscord;
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
                if (filter == ItemFilter.Everything || weaponSkinItem.IsUnlocked(false))
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