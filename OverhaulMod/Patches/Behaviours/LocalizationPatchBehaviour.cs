namespace OverhaulMod.Patches.Behaviours
{
    internal class LocalizationPatchBehaviour : GamePatchBehaviour
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
