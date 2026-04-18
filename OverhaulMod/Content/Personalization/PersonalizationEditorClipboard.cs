using OverhaulMod.Engine;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorClipboard : Singleton<PersonalizationEditorClipboard>
    {
        private Vector3 _copiedItemOffsetPosition, _copiedItemOffsetRotation, _copiedItemOffsetScale;

        private bool _hasCopiedItemOffset;

        private List<ColorPairFloat> _copiedVolumeColorSettings;

        private Dictionary<string, FavoriteColorSettings> _copiedVolumeFavoriteColorSettings;

        public void CopyItemOffset(ref AccessoryOffset accessoryOffset)
        {
            _copiedItemOffsetPosition = accessoryOffset.GetPosition();
            _copiedItemOffsetRotation = accessoryOffset.GetEulerAngles();
            _copiedItemOffsetScale = accessoryOffset.GetScale();
            _hasCopiedItemOffset = true;
        }

        public void PasteItemOffset(ref AccessoryOffset accessoryOffset)
        {
            accessoryOffset.SetPosition(_copiedItemOffsetPosition);
            accessoryOffset.SetEulerAngles(_copiedItemOffsetRotation);
            accessoryOffset.SetScale(_copiedItemOffsetScale);
        }

        public bool HasCopiedIemOffset() => _hasCopiedItemOffset;

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