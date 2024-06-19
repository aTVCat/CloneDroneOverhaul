using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorObjectBrowser : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("ObjectDisplayPrefab", false)]
        private readonly ModdedObject m_objectDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform m_container;

        public Action callback
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            System.Collections.Generic.List<PersonalizationEditorObjectSpawnInfo> list = PersonalizationEditorObjectManager.Instance.GetObjectInfos();
            foreach (PersonalizationEditorObjectSpawnInfo obj in list)
            {
                ModdedObject moddedObject = Instantiate(m_objectDisplayPrefab, m_container);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = obj.Name;
                moddedObject.GetObject<Text>(1).text = LocalizationManager.Instance.GetTranslatedString($"ce_object_{obj.Name}");

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    PersonalizationEditorObjectBehaviour b = PersonalizationEditorObjectManager.Instance.PlaceObject(obj.Path, PersonalizationEditorManager.Instance.currentEditingRoot.transform, true);
                    b.UniqueIndex = PersonalizationEditorObjectManager.Instance.GetNextUniqueIndex();
                    b.ControllerInfo = PersonalizationEditorManager.Instance.currentEditingRoot.ControllerInfo;
                    PersonalizationEditorManager.Instance.SerializeRoot();
                    Hide();

                    callback?.Invoke();
                    callback = null;
                });
            }
        }
    }
}
