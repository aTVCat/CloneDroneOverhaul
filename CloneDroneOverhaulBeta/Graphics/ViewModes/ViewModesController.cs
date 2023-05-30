using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class ViewModesController : OverhaulGameplayController
    {
        [SettingDropdownParameters("Third person@First person")]
        [OverhaulSetting("Gameplay.Camera.View mode", 0)]
        public static int ViewModeType;
        [OverhaulSetting("Gameplay.Camera.Sync camera with head rotation", false)]
        public static bool SyncCameraWithHeadRotation;

        public static readonly Vector3 DefaultCameraOffset = new Vector3(0, 0.425f, -0.1f);

        public static bool IsFirstPersonModeEnabled => ViewModeType == 1;

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!OverhaulFeatureAvailabilitySystem.BuildImplements.IsViewModesSettingsEnabled || !hasInitializedModel || !ViewModesExpansion.IsFirstPersonMoverSupported(firstPersonMover))
            {
                return;
            }

            ViewModesExpansion viewModesExpansion = firstPersonMover.gameObject.AddComponent<ViewModesExpansion>();
        }
    }
}
