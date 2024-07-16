using OverhaulMod.Utils;
using System;
using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class RealisticLightingInfoList
    {
        public List<RealisticLightingInfo> LightingInfos;

        [NonSerialized]
        private Dictionary<string, RealisticLightingInfo> m_prefabNameToInfo;

        public void FixValues()
        {
            if (LightingInfos == null)
                LightingInfos = new List<RealisticLightingInfo>();

            if (m_prefabNameToInfo == null)
                m_prefabNameToInfo = new Dictionary<string, RealisticLightingInfo>();
        }

        public RealisticLightingInfo GetLightingInfo(string prefabName)
        {
            if (prefabName.IsNullOrEmpty())
                return null;

            if (m_prefabNameToInfo.TryGetValue(prefabName, out RealisticLightingInfo info))
                return info;

            foreach (RealisticLightingInfo realisticLightingInfo in LightingInfos)
                if (realisticLightingInfo.LevelPrefabName == prefabName)
                {
                    m_prefabNameToInfo.Add(prefabName, realisticLightingInfo);
                    return realisticLightingInfo;
                }

            return null;
        }
    }
}
