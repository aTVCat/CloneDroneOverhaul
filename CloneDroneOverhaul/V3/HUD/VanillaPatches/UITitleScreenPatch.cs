using UnityEngine.UI;

namespace CloneDroneOverhaul.V3.HUD
{
    public class UIEmoteSelectionPatch : V3_ModHUDBase
    {
        private EmoteSelectionUI _emoteSelectionUI;

        private bool _hasInitialized;

        private void Start()
        {
            _emoteSelectionUI = GameUIRoot.Instance.EmoteSelectionUI;
            if (_emoteSelectionUI == null)
            {
                return;
            }

            _emoteSelectionUI.GetComponent<Image>().enabled = false;

            _hasInitialized = true;
        }
    }
}
