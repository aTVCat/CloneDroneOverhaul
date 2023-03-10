using CDOverhaul.Gameplay;
using CDOverhaul.Patches;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    public class LevelEditorGarbageDropPoint : MonoBehaviour
    {
        private Transform m_Bot;

        private void Start()
        {
            GetComponent<ModdedObject>().GetObject<Transform>(0).gameObject.SetActive(GameModeManager.IsInLevelEditor());

            if (!GameModeManager.IsInLevelEditor())
            {
                OverhaulController.GetController<AdvancedGarbageController>().AddGarbageDropPoint(this.transform);
            }
        }

        private void OnDestroy()
        {
            OverhaulController.GetController<AdvancedGarbageController>().RemoveGarbageDropPoint(this.transform);
        }
    }
}