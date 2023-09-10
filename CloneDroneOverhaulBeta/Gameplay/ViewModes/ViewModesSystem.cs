using CDOverhaul.Gameplay;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class ViewModesSystem : OverhaulGameplaySystem
    {
        public const float DEFAULT_UP_MULTIPLIER = 0.45f;
        public const float ADDITIONAL_UP_MULTIPLIER = 0.25f;

        [OverhaulSettingDropdownParameters("Third person@First person")]
        [OverhaulSettingAttribute_Old("Gameplay.Camera.View mode", 0)]
        public static int ViewModeType;

        [OverhaulSettingRequiredValue(1)]
        [OverhaulSettingAttribute_Old("Gameplay.Camera.Sync camera with head rotation", false, false, null, "Gameplay.Camera.View mode")]
        public static bool SyncCameraWithHeadRotation;

        [OverhaulSettingSliderParameters(false, -10f, 25f)]
        [OverhaulSettingAttribute_Old("Gameplay.Camera.Field of view offset", 0f)]
        public static float FOVOffset;

        public static readonly Vector3 DefaultCameraOffset = new Vector3(0, 0.45f, -0.1f);
        public static readonly Vector3 AimBowCameraOffset = new Vector3(0, 0f, -2.8f);

        public static bool IsLargeBot(FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover)
                return false;

            EnemyType type = firstPersonMover.CharacterType;
            return type == EnemyType.Hammer3 || type == EnemyType.Hammer5;
        }

        public static bool IsFirstPersonModeEnabled => ViewModeType == 1;

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel || !ViewModesExpansion.IsFirstPersonMoverSupported(firstPersonMover))
                return;

            _ = firstPersonMover.gameObject.AddComponent<ViewModesExpansion>();
        }
    }
}
