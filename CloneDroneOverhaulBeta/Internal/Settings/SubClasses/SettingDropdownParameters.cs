using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class SettingDropdownParameters : Attribute
    {
        public const char UsualSeparator = '@';

        public List<Dropdown.OptionData> Options;

        public SettingDropdownParameters(string values, char separator = UsualSeparator)
        {
            Options = new List<Dropdown.OptionData>();
            foreach (string str in values.Split(separator))
            {
                Options.Add(new Dropdown.OptionData(str));
            }
        }
    }
}
