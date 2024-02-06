using OverhaulMod.UI;

namespace OverhaulMod.Utils
{
    public static class ModSettingConstants
    {
        public const string UI_PREFIX = "ModUI_";

        /// <summary>
        /// <see cref="Engine.FadingVoxelManager"/>
        /// </summary>
        public const string ENABLE_FADING = "EnableFading";

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
    }
}
