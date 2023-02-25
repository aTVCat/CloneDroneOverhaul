﻿using CDOverhaul.Gameplay;
using System.Collections.Generic;

namespace CDOverhaul.Shared
{
    public class CustomAnimationsController : OverhaulController
    {
        public bool ShouldWork => !GameModeManager.IsMultiplayer();

        public static readonly GameMode EditorGamemode = (GameMode)(OverhaulGameplayCoreController.GamemodeStartIndex + 2);

        public CustomRobotAnimationsData AnimationsContainer;
        public const string AnimsContainerFilename = "AnimationsContainerAnimationsContainer";

        public override void Initialize()
        {
            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.FirstPersonMoverSpawned_DelayEventString, configFPM);
            _ = OverhaulEventManager.AddEventListener(GamemodeSubstatesController.SubstateChangedEventString, enterAnimationMode);
            AnimationsContainer = CustomRobotAnimationsData.GetData<CustomRobotAnimationsData>(AnimsContainerFilename, true, string.Empty);
        }

        private void configFPM(FirstPersonMover mover)
        {
            if (!ShouldWork)
            {
                return;
            }
            _ = mover.gameObject.AddComponent<CustomRobotAnimationFPMExtention>();
        }

        private void enterAnimationMode()
        {
            GamemodeSubstate s = OverhaulGameplayCoreController.Instance.GamemodeSubstates.GamemodeSubstate;
            if (s != GamemodeSubstate.AnimatingCustomAnimation)
            {
                return;
            }
        }

        /// <summary>
        /// Get all body parts that may be animated in level editor
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllBodyParts()
        {
            List<string> list = new List<string>();
            list.AddRange(CharacterAnimationManager.Instance.UpperBodyPartNames);
            list.AddRange(Singleton<CharacterAnimationManager>.Instance.LegsBodyPartNames);
            return list;
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}
