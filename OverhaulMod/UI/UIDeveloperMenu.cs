using UnityEngine;

namespace OverhaulMod.UI
{
    public class UIDeveloperMenu : OverhaulBehaviour
    {
        public void OnGUI()
        {
#if DEBUG
            if (GameModeManager.IsOnTitleScreen())
            {
                if (GUILayout.Button("Update build compile date"))
                {
                    ModBuildInfo.GenerateExtraInfo();
                }
            }
#endif
        }
    }
}
