using OverhaulMod.Patches.Addons;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;

namespace OverhaulMod.Patches.Addons
{
    internal class SkyboxesAddon : GameAddon
    {
        public override void Patch()
        {
            if (ModContentManager.Instance.HasFolder("default"))
            {
                _ = ModActionUtils.RunCoroutine(patchCoroutine());
            }
        }

        private IEnumerator patchCoroutine()
        {
            ModResources.Instance.LoadAssetAsync("overhaul_default_skyboxes", "Chapter4Skybox", delegate (Material material)
            {
                SkyBoxManager.Instance.LevelConfigurableSkyboxes[2] = material;
            }, null, "assets/content/default/");
            ModResources.Instance.LoadAssetAsync("overhaul_default_skyboxes", "Chapter5Skybox", delegate (Material material)
            {
                SkyBoxManager.Instance.LevelConfigurableSkyboxes[7] = material;
            }, null, "assets/content/default/");
            yield break;
        }
    }
}
