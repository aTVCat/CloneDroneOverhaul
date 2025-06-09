using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class AdditionalSkyboxesManager : Singleton<AdditionalSkyboxesManager>
    {
        private List<AdditionalSkyboxInfo> m_skyboxes;

        public void AddSkybox(string assetBundleName, string skyboxName, Material skybox)
        {
            if (m_skyboxes == null)
                m_skyboxes = new List<AdditionalSkyboxInfo>() { { new AdditionalSkyboxInfo() { AssetBundle = assetBundleName, SkyboxName = skyboxName, SkyboxMaterial = skybox } } };
            else
                m_skyboxes.Add(new AdditionalSkyboxInfo() { AssetBundle = assetBundleName, SkyboxName = skyboxName, SkyboxMaterial = skybox });
        }

        public void SetSkybox(string skybox)
        {
            if (skybox.IsNullOrEmpty() || m_skyboxes.IsNullOrEmpty())
                return;

            foreach (AdditionalSkyboxInfo info in m_skyboxes)
            {
                if (info.GetKey() == skybox)
                {
                    SkyBoxManager.Instance._currentSkybox = info.SkyboxMaterial;
                    RenderSettings.skybox = info.SkyboxMaterial;
                    break;
                }
            }
        }

        public static string GetSkyboxKey(string bundle, string skyboxName)
        {
            return $"{bundle}.{skyboxName}";
        }

        public List<Dropdown.OptionData> GetSkyboxOptions()
        {
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();

            if (!m_skyboxes.IsNullOrEmpty())
                foreach (AdditionalSkyboxInfo info in m_skyboxes)
                {
                    list.Add(new DropdownStringOptionData() { text = info.SkyboxName, StringValue = info.GetKey() });
                }

            return list;
        }

        public List<Dropdown.OptionData> GetSkyboxOptionsForLevelEditor(string currentValue)
        {
            bool foundOption = currentValue.IsNullOrEmpty();

            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>() { new DropdownStringOptionData() { text = "Default", StringValue = string.Empty } };
            if (!m_skyboxes.IsNullOrEmpty())
                foreach (AdditionalSkyboxInfo info in m_skyboxes)
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
