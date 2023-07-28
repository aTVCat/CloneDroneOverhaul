using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class RobotCameraZoomExpansion : OverhaulCharacterExpansion
    {
        public static float FOVOffset = 0f;

        public override void Start()
        {
            base.Start();
            FOVOffset = 0f;
        }

        private void Update()
        {
            if (!Owner || !Owner.IsMainPlayer())
                return;

            if (!Owner.IsAlive())
            {
                FOVOffset = 0f;
                base.enabled = false;
                return;
            }

            if (Input.GetKey(KeyCode.Z))
            {
                FOVOffset = Mathf.Lerp(FOVOffset, -20f, Time.unscaledDeltaTime * 10f);
            }
            else
            {
                FOVOffset = Mathf.Lerp(FOVOffset, 0f, Time.unscaledDeltaTime * 10f);
            }
        }
    }
}
