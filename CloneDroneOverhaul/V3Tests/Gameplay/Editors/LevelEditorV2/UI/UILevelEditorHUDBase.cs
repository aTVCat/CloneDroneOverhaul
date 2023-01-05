using UnityEngine;

namespace CloneDroneOverhaul.LevelEditor
{
    public class UILevelEditorHUDBase : MonoBehaviour
    {
        protected ModdedObject MyModdedObject { get; private set; }
        public bool HasModdedObject => MyModdedObject != null;

        private void Awake()
        {
            MyModdedObject = base.GetComponent<ModdedObject>();
        }
    }
}
