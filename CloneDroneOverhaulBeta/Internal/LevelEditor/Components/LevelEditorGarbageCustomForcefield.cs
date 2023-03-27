using CDOverhaul.Gameplay;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    public class LevelEditorGarbageCustomForcefield : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<ModdedObject>().GetObject<Transform>(0).GetComponent<Collider>().enabled = GameModeManager.IsInLevelEditor();
            GetComponent<ModdedObject>().GetObject<Transform>(0).GetComponent<Renderer>().material = OverhaulController.GetController<AdvancedGarbageController>().GetForcefieldMaterial();
            GetComponent<ModdedObject>().GetObject<Transform>(0).GetComponent<Renderer>().material.color = new Color(2, 0.7f, 0.7f, 1.2f);
            GetComponent<ModdedObject>().GetObject<Transform>(0).GetComponent<Renderer>().gameObject.AddComponent<ScrollMaterial>().ScrollSpeed = new Vector2(0, 0.05f);
        }
    }
}