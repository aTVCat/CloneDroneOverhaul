using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class ViewModesController : OverhaulGameplayController
    {
        [OverhaulSettingDropdownParameters("Third person@First person")]
        [OverhaulSetting("Gameplay.Camera.View mode", 0, OverhaulVersion.IsUpdate2Hotfix)]
        public static int ViewModeType;
        [OverhaulSetting("Gameplay.Camera.Sync camera with head rotation", false, OverhaulVersion.IsUpdate2Hotfix)]
        public static bool SyncCameraWithHeadRotation;

        public static readonly Vector3 DefaultCameraOffset = new Vector3(0, 0.45f, -0.1f);
        public const float DefaultCameraUpTransformMultiplier = 0.45f;
        public const float AdditionalCameraUpTransformMultiplier = 0.25f;

        public static bool IsLargeBot(FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover)
                return false;

            EnemyType type = firstPersonMover.CharacterType;
            return type == EnemyType.Hammer3 || type == EnemyType.Hammer5;
        }

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
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsViewModesSettingsEnabled || !hasInitializedModel || !ViewModesExpansion.IsFirstPersonMoverSupported(firstPersonMover))
                return;
            _ = firstPersonMover.gameObject.AddComponent<ViewModesExpansion>();
        }
    }
}
