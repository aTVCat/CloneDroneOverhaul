using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorHierarchyPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnCreateButtonClicked))]
        [UIElement("CreateButton")]
        private readonly Button _createButton;

        [UIElement("ObjectDisplayPrefab", false)]
        private readonly ModdedObject _objectDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform _objectDisplayContainer;

        private PersonalizationItemInfo _itemInfo;
        public PersonalizationItemInfo ItemInfo
        {
            get
            {
                return _itemInfo;
            }
            set
            {
                _itemInfo = value;
                Populate();
            }
        }

        public void Populate()
        {
            if (_objectDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_objectDisplayContainer);

            foreach (PersonalizationEditorObjectInfo obj in ItemInfo.RootObject.Children)
            {
                ModdedObject moddedObject = Instantiate(_objectDisplayPrefab, _objectDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(1).text = obj.Name;
                moddedObject.GetObject<Button>(2).onClick.AddListener(delegate
                {
                    ModUIUtils.MessagePopup(true, $"Delete {obj.Name}?", LocalizationManager.Instance.GetTranslatedString("action_cannot_be_undone"), 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
                    {
                        PersonalizationEditorObjectBehaviour behaviour = PersonalizationEditorObjectManager.Instance.GetInstantiatedObject(obj.UniqueIndex);
                        if (behaviour)
                        {
                            Destroy(moddedObject.gameObject);
                            PersonalizationEditorObjectManager.Instance.DeleteObject(behaviour);
                        }
                    });
                });

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    UIPersonalizationEditor.Instance.Inspector.EditObject(PersonalizationEditorObjectManager.Instance.GetInstantiatedObject(obj.UniqueIndex));
                });

                Action refreshAction = delegate
                {
                    if (button) button.interactable = UIPersonalizationEditor.Instance.Inspector.GetEditingObjectUniqueIndex() != obj.UniqueIndex;
                };
                refreshAction();

                EventController eventController = moddedObject.gameObject.AddComponent<EventController>();
                eventController.AddEventListener(PersonalizationEditorObjectManager.OBJECT_SELECTION_CHANGED_EVENT, refreshAction);
            }
        }

        public void OnCreateButtonClicked()
        {
            UIPersonalizationEditorObjectBrowser ob = ModUIs.ShowPersonalizationEditorObjectBrowser(UIPersonalizationEditor.Instance.transform);
            ob.callback = Populate;
        }
    }
}
