using OverhaulMod.UI;

namespace OverhaulMod.Utils
{
    public static class ModSettingsConstants
    {
        public const string UI_PREFIX = "ModUI_";

        /// <summary>
        /// <see cref="Engine.FadingVoxelManager"/>
        /// </summary>
        public const string ENABLE_VOXEL_FIRE_FADING = "EnableVoxelFireFading";

        /// <summary>
        /// <see cref="Engine.FadingVoxelManager"/>
        /// </summary>
        public const string ENABLE_VOXEL_BURNING = "EnableVoxelBurning";

        /// <summary>
        /// <see cref="Engine.ArenaRemodelManager"/>
        /// </summary>
        public const string ENABLE_ARENA_REMODEL = "EnableArenaRemodel";

        /// <summary>
        /// <see cref="Engine.TitleScreenCustomizationManager"/>
        /// </summary>
        public const string TITLE_SCREEN_MUSIC_TRACK_INDEX = "TitleScreenMusicTrackIndex";

        /// <summary>
        /// <see cref="Engine.TitleScreenCustomizationManager"/>
        /// </summary>
        public const string TITLE_SCREEN_BACKGROUND_TYPE = "TitleScreenBackgroundType";

        /// <summary>
        /// <see cref="ModUIManager"/>
        /// </summary>
        public const string SHOW_CHAPTER_SELECTION_MENU_REWORK = UI_PREFIX + nameof(UIChapterSelectMenuRework);

        /// <summary>
        /// <see cref="ModUIManager"/>
        /// </summary>
        public const string SHOW_ENDLESS_MODE_MENU = UI_PREFIX + nameof(UIEndlessModeMenu);

        /// <summary>
        /// <see cref="ModUIManager"/>
        /// </summary>
        public const string SHOW_CHALLENGES_MENU_REWORK = UI_PREFIX + nameof(UIChallengesMenuRework);

        /// <summary>
        /// <see cref="ModUIManager"/>
        /// </summary>
        public const string SHOW_WORKSHOP_BROWSER_REWORK = UI_PREFIX + nameof(UIWorkshopBrowser);

        /// <summary>
        /// <see cref="ModUIManager"/>
        /// </summary>
        public const string SHOW_ADVANCEMENTS_MENU_REWORK = UI_PREFIX + nameof(UIAdvancementsMenu);

        /// <summary>
        /// <see cref="ModUIManager"/>
        /// </summary>
        public const string SHOW_SETTINGS_MENU_REWORK = UI_PREFIX + nameof(UISettingsMenuRework);

        /// <summary>
        /// <see cref="ModUIManager"/>
        /// </summary>
        public const string SHOW_TITLE_SCREEN_REWORK = UI_PREFIX + nameof(UITitleScreenRework);

        /// <summary>
        /// <see cref="Engine.ModAudioManager"/>
        /// </summary>
        public const string ENABLE_REVERB_FILTER = "SoundReverbFilterEnabled";

        /// <summary>
        /// <see cref="Engine.ModAudioManager"/>
        /// </summary>
        public const string REVERB_FILTER_INTENSITY = "SoundReverbFilterIntensity";

        /// <summary>
        /// <see cref="ModCore"/>
        /// </summary>
        public const string ENABLE_TITLE_BAR_OVERHAUL = "TitleBarOverhaul";

        /// <summary>
        /// <see cref="Engine.CameraManager"/>
        /// </summary>
        public const string ENABLE_FIRST_PERSON_MODE = "EnableFirstPersonMode";

        /// <summary>
        /// <see cref="Engine.CameraManager"/>
        /// </summary>
        public const string CAMERA_MODE_TOGGLE_KEYBIND = "CameraModeToggleKeybind";

        /// <summary>
        /// <see cref="Content.UpdateManager"/>
        /// </summary>
        public const string CHECK_FOR_UPDATES_ON_STARTUP = "CheckForUpdatesOnStartup";

        /// <summary>
        /// <see cref="Engine.TitleScreenCustomizationManager"/>
        /// </summary>
        public const string INTRODUCE_TITLE_SCREEN_CUSTOMIZATION = "IntroduceTitleScreenCustomization";

        /// <summary>
        /// <see cref="ModManagers"/>
        /// </summary>
        public const string SHOW_MOD_SETUP_SCREEN_ON_START = "ShowModSetupScreenOnStart";

        /// <summary>
        /// <see cref="UI.UIVersionLabel"/>
        /// </summary>
        public const string SHOW_VERSION_LABEL = "ShowModVersionLabel";

        /// <summary>
        /// <see cref="Visuals.PostEffectsManager"/>
        /// </summary>
        public const string ENABLE_SSAO = "EnableSSAO";

        /// <summary>
        /// <see cref="UI.UIImageEffects"/>
        /// </summary>
        public const string ENABLE_VIGNETTE = "EnableVignette";

        /// <summary>
        /// <see cref="UI.UIImageEffects"/>
        /// </summary>
        public const string ENABLE_DITHERING = "EnableDithering";

        /// <summary>
        /// <see cref="Visuals.ParticleManager"/>
        /// </summary>
        public const string ENABLE_NEW_PARTICLES = "EnableParticles";

        /// <summary>
        /// <see cref="Visuals.Environment.SeveredBodyPartSparks"/>
        /// </summary>
        public const string ENABLE_GARBAGE_PARTICLES = "EnableGarbageParticles";

