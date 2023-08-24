using CDOverhaul.Visuals;
using UnityEngine;

namespace CDOverhaul.Patches
{
    public class CrosshairsUIReplacement : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            OverhaulEventsController.AddEventListener(OverhaulSettingsController.SettingChangedEventString, updateCrossHairs);
            updateCrossHairs();
            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            OverhaulEventsController.RemoveEventListener(OverhaulSettingsController.SettingChangedEventString, updateCrossHairs);
            base.Cancel();
        }

        private void updateCrossHairs()
        {
            GameUIRoot.Instance.CrosshairsUI.Child.transform.localPosition = ViewModesManager.IsFirstPersonModeEnabled ? Vector3.one * -2f : Vector3.zero;
        }
    }
}
