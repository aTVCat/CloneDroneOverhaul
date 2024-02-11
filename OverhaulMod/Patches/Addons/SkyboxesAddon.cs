using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;

namespace OverhaulMod.Patches.Addons
{
    internal class SkyboxesAddon : GameAddon
    {
        public override void Patch()
        {
            _ = ModActionUtils.RunCoroutine(patchCoroutine());
        }

        private IEnumerator patchCoroutine()
        {
            yield return null;
            yield return null;

            SkyBoxManager.Instance.LevelConfigurableSkyboxes[8].SetColor("_Tint", new Color(0.6f, 0.73f, 2f, 1f));
            if (ContentManager.Instance.HasContent(ContentManager.EXTRAS_CONTENT_FOLDER_NAME))
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
                    }, null, $"assets/content/addons/{ContentManager.EXTRAS_CONTENT_FOLDER_NAME}/");
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
                    }, null, $"assets/content/addons/{ContentManager.EXTRAS_CONTENT_FOLDER_NAME}/");
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
            material.name = og.name;
            array[index] = material;
        }
    }
}
