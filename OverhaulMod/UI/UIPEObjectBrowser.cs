using OverhaulMod.Content.Personalization;
using OverhaulMod.Content.Personalization.Objects;
using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPEObjectBrowser : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElement("ObjectDisplayPrefab", false)]
        private readonly ModdedObject _objectDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform _container;

        public Action Callback
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            System.Collections.Generic.List<PersonalizationEditorObjectSpawnInfo> list = PersonalizationEditorObjectManager.Instance.GetObjectInfos();
            foreach (PersonalizationEditorObjectSpawnInfo obj in list)
            {
                if (obj.Path == "Empty")
                    continue;

                ModdedObject moddedObject = Instantiate(_objectDisplayPrefab, _container);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = obj.DisplayName;
                moddedObject.GetObject<Text>(1).text = LocalizationManager.Instance.GetTranslatedString($"ce_object_{obj.DisplayName}");

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    PersonalizationEditorPlacedObject b = PersonalizationEditorObjectManager.Instance.PlaceObject(obj.Path, PersonalizationEditorManager.Instance.EditingRoot.transform, true);
                    b.UniqueIndex = PersonalizationEditorObjectManager.Instance.GetNextUniqueIndex();
                    b.SpawnInfo = PersonalizationEditorManager.Instance.EditingRoot.SpawnInfo;
                    b.SerializedScale = Vector3.one;
                    PersonalizationEditorObjectManager.Instance.AddInstantiatedObject(b);
                    PersonalizationEditorManager.Instance.SerializeRoot();
                    Hide();

                    Callback?.Invoke();
                    Callback = null;
                });
            }
        }
    }
}
