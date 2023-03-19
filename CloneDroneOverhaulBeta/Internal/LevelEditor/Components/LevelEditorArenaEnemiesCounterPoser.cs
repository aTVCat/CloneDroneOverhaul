using CDOverhaul.Patches;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    [RequireComponent(typeof(Collider), typeof(Renderer))]
    public class LevelEditorArenaEnemiesCounterPoser : MonoBehaviour
    {
        private void Start()
        {
            //ReplacementBase.GetReplacement<NewArenaActivator>().GetController().EnemiesLeftPositionOverride = this;
            Collider collider = base.GetComponent<Collider>();
            collider.enabled = GameModeManager.IsInLevelEditor();
            Renderer renderer = base.GetComponent<Renderer>();
            renderer.enabled = GameModeManager.IsInLevelEditor();
        }

        private void OnDestroy()
        {
            //ReplacementBase.GetReplacement<NewArenaActivator>().GetController().EnemiesLeftPositionOverride = null;
        }
    }
}