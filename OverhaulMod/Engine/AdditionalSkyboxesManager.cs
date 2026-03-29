using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class AdditionalSkyboxesManager : Singleton<AdditionalSkyboxesManager>
    {
        private List<AdditionalSkyboxInfo> _skyboxes;

        public void AddSkybox(string assetBundleName, string skyboxName, Material skybox)
        {
            if (_skyboxes == null)
                _skyboxes = new List<AdditionalSkyboxInfo>() { { new AdditionalSkyboxInfo() { AssetBundle = assetBundleName, SkyboxName = skyboxName, SkyboxMaterial = skybox } } };
            else
                _skyboxes.Add(new AdditionalSkyboxInfo() { AssetBundle = assetBundleName, SkyboxName = skyboxName, SkyboxMaterial = skybox });
        }

        public void SetSkybox(string skybox)
        {
            if (_skyboxes.IsNullOrEmpty()) return;
            foreach (AdditionalSkyboxInfo info in _skyboxes)
            {
                if (info.GetKey() != skybox) continue;

                SkyBoxManager.Instance._currentSkybox = info.SkyboxMaterial;
                RenderSettings.skybox = info.SkyboxMaterial;
                break;
            }
        }

        public void SetTint(Color color)
        {
            Material material = RenderSettings.skybox;
            if (material && material.HasProperty("_Tint"))
                material.SetColor("_Tint", color);
        }

        public void SetRotation(float rotation)
        {
            Material material = RenderSettings.skybox;
            if (material && material.HasProperty("_Rotation"))
                material.SetFloat("_Rotation", rotation);
        }

        public static string GetSkyboxKey(string bundle, string skyboxName)
        {
            return $"{bundle}.{skyboxName}";
        }

        public List<Dropdown.OptionData> GetSkyboxOptions()
        {
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();

            if (!_skyboxes.IsNullOrEmpty())
                foreach (AdditionalSkyboxInfo info in _skyboxes)
                {
                    list.Add(new DropdownStringOptionData() { text = info.SkyboxName, StringValue = info.GetKey() });
                }

            return list;
        }

        public List<Dropdown.OptionData> GetSkyboxOptionsForLevelEditor(string currentValue)
        {
            bool foundOption = currentValue.IsNullOrEmpty();

            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>() { new DropdownStringOptionData() { text = "Default", StringValue = string.Empty } };
            if (!_skyboxes.IsNullOrEmpty())
                foreach (AdditionalSkyboxInfo info in _skyboxes)
                {
                    list.Add(new DropdownStringOptionData() { text = info.SkyboxName, StringValue = info.GetKey() });

                    if (!foundOption && info.GetKey() == currentValue)
                        foundOption = true;
                }

            if (!foundOption)
            {
                if (currentValue.IsNullOrEmpty())
                    list.Add(new DropdownStringOptionData() { text = "None", StringValue = string.Empty });
                else
                    list.Add(new DropdownStringOptionData() { text = currentValue, StringValue = currentValue });
            }

            return list;
        }
    }
}
