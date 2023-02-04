namespace OverhaulAPI
{
    public interface IOverhaulGamemode
    {
        /// <summary>
        /// The name of gamemode
        /// </summary>
        /// <returns></returns>
        string GetGamemodeName();

        /// <summary>
        /// The number of <see cref="GameMode"/>
        /// </summary>
        /// <returns></returns>
        GameMode GetGameMode();

        /// <summary>
        /// The description of gamemode
        /// </summary>
        /// <returns></returns>
        string GetGamemodeDescription();

        /// <summary>
        /// Does the gamemode hide arena?
        /// </summary>
        /// <returns></returns>
        bool HideArena();

        /// <summary>
        /// Called when we start the gamemode
        /// </summary>
        void StartGamemode();

        /// <summary>
        /// Called when game spawned the player
        /// </summary>
        void OnPlayerSpawned();

    }
}
