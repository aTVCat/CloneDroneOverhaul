using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class UIDeveloperMenu : OverhaulBehaviour
    {
        public void OnGUI()
        {
            if (GameModeManager.IsOnTitleScreen())
            {
                if(GUILayout.Button("Update build compile date"))
                {
                    ModBuildInfo.GenerateExtraInfo();
                }
            }
        }
    }
}
