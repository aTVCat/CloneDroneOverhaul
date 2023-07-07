using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AdvancedPhotomodeSettingAttribute : Attribute
    {
        public string DisplayName;
        public string CategoryName;

        public AdvancedPhotomodeSettingAttribute(string displayName, string category)
        {
            DisplayName = displayName;
            CategoryName = category;
        }
    }
}
