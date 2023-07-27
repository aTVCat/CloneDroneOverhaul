using UnityEngine;

namespace CDOverhaul.Patches
{
    public class GameModeSelectPanelBehaviourFix : OverhaulBehaviour
    {
        private Transform m_Box;

        public override void OnEnable()
        {
            if (!m_Box)
            {
                m_Box = base.transform.FindChildRecursive("Box");
                if (!m_Box)
                    return;
            }

            m_Box.gameObject.SetActive(true);
        }
    }
}
