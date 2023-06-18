using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.HUD.Gamemodes
{
    public class LBSGameCustomization : FullscreenWindowPageBase
    {
        public override Vector2 GetWindowSize() => OverhaulGamemodesUIFullscreenWindow.GameCustomizationWindowSize;
    }
}
