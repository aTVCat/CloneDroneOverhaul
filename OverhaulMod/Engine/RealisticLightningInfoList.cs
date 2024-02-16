using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
