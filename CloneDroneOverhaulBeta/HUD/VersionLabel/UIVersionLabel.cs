using CDOverhaul.Gameplay;
using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIVersionLabel : UIBase
    {
        private Text _versionLabel;

        public override void Initialize()
        {
            _ = OverhaulEventManager.AddEventListener(MainGameplayController.GamemodeChangedEventString, onGamemodeUpdated);

            _versionLabel = MyModdedObject.GetObject<Text>(0);

            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        private void onGamemodeUpdated()
        {
            if (_versionLabel == null)
            {
                return;
            }

            try
            {
                _versionLabel.text = GameModeManager.IsOnTitleScreen() ? OverhaulVersion.ModFullName : OverhaulVersion.ModShortName;
            }
            catch
            {
                debug.Log("Version label: Problem encountered with _versionLabel", Color.red);
            }
        }
    }
}