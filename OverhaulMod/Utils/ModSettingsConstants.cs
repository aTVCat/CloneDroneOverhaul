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
    }
}
