using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIComponentGraphicColorUpdater : OverhaulBehaviour
    {
        private Graphic m_Graphic;

        private Color m_TargetColor;

        public Color[] colors
        {
            get;
            set;
        }

        private int m_ColorIndex;
        public int colorIndex
        {
            get => m_ColorIndex;
            set
            {
                m_ColorIndex = value;
                m_TargetColor = colors[value];
            }
        }

        public float multiplier
        {
            get;
            set;
        } = 12.5f;

        public void SetGraphic(Graphic graphic)
        {
            m_Graphic = graphic;
        }

        private void Update()
        {
            if (!m_Graphic)
            {
                return;
            }

            float deltaTime = Time.unscaledDeltaTime * multiplier;
            Color targetColor = m_TargetColor;
            Color currentColor = m_Graphic.color;
            currentColor.r = Mathf.Lerp(currentColor.r, targetColor.r, deltaTime);
            currentColor.g = Mathf.Lerp(currentColor.g, targetColor.g, deltaTime);
            currentColor.b = Mathf.Lerp(currentColor.b, targetColor.b, deltaTime);
            currentColor.a = Mathf.Lerp(currentColor.a, targetColor.a, deltaTime);
            m_Graphic.color = currentColor;
        }
    }
}
