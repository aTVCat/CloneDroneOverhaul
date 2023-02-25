using CDOverhaul.Gameplay;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    [RequireComponent(typeof(Collider), typeof(Renderer))]
    public class LevelEditorGamemodeSubstateChanger : MonoBehaviour
    {
        [IncludeInLevelEditor(false, true)]
        public int Index = 0;

        private void Start()
        {
            Collider collider = base.GetComponent<Collider>();
            collider.enabled = GameModeManager.IsInLevelEditor();
            Renderer renderer = base.GetComponent<Renderer>();
            renderer.enabled = GameModeManager.IsInLevelEditor();
        }

        [CallFromAnimation]
        public void SetGamemodeSubstate()
        {
            OverhaulGameplayCoreController.Instance.GamemodeSubstates.GamemodeSubstate = (GamemodeSubstate)Index;
            FirstPersonMover mover = CharacterTracker.Instance.GetPlayerRobot();
            if (mover != null)
            {
                mover.transform.localRotation = base.transform.localRotation;
            }
        }
    }
}