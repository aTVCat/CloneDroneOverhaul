using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloneDroneOverhaul.V3Tests.Base;

namespace CloneDroneOverhaul.V3Tests.Gameplay.Animations
{
    /// <summary>
    /// A utility that allows user to make own animations for robots
    /// </summary>
    public class CustomAnimationController : V3_ModControllerBase
    {
        public void StartEditor()
        {
            CustomAnimationEditor editor = new CustomAnimationEditor();
            editor.EnterEditor();
            HUD.UIEditors.GetInstance<HUD.UIEditors>().InitializeRobotAnimationEditorUI();
        }
    }
}
