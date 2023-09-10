﻿using CDOverhaul.Visuals;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class RobotCameraZoomExpansion : OverhaulCharacterExpansion
    {
        [OverhaulSettingAttribute_Old("Gameplay.Control.Hold Z to zoom camera", true)]
        public static bool EnableZooming;
        [OverhaulSettingAttribute_Old("Gameplay.Control.Zoom when aiming", false)]
        public static bool ZoomWhenAimingBow;
        [OverhaulSettingSliderParameters(false, 10f, 35f)]
        [OverhaulSettingAttribute_Old("Gameplay.Control.Zoom strength", 15f, false, null, "Gameplay.Control.Hold Z to zoom camera")]
        public static float ZoomBy;
        [OverhaulSettingSliderParameters(false, 2f, 30f)]
        [OverhaulSettingAttribute_Old("Gameplay.Control.Zoom hardness", 10f, false, null, "Gameplay.Control.Hold Z to zoom camera")]
        public static float ZoomHardness;

        public static float FOVOffset = 0f;

        public override void Start()
        {
            base.Start();
            FOVOffset = 0f;
        }

        private void Update()
        {
            if (!EnableZooming || !Owner || !Owner.IsMainPlayer())
                return;

            if (!Owner.IsAlive())
            {
                FOVOffset = 0f;
                base.enabled = false;
                return;
            }

            FOVOffset = Input.GetKey(KeyCode.Z) || ((ViewModesSystem.IsFirstPersonModeEnabled || ZoomWhenAimingBow) && Owner.IsAimingBow())
                ? Mathf.Lerp(FOVOffset, -ZoomBy, Time.unscaledDeltaTime * ZoomHardness)
                : Mathf.Lerp(FOVOffset, 0f, Time.unscaledDeltaTime * ZoomHardness);
        }
    }
}
