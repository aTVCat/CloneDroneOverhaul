using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIUpdateInfoEditor : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;
    }
}
