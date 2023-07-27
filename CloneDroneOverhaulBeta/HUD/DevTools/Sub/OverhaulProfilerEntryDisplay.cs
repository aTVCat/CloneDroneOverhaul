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
            if (m_MethodLabel && m_TimeLabel)
            {
                long ticks = OverhaulProfiler.GetEntryTicks(MyEntry);
                long ms = OverhaulProfiler.GetEntryMs(MyEntry);
                m_TimeLabel.text = ticks + " ticks, " + ms + " ms";

                m_TimeLabel.color = ms > 15 ? Color.red : ms > 7 ? "#FF7F08".ToColor() : ms > 3 ? Color.yellow : Color.white;
            }
        }
    }
}
