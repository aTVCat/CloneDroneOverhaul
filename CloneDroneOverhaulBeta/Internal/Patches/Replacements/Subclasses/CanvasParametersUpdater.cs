using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Patches
{
    public class CanvasParametersUpdater : OverhaulBehaviour
    {
        private Canvas m_Canvas;
        public Canvas TargetCanvas
        {
            get
            {
                if (!m_Canvas)
                    m_Canvas = GetComponent<Canvas>();

                return m_Canvas;
            }
        }

        public bool OverrideSorting;
        public int SortingOrder;

        private void Update()
        {
            Canvas canvas = TargetCanvas;
            if (!canvas)
            {
                base.enabled = false;
                return;
            }

            canvas.overrideSorting = OverrideSorting;
            canvas.sortingOrder = SortingOrder;
        }
    }
}
