using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UISettingInfoEditor : OverhaulUIBehaviour
    {
        [UIElement("Panel", typeof(DraggablePanel))]
        private readonly GameObject _panelObject;

        [UIElementAction(nameof(OnCloseButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        public override bool EnableCursor => true;

        public void OnCloseButtonClicked()
        {
            Hide();
        }
    }
}
