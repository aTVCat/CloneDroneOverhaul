using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Engine
{
    public class ModSettingsManager : Singleton<ModSettingsManager>
    {
        private List<ModSetting> m_settings;
        private Dictionary<string, ModSetting> m_nameToSetting;

        public override void Awake()
        {
            base.Awake();

            m_settings = new List<ModSetting>();
            m_nameToSetting = new Dictionary<string, ModSetting>();
        }

        public ModSetting GetSetting(string name)
        {
            return null;
        }


    }
}
