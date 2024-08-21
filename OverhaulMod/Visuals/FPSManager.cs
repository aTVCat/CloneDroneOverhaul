using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Visuals
{
    public class FPSManager : Singleton<FPSManager>
    {
        [ModSetting(ModSettingsConstants.FPS_CAP, -1)]
        public static int FPSCap;

        public static readonly List<Dropdown.OptionData> FPSCapOptions = new List<Dropdown.OptionData>()
        {
            new DropdownIntOptionData() { IntValue = 100, text = "Custom FPS" },
            new DropdownIntOptionData() { IntValue = -1, text = "Unlimited FPS" },
            new DropdownIntOptionData() { IntValue = 30, text = "30 FPS" },
            new DropdownIntOptionData() { IntValue = 60, text = "60 FPS" },
            new DropdownIntOptionData() { IntValue = 90, text = "90 FPS" },
            new DropdownIntOptionData() { IntValue = 120, text = "120 FPS" },
            new DropdownIntOptionData() { IntValue = 144, text = "144 FPS" },
            new DropdownIntOptionData() { IntValue = 165, text = "165 FPS" },
            new DropdownIntOptionData() { IntValue = 240, text = "240 FPS" },
            new DropdownIntOptionData() { IntValue = 300, text = "300 FPS" },
            new DropdownIntOptionData() { IntValue = 360, text = "360 FPS" },
        };

        public int GetFPSCapDropdownValue()
        {
            int settingValue = FPSCap;
            var options = FPSCapOptions;

            for(int i = 0; i < options.Count; i++)
            {
                var option = options[i];
                if(option is DropdownIntOptionData intOption && intOption.IntValue == settingValue)
                {
                    return i;
                }
            }
            return 0;
        }

        public void SetFPSCapDropdownValue(int value)
        {
            if (value < 0 || value >= FPSCapOptions.Count)
                value = 0;

            if (FPSCapOptions[value] is DropdownIntOptionData intOption)
            {
                SetFPSCap(intOption.IntValue);
            }
        }

        public void SetFPSCap(int value)
        {
            ModSettingsManager.SetIntValue(ModSettingsConstants.FPS_CAP, value);
        }

        public static void RefreshFPSCap()
        {
            Application.targetFrameRate = FPSCap;
        }
    }
}
