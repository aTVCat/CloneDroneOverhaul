using UnityEngine;

namespace OverhaulMod.Engine
{
    public class OverhaulAnimator : ModBehaviour
    {
        public OverhaulAnimationListInfo animationList { get; set; }

        private Transform m_objectTransform;
        public Transform objectTransform
        {
            get
            {
                if (!m_objectTransform)
                {
                    m_objectTransform = base.transform;
                }
                return m_objectTransform;
            }
        }

        public void LoadAnimationListInfo(string path)
        {

        }
    }
}
