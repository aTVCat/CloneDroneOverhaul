using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class OverhaulSettingDropdownParameters : OverhaulSettingBaseAttribute
    {
        public const char UsualSeparator = '@';

        public List<Dropdown.OptionData> Options;

        public float Height;

        public OverhaulSettingDropdownParameters(string values, float height = 150f, char separator = UsualSeparator)
        {
            Height = height;
            Options = new List<Dropdown.OptionData>();
            foreach (string str in values.Split(separator))
            {
                Options.Add(new Dropdown.OptionData(str));
            }
        }
    }
}
