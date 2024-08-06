using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Engine
{
    public class AutoBuildListInfo
    {
        public List<AutoBuildInfo> Builds;

        public void FixValues()
        {
            if(Builds==null)
                Builds = new List<AutoBuildInfo>();
        }
    }
}
