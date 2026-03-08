using OverhaulMod.Engine;
using System.Collections.Generic;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorCopyPasteManager : Singleton<PersonalizationEditorCopyPasteManager>
    {
        private List<ColorPairFloat> _copiedVolumeColorSettings;
        private Dictionary<string, FavoriteColorSettings> _copiedVolumeFavoriteColorSettings;

        public void CopyColorSettings(List<ColorPairFloat> colorPairs, Dictionary<string, FavoriteColorSettings> favoriteColorSettings)
        {
            _copiedVolumeColorSettings = colorPairs;
            _copiedVolumeFavoriteColorSettings = favoriteColorSettings;
        }

        public void PasteColorSettings(out List<ColorPairFloat> colorPairs, out Dictionary<string, FavoriteColorSettings> favoriteColorSettings)
        {
            colorPairs = _copiedVolumeColorSettings;
            favoriteColorSettings = _copiedVolumeFavoriteColorSettings;
        }
    }
}
