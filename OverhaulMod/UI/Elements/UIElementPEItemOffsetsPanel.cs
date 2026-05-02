using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPEItemOffsetsPanel : OverhaulUIBehaviour
    {
        [UIElement("CharacterModelButton", false)]
        private readonly ModdedObject _characterModelButtonPrefab;

        [UIElement("Content")]
        private readonly Transform _characterModelButtonsContainer;

        [UIElement("NothingToEditOverlay", true)]
        private readonly GameObject _nothingToEditOverlay;

        [UIElement("SkinNameLabel")]
        private readonly Text _skinNameLabel;

        [UIElementAction(nameof(OnPositionChanged))]
        [UIElement("PositionPanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field _positionField;

        [UIElementAction(nameof(OnRotationChanged))]
        [UIElement("RotationPanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field _rotationField;

        [UIElementAction(nameof(OnScaleChanged))]
        [UIElement("ScalePanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field _scaleField;

        [UIElementAction(nameof(OnCopyButtonClicked))]
        [UIElement("CopyOffsetButton")]
        private readonly Button _copyButton;

        [UIElementAction(nameof(OnPasteButtonClicked))]
        [UIElement("PasteOffsetButton")]
        private readonly Button _pasteButton;

        public PersonalizationItemInfo EditingItemInfo
        {
            get => PersonalizationEditorManager.Instance.EditingItemInfo;
        }

        public PersonalizationEditorObjectBehaviour EditingItemRoot
        {
            get => PersonalizationEditorManager.Instance.EditingRoot;
        }

        private AccessoryOffset _editingOffset;

        private bool _disableCallbacks;

        private float _lockSelectionTimer;

        protected override void OnInitialized()
        {
            GameObject prevSelectedGraphic = null;

            _pasteButton.interactable = false;

            List<CharacterModelCustomizationEntry> models = MultiplayerCharacterCustomizationManager.Instance.CharacterModels;
            for (int i = 0; i < models.Count; i++)
            {
                int index = i;
                CharacterModelCustomizationEntry modelEntry = models[i];
                ModdedObject moddedObject = Instantiate(_characterModelButtonPrefab, _characterModelButtonsContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Image>(0).sprite = modelEntry.GetSprite(CustomizationCategoryType.FullModel);

                GameObject selectedGraphic = moddedObject.GetObject<GameObject>(1);
                selectedGraphic.SetActive(false);

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    if (_lockSelectionTimer != 0f) return;

                    if (prevSelectedGraphic) prevSelectedGraphic.SetActive(false);
                    selectedGraphic.SetActive(true);
                    prevSelectedGraphic = selectedGraphic;

                    onOffsetButtonClicked(index);
                    _lockSelectionTimer = 0.4f;
                });
            }
        }

        public override void Update()
        {
            _lockSelectionTimer = Mathf.Max(0f, _lockSelectionTimer - Time.deltaTime);
        }

        public void OnPositionChanged(Vector3 value)
        {
            if (_disableCallbacks) return;

            _editingOffset.SetPosition(value);
            EditingItemRoot.transform.localPosition = value;
        }

        public void OnRotationChanged(Vector3 value)
        {
            if (_disableCallbacks) return;

            _editingOffset.SetEulerAngles(value);
            EditingItemRoot.transform.localEulerAngles = value;
        }

        public void OnScaleChanged(Vector3 value)
        {
            if (_disableCallbacks) return;

            _editingOffset.SetScale(value);
            EditingItemRoot.transform.localScale = value;
        }

        public void OnCopyButtonClicked()
        {
            PersonalizationEditorClipboard.Instance.CopyItemOffset(ref _editingOffset);
            _pasteButton.interactable = true;
        }

        public void OnPasteButtonClicked()
        {
            PersonalizationEditorClipboard.Instance.PasteItemOffset(ref _editingOffset);

            _positionField.Vector = _editingOffset.GetPosition();
            _rotationField.Vector = _editingOffset.GetEulerAngles();
            _scaleField.Vector = _editingOffset.GetScale();
        }

        private void onOffsetButtonClicked(int index)
        {
            _editingOffset = EditingItemInfo.AccessoryOffsets.Offsets[index];
            UIPE.Instance.Utilities.SetCharacterModel(index);

            _skinNameLabel.text = $"Edit offset for {MultiplayerCharacterCustomizationManager.Instance.CharacterModels[index].Name}";
            _nothingToEditOverlay.SetActive(false);

            _disableCallbacks = true;
            _positionField.Vector = _editingOffset.GetPosition();
            _rotationField.Vector = _editingOffset.GetEulerAngles();
            _scaleField.Vector = _editingOffset.GetScale();
            _disableCallbacks = false;
        }
    }
}
