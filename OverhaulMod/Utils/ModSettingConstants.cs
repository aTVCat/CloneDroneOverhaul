using OverhaulMod.UI;

namespace OverhaulMod.Utils
{
    public static class ModSettingConstants
    {
        public const string UI_PREFIX = "ModUI_";

        /// <summary>
        /// <see cref="Engine.FadingVoxelManager"/>
        /// </summary>
        public const string ENABLE_VOXEL_FIRE_FADING = "EnableVoxelFireFading";

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
    }
}
