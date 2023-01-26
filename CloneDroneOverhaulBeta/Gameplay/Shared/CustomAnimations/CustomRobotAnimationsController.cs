using CDOverhaul.Gameplay;
using System.Collections.Generic;

namespace CDOverhaul.Shared
{
    public class CustomAnimationsController : ModController
    {
        public bool ShouldWork => !GameModeManager.IsMultiplayer();

        public static readonly GameMode EditorGamemode = (GameMode)(MainGameplayController.GamemodeStartIndex + 2);

        public CustomRobotAnimationsData AnimationsContainer;
        public const string AnimsContainerFilename = "AnimationsContainerAnimationsContainer";

        public override void Initialize()
        {
            OverhaulEventManager.AddListenerToEvent<FirstPersonMover>(MainGameplayController.FirstPersonMoverSpawned_DelayEventString, onRobotSpawn);
            AnimationsContainer = CustomRobotAnimationsData.GetData<CustomRobotAnimationsData>(AnimsContainerFilename, true, string.Empty);

            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        private void onRobotSpawn(FirstPersonMover mover)
        {
            if (!ShouldWork)
            {
                return;
            }
            mover.gameObject.AddComponent<CustomRobotAnimationFPMExtention>();
        }

        public CustomRobotAnimation GetAnimation(in string name)
        {
            return AnimationsContainer.GetAnimation(name);
        }

        public static List<string> GetAllBodyParts()
        {
            List<string> list = new List<string>();
            list.AddRange(CharacterAnimationManager.Instance.UpperBodyPartNames);
            list.AddRange(Singleton<CharacterAnimationManager>.Instance.LegsBodyPartNames);
            return list;
        }
    }
}
