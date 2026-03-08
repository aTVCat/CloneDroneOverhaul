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


        private bool _isProcessingInput;

        private float _timeLeftToProcessInput;

        private bool _hasSucceed;

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
            return _hasSucceed;
        }

        public bool IsProcessingInput()
        {
            return _isProcessingInput;
        }

        public void ProcessInput()
        {
            if (TimeToProcessInput <= 0f)
            {
                ProcessInputNow();
                return;
            }

            _hasSucceed = false;
            _isProcessingInput = true;
            _timeLeftToProcessInput = TimeToProcessInput;

            FileNameProcessResult fileNameProcessResult = new FileNameProcessResult
            {
                IsNotResult = true,
                DisplayColor = Color.gray,
                Message = "Please wait..."
            };
            OnProcessedName.Invoke(fileNameProcessResult);
        }

        public void ProcessInputNow()
        {
            _isProcessingInput = false;
            _timeLeftToProcessInput = 0f;

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
                _hasSucceed = true;

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
            if (!_isProcessingInput)
                return;

            _timeLeftToProcessInput = Mathf.Max(0f, _timeLeftToProcessInput - Time.unscaledDeltaTime);
            if (_timeLeftToProcessInput == 0f)
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
