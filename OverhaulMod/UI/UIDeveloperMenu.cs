using UnityEngine;

namespace OverhaulMod.UI
{
    public class UIDeveloperMenu : OverhaulBehaviour
    {
        public static bool Enabled;

        public void OnGUI()
        {
            if (!Enabled)
                return;

#if DEBUG
            if (GameModeManager.IsOnTitleScreen())
            {
                if (GUILayout.Button("Update build compile date"))
                {
                    ModBuildInfo.GenerateExtraInfo();
                }
            }

            ModDebug.forceDisableCursor = GUILayout.Toggle(ModDebug.forceDisableCursor, "Force disable cursor [Ctrl+I]");
#endif
        }

#if DEBUG
        public override void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
            {
                Enabled = !Enabled;
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I))
            {
                ModDebug.forceDisableCursor = false;
                GameUIRoot.Instance.RefreshCursorEnabled();
            }
        }
#endif
    }
}
