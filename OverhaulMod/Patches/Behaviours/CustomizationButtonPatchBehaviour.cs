using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches.Behaviours
{
    internal class CustomizationButtonPatchBehaviour : GamePatchBehaviour
    {
        private GameObject _customizeButton;

        public override void Patch()
        {
            Transform buttonRoot = ModCache.titleScreenUI.CustomizationUI.ButtonRoot.transform;
            _customizeButton = TransformUtils.FindChildRecursive(buttonRoot, "CustomizeButton").gameObject;

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelSpawned, onLevelSpawned);

            onLevelSpawned();
        }

        public override void UnPatch()
        {
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.LevelSpawned, onLevelSpawned);
        }

        private void onLevelSpawned()
        {
            if (!GameModeManager.IsOnTitleScreen()) return;

            _customizeButton.gameObject.SetActive(FindObjectOfType<CustomizationPlayerPreview>());
        }
    }
}
