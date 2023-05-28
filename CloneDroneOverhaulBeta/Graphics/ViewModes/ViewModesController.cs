﻿using System;
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

        public static readonly Vector3 DefaultCameraOffset = new Vector3(0, 0.425f, -0.05f);

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
            if (!hasInitializedModel || !ViewModesExpansion.IsFirstPersonMoverSupported(firstPersonMover))
            {
                return;
            }

            ViewModesExpansion viewModesExpansion = firstPersonMover.gameObject.AddComponent<ViewModesExpansion>();
        }
    }
}
