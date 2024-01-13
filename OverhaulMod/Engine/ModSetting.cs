using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Engine
{
    public class ModSetting
    {
        public string name
        {
            get;
            private set;
        }

        public Tag tag
        {
            get;
            private set;
        }

        public enum Tag
        {
            None,
        }
    }
}
