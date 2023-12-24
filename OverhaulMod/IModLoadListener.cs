namespace OverhaulMod
{
    public interface IModLoadListener
    {
        /// <summary>
        /// Called every time the mod tries to load itself, but there's no need to
        /// </summary>
        void OnModLoaded();
    }
}
