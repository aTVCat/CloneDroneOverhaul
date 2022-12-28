using System;
using System.Collections.Generic;
using System.Linq;
using CloneDroneOverhaul.V3Tests.Base;
using CloneDroneOverhaul.V3Tests.HUD;
using UnityEngine.UI;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.HUD
{
    public class UIEditors : V3_ModHUDBase
    {
        private void Start()
        {
            ModdedObject.GetObjectFromList<ModdedObject>(0).gameObject.SetActive(false);
        }

        public void InitializeRobotAnimationEditorUI()
        {
            ModdedObject mObject = ModdedObject.GetObjectFromList<ModdedObject>(0);

            Gameplay.Animations.UIAnimationPanel panel = V3_ModHUDBase.AddHUD<Gameplay.Animations.UIAnimationPanel>(mObject);
            panel.AnimationType = Gameplay.Animations.EAnimationType.RobotEditorAnimation;
            panel.AnimationsDropdown = mObject.GetObjectFromList<Dropdown>(0);

            panel.gameObject.SetActive(true);
        }
    }
}
