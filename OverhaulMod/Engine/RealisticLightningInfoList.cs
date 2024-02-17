using OverhaulMod.Utils;
using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class RealisticLightningInfoList
    {
        public List<RealisticLightningInfo> LightningInfos;

        public void FixValues()
        {
            if (LightningInfos == null)
                LightningInfos = new List<RealisticLightningInfo>();
        }

        public RealisticLightningInfo GetLightningInfo(string prefabName)
        {
            if (prefabName.IsNullOrEmpty())
                return null;

            foreach (RealisticLightningInfo realisticLightningInfo in LightningInfos)
                if (realisticLightningInfo.LevelPrefabName == prefabName)
                    return realisticLightningInfo;

            return null;
        }
    }
}
