using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class AdvancedPhotomodeUITogglePatcher : OverhaulBehaviour
    {
        public Toggle ToggleReference;
        public GameObject ObjectToToggle;

        private bool m_ValueLastFrame;

        private void Update()
        {
            bool newValue = ToggleReference.isOn;
            if (m_ValueLastFrame != newValue)
            {
                ObjectToToggle.SetActive(newValue);
            }
            m_ValueLastFrame = newValue;
        }
    }
}
