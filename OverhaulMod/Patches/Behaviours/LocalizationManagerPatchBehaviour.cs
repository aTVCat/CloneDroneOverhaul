namespace OverhaulMod.Patches.Behaviours
{
    internal class LocalizationManagerPatchBehaviour : GamePatchBehaviour
    {
        public override void Patch()
        {
            LocalizationManager localizationManager = LocalizationManager.Instance;
            if (localizationManager)
            {
                localizationManager.SupportedLanguages[11].FlagImage = localizationManager.SupportedLanguages[6].FlagImage;
            }
        }
    }
}
