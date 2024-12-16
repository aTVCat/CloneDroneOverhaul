using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class AdditionalSkyboxesManager : Singleton<AdditionalSkyboxesManager>
    {
        private Dictionary<string, AdditionalSkyboxInfo> m_skyboxes;

        public void AddSkybox(string key, string displayName, Material skybox)
        {
            if (m_skyboxes == null)
                m_skyboxes = new Dictionary<string, AdditionalSkyboxInfo>() { { key, new AdditionalSkyboxInfo() { DisplayName = displayName, Skybox = skybox } } };
            else
                m_skyboxes.Add(key, new AdditionalSkyboxInfo() { DisplayName = displayName, Skybox = skybox });
        }

        public void SetSkybox(string skyboxKey)
        {
            if (skyboxKey.IsNullOrEmpty() || m_skyboxes.IsNullOrEmpty())
                return;

            if (m_skyboxes.TryGetValue(skyboxKey, out AdditionalSkyboxInfo skyboxInfo))
            {
                SkyBoxManager.Instance._currentSkybox = skyboxInfo.Skybox;
                RenderSettings.skybox = skyboxInfo.Skybox;
            }
        }

        public List<Dropdown.OptionData> GetSkyboxOptions()
        {
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            foreach(var kv in m_skyboxes)
            {
                list.Add(new DropdownStringOptionData() { text = kv.Value.DisplayName, StringValue = kv.Key });
            }
            return list;
        }
    }
}