        /// <summary>
        /// <see cref="Engine.CameraRollingController"/>
        /// </summary>
        public const string ENABLE_CAMERA_ROLLING = "EnableCameraRolling";

        /// <summary>
        /// <see cref="Engine.CameraRollingController"/>
        /// </summary>
        public const string ENABLE_CAMERA_BOBBING = "EnableCameraBobbing";

        /// <summary>
        /// <see cref="Visuals.PostEffectsManager"/>
        /// </summary>
        public const string ENABLE_BLOOM = "EnableBloom";

        /// <summary>
        /// <see cref="Engine.LightningTransitionManager"/>
        /// </summary>
        public const string ENABLE_LIGHTNING_TRANSITION = "EnableLightningTransition";

        /// <summary>
        /// <see cref="Engine.AutoBuildManager"/>
        /// </summary>
        public const string AUTO_BUILD_KEY_BIND = "AutoBuildKeyBind";

        /// <summary>
        /// <see cref="Engine.CameraFOVController"/>
        /// </summary>
        public const string CAMERA_FOV_OFFSET = "CameraFOVOffset";

        /// <summary>
        /// <see cref="Content.Personalization.PersonalizationController"/>
        /// </summary>
        public const string SWORD_SKIN = "SwordSkinV2";

        /// <summary>
        /// <see cref="Content.Personalization.PersonalizationController"/>
        /// </summary>
        public const string BOW_SKIN = "BowSkinV2";

        /// <summary>
        /// <see cref="Content.Personalization.PersonalizationController"/>
        /// </summary>
        public const string HAMMER_SKIN = "HammerSkinV2";

        /// <summary>
        /// <see cref="Content.Personalization.PersonalizationController"/>
        /// </summary>
        public const string SPEAR_SKIN = "SpearSkinV2";

        /// <summary>
        /// <see cref="Content.Personalization.PersonalizationController"/>
        /// </summary>
        public const string SHIELD_SKIN = "ShieldSkinV2";

        /// <summary>
        /// <see cref="Visuals.RobotWeaponBag"/>
        /// </summary>
        public const string ENABLE_WEAPON_BAG = "EnableWeaponBag";

        /// <summary>
        /// <see cref="Patches.Behaviours.ColorsPatchBehaviour"/>
        /// </summary>
        public const string CHANGE_HIT_COLORS = "ChangeHitColors";

        /// <summary>
        /// <see cref="Visuals.Environment.WeatherManager"/>
        /// </summary>
        public const string ENABLE_WEATHER = "EnableWeather";

        /// <summary>
        /// <see cref="Visuals.Environment.WeatherManager"/>
        /// </summary>
        public const string FORCE_WEATHER_TYPE = "ForceWeatherType";

        /// <summary>
        /// <see cref="ModCore.ShowSpeakerName"/>
        /// </summary>
        public const string SHOW_SPEAKER_NAME = "ShowSpeakerName";

        /// <summary>
        /// <see cref="UI.UIFeedbackMenu"/>
        /// </summary>
        public const string HAS_EVER_SENT_FEEDBACK = "HasEverSentFeedback";

        /// <summary>
        /// <see cref="Visuals.Environment.FloatingDustManager"/>
        /// </summary>
        public const string ENABLE_FLOATING_DUST = "EnableFloatingDust";

        /// <summary>
        /// <see cref="Visuals.PostEffectsManager"/>
        /// </summary>
        public const string TWEAK_BLOOM = "TweakBloom";

        /// <summary>
        /// <see cref="Engine.TransitionManager"/>
        /// </summary>
        public const string OVERHAUL_SCENE_TRANSITIONS = "OverhaulSceneTransitions";

        /// <summary>
        /// <see cref="Engine.TransitionManager"/>
        /// </summary>
        public const string OVERHAUL_NON_SCENE_TRANSITIONS = "OverhaulNonSceneTransitions";

        /// <summary>
        /// <see cref="Engine.TransitionManager"/>
        /// </summary>
        public const string TRANSITION_ON_STARTUP = "TransitionOnStart";

        /// <summary>
        /// <see cref="Engine.AdvancedPhotoModeManager"/>
        /// </summary>
        public const string ADVANCED_PHOTO_MODE = "AdvancedPhotoMode";

        /// <summary>
        /// <see cref="Engine.RichPresenceManager"/>
        /// </summary>
        public const string ENABLE_RPC = "EnableRPC";

        /// <summary>
        /// <see cref="Engine.RichPresenceManager"/>
        /// </summary>
        public const string RPC_DETAILS = "RPCDetails";

        /// <summary>
        /// <see cref="Engine.RichPresenceManager"/>
        /// </summary>
        public const string RPC_DISPLAY_LEVEL_FILE_NAME = "RPCDisplayLevelFileName";

        /// <summary>
        /// <see cref="Patches.Behaviours.EnergyUIPatchBehaviour"/>
        /// </summary>
        public const string ENERGY_UI_REWORK = "EnergyUIRework";

        /// <summary>
        /// <see cref="EnergyUIBehaviour"/>
        /// </summary>
        public const string ENERGY_UI_FADE_OUT_IF_FULL = "EnergyUIFadeOutIfFull";

        /// <summary>
        /// <see cref="EnergyUIBehaviour"/>
        /// </summary>
        public const string ENERGY_UI_FADE_OUT_INTENSITY = "EnergyUIFadeOutIntensity";
    }
}
