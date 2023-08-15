using System;

namespace CDOverhaul
{
    public class OverhaulAssetInfo
    {
        public string AssetBundle = "none";
        public string AssetName = "none";

        public bool FixMaterials;

        [NonSerialized]
        public UnityEngine.Object Asset;

        public bool IsNone() => AssetBundle == "none" || AssetName == "none";

        public bool LoadAsset()
        {
            return !IsNone()
&& (Asset || OverhaulAssetsController.TryGetAsset<UnityEngine.Object>(AssetName, AssetBundle, out Asset, FixMaterials))
&& (bool)Asset;
        }
    }
}
