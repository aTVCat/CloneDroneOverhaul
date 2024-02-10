using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class CrosshairOffsetController : MonoBehaviour
    {
        private Vector3 m_offset;

        private void Start()
        {
            m_offset = new Vector3(0f, 22.5f, 0f);
        }

        private void Update()
        {
            base.transform.localPosition = CameraModeManager.EnableFirstPersonMode ? m_offset : Vector3.zero;
        }
    }
}
