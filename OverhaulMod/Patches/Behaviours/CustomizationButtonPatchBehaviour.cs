using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches.Behaviours
{
    internal class CustomizationButtonPatchBehaviour : GamePatchBehaviour
    {
        private GameObject m_customizeButton;

        public override void Patch()
        {
            Transform buttonRoot = ModCache.titleScreenUI.CustomizationUI.ButtonRoot.transform;
            m_customizeButton = TransformUtils.FindChildRecursive(buttonRoot, "CustomizeButton").gameObject;

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelSpawned, onLevelSpawned);

            onLevelSpawned();
        }

        public override void UnPatch()
        {
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.LevelSpawned, onLevelSpawned);
        }

        private void onLevelSpawned()
        {
            m_customizeButton.gameObject.SetActive(TitleScreenCustomizationManager.Instance.overrideLevelDescription == null);
        }
    }
}
