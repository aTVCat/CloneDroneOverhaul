using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UISettingsImportExportMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button _doneButton;

        [UIElementAction(nameof(OnFileNameChanged))]
        [UIElement("FileNameField")]
        private readonly InputField _fileNameField;

        [UIElement("OpenFileExplorerToggle")]
        private readonly Toggle _openFileExplorerToggle;

        [UIElement("StatusText")]
        private readonly Text _statusText;

        private float _timeLeftToRefreshStatus;

        private bool _hasToRefreshStatus;

        public override void Show()
        {
            base.Show();

            _openFileExplorerToggle.isOn = true;
            _fileNameField.text = string.Empty;

            _timeLeftToRefreshStatus = 0f;
            _hasToRefreshStatus = true;
        }

        public override void Update()
        {
            if (_hasToRefreshStatus)
            {
                _timeLeftToRefreshStatus -= Time.unscaledDeltaTime;
                if (_timeLeftToRefreshStatus <= 0f)
                {
                    _hasToRefreshStatus = false;
                    _timeLeftToRefreshStatus = 0f;

                    RefreshStatus();
                }
            }
        }

        public void SetStatusText(string text, Color color)
        {
            _statusText.text = text;
            _statusText.color = color;
        }

        public void StartRefreshingStatus()
        {
            SetStatusText("Please wait...", Color.gray);
            _doneButton.interactable = false;

            _timeLeftToRefreshStatus = 1f;
            _hasToRefreshStatus = true;
        }

        public void RefreshStatus()
        {
            if (_fileNameField.text.IsNullOrEmpty())
            {
                SetStatusText("File name is empty.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            if (_fileNameField.text.IsNullOrWhiteSpace())
            {
                SetStatusText("File name is whitespace.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            if (_fileNameField.text.EndsWith(" "))
            {
                SetStatusText("Item name ends with whitespace.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            foreach (char c in Path.GetInvalidFileNameChars())
                if (_fileNameField.text.Contains(c.ToString()))
                {
                    SetStatusText($"File name contains invalid character: {c}", Color.red);
                    _doneButton.interactable = false;
                    return;
                }

            string path = Path.Combine(ModCore.SavesFolder, $"{_fileNameField.text}.json");
            if (File.Exists(path))
            {
                SetStatusText("A file with the same name already exists.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            SetStatusText("You can export settings.", Color.green);
            _doneButton.interactable = true;
        }

        public void OnDoneButtonClicked()
        {
            string path = Path.Combine(ModCore.SavesFolder, $"{_fileNameField.text}.json");
            ModJsonUtils.WriteStream(path, ModSettingsDataManager.Instance.CreateDataContainerForExport());

            Hide();
            if (_openFileExplorerToggle.isOn)
                ModFileUtils.OpenFileExplorer(ModCore.SavesFolder);
        }

        public void OnFileNameChanged(string value)
        {
            StartRefreshingStatus();
        }
    }
}
