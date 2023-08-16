using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class GameUIRootBehaviour : MonoBehaviour
    {
        private GraphicRaycaster m_GraphicRayCaster;

        private void Start()
        {
            m_GraphicRayCaster = GameUIRoot.Instance.GetComponent<GraphicRaycaster>();
        }

        private void Update()
        {
            if (Time.frameCount % 3 == 0)
                m_GraphicRayCaster.enabled = Cursor.visible;
        }
    }
}