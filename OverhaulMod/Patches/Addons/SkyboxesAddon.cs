using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;

namespace OverhaulMod.Patches.Addons
{
    internal class SkyboxesAddon : GameAddon
    {
        public override void Start()
        {
            base.Start();
            ModCore.ContentDownloaded += onContentDownloaded;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            ModCore.ContentDownloaded -= onContentDownloaded;
        }

        public override void Patch()
        {
            patch();
        }

        private void patch()
        {
            _ = ModActionUtils.RunCoroutine(patchCoroutine());
        }

        private IEnumerator patchCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            SkyBoxManager.Instance.LevelConfigurableSkyboxes[8].SetColor("_Tint", new Color(0.6f, 0.73f, 2f, 1f));
            if (ContentManager.Instance.HasContent("default"))
            {
                if (ModAdvancedCache.TryGet("Chapter4Skybox_Rework", out Material material1))
                {
                    replaceSkyboxMaterial(material1, 2);
                }
                else
                {
                    ModResources.Instance.LoadAssetAsync("overhaul_default_skyboxes", "Chapter4Skybox", delegate (Material material)
                    {
                        ModAdvancedCache.Add("Chapter4Skybox_Rework", material);
                        replaceSkyboxMaterial(material, 2);
                    }, null, "assets/content/default/");
                }

                if (ModAdvancedCache.TryGet("Chapter5Skybox_Rework", out Material material2))
                {
                    replaceSkyboxMaterial(material2, 7);
                }
                else
                {
                    ModResources.Instance.LoadAssetAsync("overhaul_default_skyboxes", "Chapter5Skybox", delegate (Material material)
                    {
                        ModAdvancedCache.Add("Chapter5Skybox_Rework", material);
                        replaceSkyboxMaterial(material, 7);
                    }, null, "assets/content/default/");
                }
            }
            yield break;
        }

        private void replaceSkyboxMaterial(Material material, int index)
        {
            Material[] array = SkyBoxManager.Instance.LevelConfigurableSkyboxes;
            Material og = array[index];
            if (og != material && RenderSettings.skybox == og)
            {
                RenderSettings.skybox = material;
            }
            array[index] = material;
        }

        private void onContentDownloaded()
        {
            patch();
        }
    }
}
