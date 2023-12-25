namespace OverhaulMod
{
    public interface IModContentLoadListener
    {
        /// <summary>
        /// Called when mod finishes loading the content
        /// </summary>
        void OnModContentLoaded();
    }
}
