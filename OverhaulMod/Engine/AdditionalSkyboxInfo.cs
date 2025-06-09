using UnityEngine;

namespace OverhaulMod.Engine
{
    public class AdditionalSkyboxInfo
    {
        public string AssetBundle;

        public string SkyboxName;

        public Material SkyboxMaterial;

        public string GetKey()
        {
            return AdditionalSkyboxesManager.GetSkyboxKey(AssetBundle, SkyboxName);
        }
    }
}
