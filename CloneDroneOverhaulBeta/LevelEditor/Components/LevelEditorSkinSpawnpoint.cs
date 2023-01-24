using CDOverhaul.Gameplay;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    [RequireComponent(typeof(Collider), typeof(Renderer))]
    public class LevelEditorSkinSpawnpoint : MonoBehaviour
    {
        private void Start()
        {
            Collider collider = base.GetComponent<Collider>();
            collider.enabled = GameModeManager.IsInLevelEditor();
            Renderer renderer = base.GetComponent<Renderer>();
            renderer.enabled = GameModeManager.IsInLevelEditor();

            MainGameplayController.Instance.WeaponSkins.CustomSkinSpawnpointInLevel = this;
        }

        private void OnDestroy()
        {
            MainGameplayController.Instance.WeaponSkins.CustomSkinSpawnpointInLevel = null;
        }
    }
}