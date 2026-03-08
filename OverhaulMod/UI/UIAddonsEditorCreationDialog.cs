using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAddonsEditorCreationDialog : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button _doneButton;

        [UIElement("FolderNameField")]
        private readonly InputField _nameField;

        [UIElement("StatusText")]
        private readonly Text _statusText;

        public Action<string> Callback;

        private FileCreationDialogController _controller;

        protected override void OnInitialized()
        {
            FileCreationDialogController controller = new FileCreationDialogController
            {
                TargetDirectory = ModCore.addonsFolder,
                CreateFolder = true,
                RestrictWhiteSpaces = true,
                TimeToProcessInput = 0.5f,
                SuccessMessage = "You can create the addon."
            };
            controller.OnProcessedName.AddListener(onProcessedInput);
            controller.Initialize(_nameField);
            _controller = controller;
        }

        public override void Show()
        {
            base.Show();
            _controller.ClearInput();
        }

        public override void Update()
        {
            _controller.UpdateController();
        }

        private void onProcessedInput(FileCreationDialogController.FileNameProcessResult result)
        {
            _statusText.text = result.Message;
            _statusText.color = result.DisplayColor;
            _doneButton.interactable = !result.Error && !result.IsNotResult;
        }

        public void OnDoneButtonClicked()
        {
            Hide();
            if (Callback != null)
            {
                Callback(_nameField.text);
                Callback = null;
            }
        }

        public class AddonCreationCallback : UnityEvent<string>
        {

        }
    }
}
