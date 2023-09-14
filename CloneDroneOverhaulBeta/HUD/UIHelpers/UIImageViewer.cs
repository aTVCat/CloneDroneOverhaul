using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIImageViewer : UIController
    {
        [UIElementComponents(new System.Type[] { typeof(OverhaulUIPanelScaler) })]
        [UIElementReference(0)]
        private readonly RawImage m_RawImage;

        private float m_TimeToReceiveInput;

        public override void Show()
        {
            base.Show();
            m_TimeToReceiveInput = Time.unscaledTime + 0.1f;
        }

        public override void OnGetArguments(object[] args)
        {
            if (args.IsNullOrEmpty())
                return;

            object object1 = args[0];
            if (!(object1 is Texture2D))
                return;

            AttachImage(object1 as Texture2D);
        }

        public override void Update()
        {
            if (Time.unscaledTime >= m_TimeToReceiveInput && Input.anyKeyDown)
                Hide();
        }

        public void AttachImage(Texture2D texture)
        {
            if (!texture)
                return;

            float num = texture.width / (float)texture.height;
            m_RawImage.texture = texture;
            m_RawImage.rectTransform.sizeDelta = new Vector2(m_RawImage.rectTransform.rect.height * num, m_RawImage.rectTransform.sizeDelta.y);
        }
    }
}