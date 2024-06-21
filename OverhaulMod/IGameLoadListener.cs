namespace OverhaulMod
{
    public interface IGameLoadListener
    {
        /// <summary>
        /// Called when game finishes initialization
        /// </summary>
        void OnGameLoaded();
    }
}
