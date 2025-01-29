using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class FileCreationDialogController
    {
        public InputField FileNameField;

        public string TargetDirectory;

        public bool CreateFolder;

        public bool RestrictWhiteSpaces;

        public float TimeToProcessInput;

        public string SuccessMessage;


        public FileNameProcessedEvent OnProcessedName = new FileNameProcessedEvent();


        private bool m_isProcessingInput;

        private float m_timeLeftToProcessInput;

        private bool m_hasSucceed;

        public void Initialize(InputField inputField)
        {
            FileNameField = inputField;
            inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
        }

        public void OnInputFieldValueChanged(string value)
        {
            ProcessInput();
        }

        public void ClearInput()
        {
            FileNameField.text = string.Empty;
            ProcessInputNow();
        }

        public bool HasSucceed()
        {
            return m_hasSucceed;
        }

        public bool IsProcessingInput()
        {
            return m_isProcessingInput;
        }

        public void ProcessInput()
        {
            if (TimeToProcessInput <= 0f)
            {
                ProcessInputNow();
                return;
            }

            m_hasSucceed = false;
            m_isProcessingInput = true;
            m_timeLeftToProcessInput = TimeToProcessInput;

            FileNameProcessResult fileNameProcessResult = new FileNameProcessResult();
            fileNameProcessResult.IsNotResult = true;
            fileNameProcessResult.DisplayColor = Color.gray;
            fileNameProcessResult.Message = "Please wait...";
            OnProcessedName.Invoke(fileNameProcessResult);
        }

        public void ProcessInputNow()
        {
            m_isProcessingInput = false;
            m_timeLeftToProcessInput = 0f;

            processInput();
        }

        private void processInput()
        {
            FileNameProcessResult fileNameProcessResult = new FileNameProcessResult();
            if (FileNameField.text.IsNullOrEmpty())
            {
                fileNameProcessResult.Message = $"{getFileSystemEntryName(CreateFolder, true)} name is empty.";
                fileNameProcessResult.DisplayColor = Color.red;
                fileNameProcessResult.Error = true;
            }
            else if (FileNameField.text.IsNullOrWhiteSpace())
            {
                fileNameProcessResult.Message = $"{getFileSystemEntryName(CreateFolder, true)} name is a whitespace.";
                fileNameProcessResult.DisplayColor = Color.red;
                fileNameProcessResult.Error = true;
            }
            else if (RestrictWhiteSpaces && FileNameField.text.Contains(" "))
            {
                fileNameProcessResult.Message = $"{getFileSystemEntryName(CreateFolder, true)} name contains whitespaces.";
                fileNameProcessResult.DisplayColor = Color.red;
                fileNameProcessResult.Error = true;
            }
            else
            {
                foreach (char c in Path.GetInvalidFileNameChars())
                    if (FileNameField.text.Contains(c.ToString()))
                    {
                        fileNameProcessResult.Message = $"{getFileSystemEntryName(CreateFolder, true)} name contains invalid character: {c}";
                        fileNameProcessResult.DisplayColor = Color.red;
                        fileNameProcessResult.Error = true;
                    }

                if (!fileNameProcessResult.Error)
                {
                    string path = Path.Combine(TargetDirectory, FileNameField.text);
                    if (CreateFolder ? Directory.Exists(path) : File.Exists(path))
                    {
                        fileNameProcessResult.Message = $"A {getFileSystemEntryName(CreateFolder, false)} with the same name already exists.";
                        fileNameProcessResult.DisplayColor = Color.red;
                        fileNameProcessResult.Error = true;
                    }
                }
            }

            if (fileNameProcessResult.Error)
            {
                OnProcessedName.Invoke(fileNameProcessResult);
            }
            else
            {
                m_hasSucceed = true;

                fileNameProcessResult.Message = SuccessMessage;
                fileNameProcessResult.DisplayColor = Color.green;
                OnProcessedName.Invoke(fileNameProcessResult);
            }
        }

        private string getFileSystemEntryName(bool folder, bool capitalizeFirstLetter)
        {
            if (folder)
            {
                if (capitalizeFirstLetter)
                {
                    return "Folder";
                }
                else
                {
                    return "folder";
                }
            }
            else
            {
                if (capitalizeFirstLetter)
                {
                    return "File";
                }
                else
                {
                    return "file";
                }
            }
        }

        /// <summary>
        /// Call this every frame
        /// </summary>
        public void UpdateController()
        {
            if (!m_isProcessingInput)
                return;

            m_timeLeftToProcessInput = Mathf.Max(0f, m_timeLeftToProcessInput - Time.unscaledDeltaTime);
            if (m_timeLeftToProcessInput == 0f)
            {
                ProcessInputNow();
            }
        }

        public class FileNameProcessedEvent : UnityEvent<FileNameProcessResult>
        {

        }

        public class FileNameProcessResult
        {
            public string Message;

            public Color DisplayColor;

            public bool Error;

            public bool IsNotResult;
        }
    }
}
