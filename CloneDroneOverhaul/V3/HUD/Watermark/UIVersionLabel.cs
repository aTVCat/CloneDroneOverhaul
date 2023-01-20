using TMPro;
using UnityEngine;

namespace CloneDroneOverhaul.V3.HUD
{
    public class UIVersionLabel : V3_ModHUDBase
    {
        private static readonly string ModName = OverhaulDescription.GetModName(true, false);
        private static readonly string ModName_Short = OverhaulDescription.GetModName(true, true);

        private bool _showVersion;

        private float _timeToNextRefresh;
        private GameMode _currentGameMode;

        private TextMeshProUGUI _versionLabel;

        private void Start()
        {
            _versionLabel = MyModdedObject.GetObjectFromList<TextMeshProUGUI>(0);
            _showVersion = true;
        }

        private void Update()
        {
            _versionLabel.gameObject.SetActive(_showVersion && !PhotoManager.Instance.IsInPhotoMode() && !CutSceneManager.Instance.IsInCutscene() && !GameModeManager.IsInLevelEditor() && !Graphics.CameraRollController.IsUIHidden);

            _timeToNextRefresh -= Time.deltaTime;

            if (_timeToNextRefresh <= 0f)
            {
                _timeToNextRefresh = 0.5f;
                if (_currentGameMode == GameMode.None)
                {
                    _versionLabel.text = GameUIRoot.Instance.TitleScreenUI.VersionLabel.text + "\n" + ModName;
                }
                else
                {
                    _versionLabel.text = ModName_Short;
                }
            }
        }

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (eventName == "onGameModeUpdated")
            {
                GameMode gm = (GameMode)(int)args[0];
                _currentGameMode = gm;
            }
        }

        public override void OnSettingRefreshed(in string settingName, in object value)
        {
            if (settingName == "Misc.Mod.Show version")
            {
                _showVersion = (bool)value;
            }
        }
    }
}