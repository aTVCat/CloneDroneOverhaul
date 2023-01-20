using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3.Base
{
    public struct SModSettingDropdownSettings
    {
        public List<Dropdown.OptionData> Options;

        public Type Type;

        public bool Translate;

        /// <summary>
        /// Must-use method to set enumerator values up
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="useInt"></param>
        public SModSettingDropdownSettings(in Type enumerator, in bool translate)
        {
            this.Type = enumerator;
            Options = new List<Dropdown.OptionData>();
            Translate = translate;
            string[] strings = enumerator.GetEnumNames();
            foreach(string @string in strings)
            {
                Options.Add(new Dropdown.OptionData(@string));
            }
        }
    }
}
