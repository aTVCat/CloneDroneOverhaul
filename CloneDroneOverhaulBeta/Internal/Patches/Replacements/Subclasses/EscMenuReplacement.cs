using UnityEngine;

namespace CDOverhaul.Patches
{
    public class EscMenuReplacement : ReplacementBase
    {
        private RectTransform m_BG;
        private Vector2 m_OgSizeDelta;
        private Vector2 m_OgPosition;

        public override void Replace()
        {
            base.Replace();
            EscMenu target = GameUIRoot.Instance.EscMenu;
            if(target == null)
            {
                SuccessfullyPatched = false;
                return;
            }

            m_BG = target.transform.GetChild(5) as RectTransform;
            m_OgSizeDelta = m_BG.sizeDelta;
            m_OgPosition = m_BG.localPosition;
            if (m_BG == null)
            {
                SuccessfullyPatched = false;
                return;
            }
            m_BG.localPosition = new Vector3(0, -20, 0);
            m_BG.sizeDelta = new Vector2(180, 235);

            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            base.Cancel();
            if (SuccessfullyPatched)
            {
                m_BG.localPosition = m_OgPosition;
                m_BG.sizeDelta = m_OgSizeDelta;
            }
        }
    }
}
