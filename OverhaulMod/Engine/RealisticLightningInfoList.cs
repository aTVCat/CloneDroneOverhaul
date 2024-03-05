using OverhaulMod.Utils;
using System;
using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class RealisticLightningInfoList
    {
        public List<RealisticLightningInfo> LightningInfos;

        [NonSerialized]
        private Dictionary<string, RealisticLightningInfo> m_prefabNameToInfo;

        public void FixValues()
        {
            if (LightningInfos == null)
                LightningInfos = new List<RealisticLightningInfo>();

            if (m_prefabNameToInfo == null)
                m_prefabNameToInfo = new Dictionary<string, RealisticLightningInfo>();
        }

        public RealisticLightningInfo GetLightningInfo(string prefabName)
        {
            if (prefabName.IsNullOrEmpty())
                return null;

            if (m_prefabNameToInfo.TryGetValue(prefabName, out RealisticLightningInfo info))
                return info;

            foreach (RealisticLightningInfo realisticLightningInfo in LightningInfos)
                if (realisticLightningInfo.LevelPrefabName == prefabName)
                {
                    m_prefabNameToInfo.Add(prefabName, realisticLightningInfo);
                    return realisticLightningInfo;
                }

            return null;
        }
    }
}
