using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UICrashScreen : OverhaulUIBehaviour
    {
        public override bool enableCursor => true;

        [UIElement("StackTrace")]
        private readonly Text m_stackTraceText;

        [UIElementAction(nameof(OnIgnoreCrashButtonClicked))]
        [UIElement("IgnoreCrashButton")]
        private readonly Button m_ignoreCrashButton;

        public void SetStackTraceText(string message)
        {
            m_stackTraceText.text = message;
        }

        public void OnIgnoreCrashButtonClicked()
        {
            Hide();

            if (ErrorManager.Instance)
                ErrorManager.Instance._hasCrashed = false;

            if(TimeManager.Instance)
                TimeManager.Instance.OnGameUnPaused();
        }
    }
}
