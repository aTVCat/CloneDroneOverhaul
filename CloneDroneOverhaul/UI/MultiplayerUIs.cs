using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class MultiplayerUIs : ModGUIBase
    {
        public class LBSUI : MonoBehaviour
        {

        }

        public override void OnInstanceStart()
        {
            base.MyModdedObject = base.GetComponent<ModdedObject>();
            foreach(GameObject obj in MyModdedObject.objects)
            {
                obj.SetActive(false);
            }
        }
    }
}
