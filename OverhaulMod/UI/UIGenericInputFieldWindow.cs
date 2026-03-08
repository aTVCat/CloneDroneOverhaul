using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIGenericInputFieldWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        [UIElement("Header")]
        private readonly Text _headerText;

        [UIElement("Description")]
        private readonly Text _descriptionText;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button _doneButton;

        [UIElement("InputField")]
        private readonly InputField _inputField;

        [UIElement("Panel")]
        private readonly CanvasGroup _panelCanvasGroup;

        [UIElement("Panel")]
        private readonly RectTransform _panelTransform;

        public override bool refreshOnlyCursor => true;

        public override bool enableCursor => true;

        public Action<string> doneAction
        {
            get;
            set;
        }

        public override void Show()
        {
            base.Show();
            _inputField.ActivateInputField();
        }

        public override void Update()
        {
            _doneButton.interactable = !_inputField.text.IsNullOrEmpty();
        }

        public void OnDoneButtonClicked()
        {
            doneAction?.Invoke(_inputField.text);
            doneAction = null;
            Hide();
        }

        public void SetTexts(string header, string description)
        {
            _headerText.text = header;
            _descriptionText.text = description;
        }

        public void SetHeight(float height)
        {
            Vector2 sizeDelta = _panelTransform.sizeDelta;
            sizeDelta.y = Mathf.Max(100f, height);
            _panelTransform.sizeDelta = sizeDelta;
        }

        public void SetInputFieldText(string text, int limit)
        {
            _inputField.text = text;
            _inputField.characterLimit = Mathf.Max(0, limit);
        }
    }
}
