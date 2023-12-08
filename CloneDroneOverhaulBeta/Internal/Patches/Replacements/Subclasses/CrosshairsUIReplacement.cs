using CDOverhaul.Graphics;
using UnityEngine;

namespace CDOverhaul.Patches
{
    public class CrosshairsUIReplacement : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            OverhaulEvents.AddEventListener(OverhaulSettingsController.SettingChangedEventString, updateCrossHairs);
            updateCrossHairs();
            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            OverhaulEvents.RemoveEventListener(OverhaulSettingsController.SettingChangedEventString, updateCrossHairs);
            base.Cancel();
        }

        private void updateCrossHairs()
        {
            GameUIRoot.Instance.CrosshairsUI.Child.transform.localPosition = ViewModesController.IsFirstPersonModeEnabled ? Vector3.one * -2f : Vector3.zero;
        }
    }
}
