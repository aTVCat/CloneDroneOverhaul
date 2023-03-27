using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class SettingsMenuReplacement : ReplacementBase
    {
        private RectTransform m_BG; private Vector2 m_OgSizeDelta;
        private HorizontalLayoutGroup m_TabHolder; private float m_OgSpacing;

        public override void Replace()
        {
            base.Replace();
            SettingsMenu target = GameUIRoot.Instance.SettingsMenu;
            if(target == null)
            {
                SuccessfullyPatched = false;
                return;
            }

            m_BG = target.transform.GetChild(1) as RectTransform;
            m_OgSizeDelta = m_BG.sizeDelta;
            if (m_BG == null)
            {
                SuccessfullyPatched = false;
                return;
            }
            m_BG.sizeDelta = new Vector2(725, 520);

            Transform transform = TransformUtils.FindChildRecursive(m_BG, "TabHolder");
            if (transform == null)
            {
                SuccessfullyPatched = false;
                return;
            }

            m_TabHolder = transform.GetComponent<HorizontalLayoutGroup>();
            if (m_TabHolder == null)
            {
                SuccessfullyPatched = false;
                return;
            }
            m_OgSpacing = m_TabHolder.spacing;
            m_TabHolder.spacing = 17;


            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            base.Cancel();
            if (SuccessfullyPatched)
            {
                m_BG.sizeDelta = m_OgSizeDelta;
                m_TabHolder.spacing = m_OgSpacing;
            }
        }
    }
}
