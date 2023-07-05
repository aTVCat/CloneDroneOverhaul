using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.DevTools
{
    public class OverhaulProfilerEntryDisplay : OverhaulBehaviour
    {
        public string MyEntry;

        private Text m_MethodLabel;
        private Text m_TimeLabel;

        private void Start()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_MethodLabel = moddedObject.GetObject<Text>(0);
            m_MethodLabel.text = MyEntry;
            m_TimeLabel = moddedObject.GetObject<Text>(1);
            m_TimeLabel.text = "N/A";
        }

        private void LateUpdate()
        {
            if(m_MethodLabel && m_TimeLabel)
            {
                long ticks = OverhaulProfiler.GetEntryTicks(MyEntry);
                long ms = OverhaulProfiler.GetEntryMs(MyEntry);
                m_TimeLabel.text = ticks + " ticks, " + ms + " ms";

                if(ms > 15)
                {
                    m_TimeLabel.color = Color.red;
                }
                else if (ms > 7)
                {
                    m_TimeLabel.color = "#FF7F08".ToColor();
                }
                else if (ms > 3)
                {
                    m_TimeLabel.color = Color.yellow;
                }
                else
                {
                    m_TimeLabel.color = Color.white;
                }
            }
        }
    }
}
