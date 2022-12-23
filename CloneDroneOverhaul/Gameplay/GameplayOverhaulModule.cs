using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.Utilities;
using ModLibrary;
using UnityEngine;

namespace CloneDroneOverhaul.RemovedOrOld
{
    public class GameplayOverhaulModule : ModuleBase
    {
        // Todo
        // Make robots more unique:
        //  - Random speed
        //  - Random jump height
        //  - Random acceleration speed - To do
        // Robot visuals editor

        public static GameplayOverhaulModule Instance;

        public override void Start()
        {
            Functions = new string[]
            {
                "onPlayerSet",
                "firstPersonMover.OnSpawn",
                "firstPersonMover.OnDestroy"
            };
            Singleton<GlobalEventManager>.Instance.AddEventListener<Camera>(GlobalEvents.PlayerCameraEnabled, OnCameraEnabled);
            Instance = this;
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
                        mover.OverrideBaseMoveSpeed(6f);
                    }
                }
            }
        }

        public void CreateLiftAndSpawnPlayer()
        {
            GameFlowManager.Instance.CallPrivateMethod("createPlayerAndSetLift");
        }
    }
}
