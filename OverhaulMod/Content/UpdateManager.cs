namespace OverhaulMod.Content
{
    public class UpdateManager : Singleton<UpdateManager>
    {
        public UpdateInfo CreateUpdateInfoOfCurrentBuild(string downloadLink, string changelog)
        {
            return new UpdateInfo()
            {
                ModBotVersion = (int)ModCore.instance.ModInfo.Version,
                ModVersion = ModBuildInfo.version,
                DownloadLink = downloadLink,
                Changelog = changelog
            };
        }
    }
}
