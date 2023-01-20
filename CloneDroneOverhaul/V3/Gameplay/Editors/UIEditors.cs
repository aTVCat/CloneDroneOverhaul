using UnityEngine.UI;

namespace CloneDroneOverhaul.V3.HUD
{
    public class UIEditors : V3_ModHUDBase
    {
        private void Start()
        {
            MyModdedObject.GetObjectFromList<ModdedObject>(0).gameObject.SetActive(false);
        }

        public void InitializeRobotAnimationEditorUI()
        {
            ModdedObject mObject = MyModdedObject.GetObjectFromList<ModdedObject>(0);

            Gameplay.Animations.UIAnimationPanel panel = V3_ModHUDBase.AddHUD<Gameplay.Animations.UIAnimationPanel>(mObject);
            panel.AnimationType = Gameplay.Animations.EAnimationType.RobotEditorAnimation;
            panel.AnimationsDropdown = mObject.GetObjectFromList<Dropdown>(0);

            panel.gameObject.SetActive(true);
        }
    }
}
