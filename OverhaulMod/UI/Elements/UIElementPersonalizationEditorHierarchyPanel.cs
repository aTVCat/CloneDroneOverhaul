using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
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

            foreach (var obj in itemInfo.RootObject.Children)
            {
                ModdedObject moddedObject = Instantiate(m_objectDisplayPrefab, m_objectDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(1).text = obj.Name;
            }
        }

        public void OnCreateButtonClicked()
        {
            var ob = ModUIConstants.ShowPersonalizationEditorObjectBrowser(UIPersonalizationEditor.instance.transform);
            ob.callback = delegate
            {
                Populate();
            };
        }
    }
}
