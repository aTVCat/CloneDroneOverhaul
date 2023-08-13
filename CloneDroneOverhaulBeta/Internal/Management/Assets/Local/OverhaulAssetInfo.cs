using System;
using TMPro;

namespace CDOverhaul
{
    public class OverhaulAssetInfo
    {
        public string AssetBundle;
        public string AssetName;

        public bool FixMaterials;

        [NonSerialized]
        public UnityEngine.Object Asset;

        public bool LoadAsset()
        {
            if (!Asset && !OverhaulAssetsController.TryGetAsset<UnityEngine.Object>(AssetName, AssetBundle, out Asset, FixMaterials))
                return false;

            return Asset;
        }
    }
}
