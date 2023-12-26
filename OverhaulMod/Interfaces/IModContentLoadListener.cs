namespace OverhaulMod
{
    public interface IModContentLoadListener
    {
        /// <summary>
        /// Called when mod finished downloading the content
        /// </summary>
        void OnModContentLoaded();
    }
}
