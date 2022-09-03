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
        List<string> Functions = new List<string>();


        public override bool ShouldWork()
        {
            return true;
        }

        public override void OnActivated()
        {
            Functions.Add("onPlayerSet");
        }

        public override List<string> GetExecutingFunctions()
        {
            return Functions;
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
