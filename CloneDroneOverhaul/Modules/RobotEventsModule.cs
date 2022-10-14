using CloneDroneOverhaul.PooledPrefabs;
using CloneDroneOverhaul.Utilities;
using ModLibrary;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;

namespace CloneDroneOverhaul.Modules
{
    public class RobotEventsModule : ModuleBase
    {
        public override bool IsEnabled()
        {
            return true;
        }

        public override void OnActivated()
        {
            Functions.Add("onPlayerSet");
            Singleton<GlobalEventManager>.Instance.AddEventListener<Camera>(GlobalEvents.PlayerCameraEnabled, OnCameraEnabled);
        }

        private void OnCameraEnabled(Camera cam)
        {
            //cam.nearClipPlane = 0.01f;
            OverhaulMain.Modules.ExecuteFunction<Camera>("onPlayerCameraEnabled", cam);
        }

        public override void RunFunction(string name, object[] arguments)
        {
            if (name == Functions[0])
            {
                RobotShortInformation info = arguments[0] as RobotShortInformation;
                RobotShortInformation info1 = arguments[1] as RobotShortInformation;

                if (GameModeManager.ConsciousnessTransferToKillerEnabled() && !info.IsNull && info.Instance is FirstPersonMover)
                {
                    FirstPersonMover mover = (FirstPersonMover)info.Instance;
                    if (mover.IsUsingMagBoots())
                    {
                        mover.OverrideBaseMoveSpeed(5.8f);
                    }
                }
            }
        }
    }
}
