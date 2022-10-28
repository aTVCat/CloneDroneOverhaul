using CloneDroneOverhaul.Utilities;
using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class RobotsOverhaulModule : ModuleBase
    {
        // Todo
        // Make robots more unique:
        //  - Random speed
        //  - Random jump height
        //  - Random acceleration speed - To do
        // Robot visuals editor

        public override void Start()
        {
            Functions = new string[]
            {
                "onPlayerSet"
            };
            Singleton<GlobalEventManager>.Instance.AddEventListener<Camera>(GlobalEvents.PlayerCameraEnabled, OnCameraEnabled);
        }

        private void OnCameraEnabled(Camera cam)
        {
            OverhaulMain.Modules.ExecuteFunction<Camera>("onPlayerCameraEnabled", cam);
        }

        public override void RunFunction(string name, object[] arguments)
        {
            // Fix robot speed
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
