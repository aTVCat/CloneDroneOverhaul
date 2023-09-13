using CDOverhaul.Visuals;
using UnityEngine;

namespace CDOverhaul.Patches
{
    public class CrosshairsUIReplacement : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            OverhaulEvents.AddEventListener(OverhaulSettingsManager_Old.SETTING_VALUE_UPDATED_EVENT, updateCrossHairs);
            updateCrossHairs();
            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            OverhaulEvents.RemoveEventListener(OverhaulSettingsManager_Old.SETTING_VALUE_UPDATED_EVENT, updateCrossHairs);
            base.Cancel();
        }

        private void updateCrossHairs()
        {
            GameUIRoot.Instance.CrosshairsUI.Child.transform.localPosition = ViewModesSystem.IsFirstPersonModeEnabled ? Vector3.one * -2f : Vector3.zero;
        }
    }
}
