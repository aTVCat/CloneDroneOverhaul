using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAddonsEditorCreationDialog : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button m_doneButton;

        [UIElement("FolderNameField")]
        private readonly InputField m_nameField;

        [UIElement("StatusText")]
        private readonly Text m_statusText;

        public Action<string> Callback;

        private FileCreationDialogController m_controller;

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
            controller.Initialize(m_nameField);
            m_controller = controller;
        }

        public override void Show()
        {
            base.Show();
            m_controller.ClearInput();
        }

        public override void Update()
        {
            m_controller.UpdateController();
        }

        private void onProcessedInput(FileCreationDialogController.FileNameProcessResult result)
        {
            m_statusText.text = result.Message;
            m_statusText.color = result.DisplayColor;
            m_doneButton.interactable = !result.Error && !result.IsNotResult;
        }

        public void OnDoneButtonClicked()
        {
            Hide();
            if (Callback != null)
            {
                Callback(m_nameField.text);
                Callback = null;
            }
        }

        public class AddonCreationCallback : UnityEvent<string>
        {

        }
    }
}
