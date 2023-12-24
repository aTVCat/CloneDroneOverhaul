using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod
{
    public class ModContentManager : Singleton<ModContentManager>
    {
        public bool IsFullInstallation()
        {
            return false;
        }
    }
}
