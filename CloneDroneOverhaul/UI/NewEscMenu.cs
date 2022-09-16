using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class NewEscMenu : ModGUIBase
    {
        public override void OnAdded()
        {
            base.gameObject.SetActive(false);
        }
    }
}
