using CDOverhaul.Gameplay;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    public class LevelEditorGarbageDropPoint : MonoBehaviour
    {
        private readonly Transform m_Bot;

        private void Start()
        {
            GetComponent<ModdedObject>().GetObject<Transform>(0).gameObject.SetActive(GameModeManager.IsInLevelEditor());

            if (!GameModeManager.IsInLevelEditor())
            {
                OverhaulController.GetController<AdvancedGarbageController>().AddGarbageDropPoint(transform);
            }
        }

        private void OnDestroy()
        {
            OverhaulController.GetController<AdvancedGarbageController>().RemoveGarbageDropPoint(transform);
        }
    }
}