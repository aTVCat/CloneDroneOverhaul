using OverhaulMod.Content.Personalization;
using OverhaulMod.Content.Personalization.Objects;
using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPEHierarchyPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnCreateButtonClicked))]
        [UIElement("CreateButton")]
        private readonly Button _createButton;

        [UIElement("ObjectDisplayPrefab", false)]
        private readonly ModdedObject _objectDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform _objectDisplayContainer;

        public PersonalizationItemInfo EditingItemInfo
        {
            get => PersonalizationEditorManager.Instance.EditingItemInfo;
        }

        public PersonalizationEditorPlacedObject EditingRoot
        {
            get => PersonalizationEditorManager.Instance.EditingRoot;
        }

        public void Populate()
        {
            if (_objectDisplayContainer.childCount != 0) TransformUtils.DestroyAllChildren(_objectDisplayContainer);

            foreach (PersonalizationEditorPlacedObject child in EditingRoot.Children)
            {
                populateRecursive(child, 0);
            }
        }

        private void populateRecursive(PersonalizationEditorPlacedObject placedObject, int depth)
        {
            ModdedObject moddedObject = Instantiate(_objectDisplayPrefab, _objectDisplayContainer);
            moddedObject.gameObject.SetActive(true);
            moddedObject.GetObject<Text>(1).text = placedObject.Name;
            moddedObject.GetObject<Button>(2).onClick.AddListener(delegate
            {
                ModUIUtils.MessagePopup(true, $"Delete {placedObject.Name}?", LocalizationManager.Instance.GetTranslatedString("action_cannot_be_undone"), 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
                {
                    Destroy(moddedObject.gameObject);
                    PersonalizationEditorObjectManager.Instance.DeleteObject(placedObject);
                });
            });

            Button button = moddedObject.GetComponent<Button>();
            button.onClick.AddListener(delegate
            {
                UIPE.Instance.Inspector.Inspect(placedObject);
            });

            Action refreshAction = delegate
            {
                if (button) button.interactable = UIPE.Instance.Inspector.GetInspectingObject() != placedObject;
            };
            refreshAction();

            EventController eventController = moddedObject.gameObject.AddComponent<EventController>();
            eventController.AddEventListener(PersonalizationEditorObjectManager.OBJECT_SELECTION_CHANGED_EVENT, refreshAction);

            foreach (PersonalizationEditorPlacedObject child in placedObject.Children)
            {
                populateRecursive(child, depth + 1);
            }
        }

        public void OnCreateButtonClicked()
        {
            UIPEObjectBrowser ob = ModUIs.ShowPersonalizationEditorObjectBrowser(UIPE.Instance.transform);
            ob.Callback = Populate;
        }
    }
}