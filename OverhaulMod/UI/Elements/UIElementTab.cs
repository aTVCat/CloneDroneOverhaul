using OverhaulMod;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementTab : OverhaulUIBehaviour
    {
        public string tabId
        {
            get;
            set;
        }

        public virtual void OnTabSelected() { }
        public virtual void OnTabDeselected() { }
        public virtual Button GetButton() { return base.GetComponent<Button>(); }
    }
}
