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
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button m_doneButton;

        [UIElementAction(nameof(OnFileNameChanged))]
        [UIElement("FileNameField")]
        private readonly InputField m_fileNameField;

        [UIElement("OpenFileExplorerToggle")]
        private readonly Toggle m_openFileExplorerToggle;

        [UIElement("StatusText")]
        private readonly Text m_statusText;

        private float m_timeLeftToRefreshStatus;

        private bool m_hasToRefreshStatus;

        public override void Show()
        {
            base.Show();

            m_openFileExplorerToggle.isOn = true;
            m_fileNameField.text = string.Empty;

            m_timeLeftToRefreshStatus = 0f;
            m_hasToRefreshStatus = true;
        }

        public override void Update()
        {
            if (m_hasToRefreshStatus)
            {
                m_timeLeftToRefreshStatus -= Time.unscaledDeltaTime;
                if (m_timeLeftToRefreshStatus <= 0f)
                {
                    m_hasToRefreshStatus = false;
                    m_timeLeftToRefreshStatus = 0f;

                    RefreshStatus();
                }
            }
        }

        public void SetStatusText(string text, Color color)
        {
            m_statusText.text = text;
            m_statusText.color = color;
        }

        public void StartRefreshingStatus()
        {
            SetStatusText("Please wait...", Color.gray);
            m_doneButton.interactable = false;

            m_timeLeftToRefreshStatus = 1f;
            m_hasToRefreshStatus = true;
        }

        public void RefreshStatus()
        {
            if (m_fileNameField.text.IsNullOrEmpty())
            {
                SetStatusText("File name is empty.", Color.red);
                m_doneButton.interactable = false;
                return;
            }

            if (m_fileNameField.text.IsNullOrWhiteSpace())
            {
                SetStatusText("File name is whitespace.", Color.red);
                m_doneButton.interactable = false;
                return;
            }

            if (m_fileNameField.text.EndsWith(" "))
            {
                SetStatusText("Item name ends with whitespace.", Color.red);
                m_doneButton.interactable = false;
                return;
            }

            foreach (char c in Path.GetInvalidFileNameChars())
                if (m_fileNameField.text.Contains(c.ToString()))
                {
                    SetStatusText($"File name contains invalid character: {c}", Color.red);
                    m_doneButton.interactable = false;
                    return;
                }

            string path = Path.Combine(ModCore.savesFolder, $"{m_fileNameField.text}.json");
            if (File.Exists(path))
            {
                SetStatusText("A file with the same name already exists.", Color.red);
                m_doneButton.interactable = false;
                return;
            }

            SetStatusText("You can export settings.", Color.green);
            m_doneButton.interactable = true;
        }

        public void OnDoneButtonClicked()
        {
            string path = Path.Combine(ModCore.savesFolder, $"{m_fileNameField.text}.json");
            ModJsonUtils.WriteStream(path, ModSettingsDataManager.Instance.CreateDataContainerForExport());

            Hide();
            if (m_openFileExplorerToggle.isOn)
                ModFileUtils.OpenFileExplorer(ModCore.savesFolder);
        }

        public void OnFileNameChanged(string value)
        {
            StartRefreshingStatus();
        }
    }
}
