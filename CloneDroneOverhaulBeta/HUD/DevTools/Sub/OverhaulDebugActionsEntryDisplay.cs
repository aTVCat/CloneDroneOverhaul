using System.Diagnostics;
using System.Reflection;
using UnityEngine.UI;

namespace CDOverhaul.DevTools
{
    public class OverhaulDebugActionsEntryDisplay : OverhaulBehaviour
    {
        public string MyEntry;
        public MethodInfo MyMethod;

        private Text m_MethodLabel;

        private void Start()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_MethodLabel = moddedObject.GetObject<Text>(0);
            m_MethodLabel.text = MyEntry;

            Button button = base.GetComponent<Button>();
            button.onClick.AddListener(onClicked);
        }

        private void onClicked()
        {
            if (MyMethod != null)
            {
                Stopwatch stopwatch = OverhaulProfiler.StartTimer();
                _ = MyMethod.Invoke(null, null);
                stopwatch.StopTimer(MyMethod.DeclaringType.ToString().Replace("CDOverhaul.", string.Empty) + "." + MyMethod.Name);
            }
        }
    }
}
