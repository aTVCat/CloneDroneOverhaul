using UnityEngine;

namespace OverhaulMod.Patches.Addons
{
    internal class LocalizationAddon : GameAddon
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
