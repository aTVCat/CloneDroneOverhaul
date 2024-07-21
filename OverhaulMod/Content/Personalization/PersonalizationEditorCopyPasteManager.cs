using OverhaulMod.Engine;
using System.Collections.Generic;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorCopyPasteManager : Singleton<PersonalizationEditorCopyPasteManager>
    {
        private List<ColorPairFloat> m_copiedVolumeColorSettings;
        private Dictionary<string, FavoriteColorSettings> m_copiedVolumeFavoriteColorSettings;

        public void CopyColorSettings(List<ColorPairFloat> colorPairs, Dictionary<string, FavoriteColorSettings> favoriteColorSettings)
        {
            m_copiedVolumeColorSettings = colorPairs;
            m_copiedVolumeFavoriteColorSettings = favoriteColorSettings;
        }

        public void PasteColorSettings(out List<ColorPairFloat> colorPairs, out Dictionary<string, FavoriteColorSettings> favoriteColorSettings)
        {
            colorPairs = m_copiedVolumeColorSettings;
            favoriteColorSettings = m_copiedVolumeFavoriteColorSettings;
        }
    }
}
