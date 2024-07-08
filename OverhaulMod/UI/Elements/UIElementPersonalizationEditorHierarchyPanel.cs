using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorHierarchyPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnCreateButtonClicked))]
        [UIElement("CreateButton")]
        private readonly Button m_createButton;

        [UIElement("ObjectDisplayPrefab", false)]
        private readonly ModdedObject m_objectDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform m_objectDisplayContainer;

        private PersonalizationItemInfo m_itemInfo;
        public PersonalizationItemInfo itemInfo
        {
            get
            {
                return m_itemInfo;
            }
            set
            {
                m_itemInfo = value;
                Populate();
            }
        }

        public void Populate()
        {
            if (m_objectDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_objectDisplayContainer);

            foreach (PersonalizationEditorObjectInfo obj in itemInfo.RootObject.Children)
            {
                ModdedObject moddedObject = Instantiate(m_objectDisplayPrefab, m_objectDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(1).text = obj.Name;
                moddedObject.GetObject<Button>(2).onClick.AddListener(delegate
                {
                    ModUIUtils.MessagePopup(true, $"Delete {obj.Name}?", LocalizationManager.Instance.GetTranslatedString("action_cannot_be_undone"), 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
                    {
                        PersonalizationEditorObjectBehaviour behaviour = PersonalizationEditorObjectManager.Instance.GetInstantiatedObject(obj.UniqueIndex);
                        if (behaviour)
                        {
                            _ = base.StartCoroutine(deleteObjectCoroutine(behaviour.gameObject));
                        }
                    });
                });
                moddedObject.GetObject<Button>(3).onClick.AddListener(delegate
                {
                    UIPersonalizationEditor.instance.PropertiesPanel.EditObject(PersonalizationEditorObjectManager.Instance.GetInstantiatedObject(obj.UniqueIndex));
                });
            }
        }

        private IEnumerator deleteObjectCoroutine(GameObject gameObject)
        {
            Destroy(gameObject);
            yield return null;
            yield return null;
            PersonalizationEditorManager.Instance.SerializeRoot();

            PersonalizationEditorManager.Instance.SpawnRootObject();
            Populate();
            yield break;
        }

        public void OnCreateButtonClicked()
        {
            UIPersonalizationEditorObjectBrowser ob = ModUIConstants.ShowPersonalizationEditorObjectBrowser(UIPersonalizationEditor.instance.transform);
            ob.callback = delegate
            {
                Populate();
            };
        }
    }
}
