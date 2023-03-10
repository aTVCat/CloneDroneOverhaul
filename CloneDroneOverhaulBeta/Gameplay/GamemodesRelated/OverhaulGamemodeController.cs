using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class OverhaulGamemodeController : OverhaulGameplayController
    {
        public static string LevelIDToStart;

        public override void Initialize()
        {
            base.Initialize();

            OverhaulEventManager.AddEventListener("GameInitializtionCompleted", onGameInitialized, true);
        }

        private void onGameInitialized()
        {
            startALevelIfRequired();
        }

        private void startALevelIfRequired()
        {
            if (string.IsNullOrEmpty(LevelIDToStart))
            {
                return;
            }


        }
    }
}