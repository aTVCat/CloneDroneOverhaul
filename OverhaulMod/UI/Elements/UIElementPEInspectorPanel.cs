using OverhaulMod.Content.Personalization.Objects;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPEInspectorPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnPositionChanged))]
        [UIElement("PositionPanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field _positionField;

        [UIElementAction(nameof(OnRotationChanged))]
        [UIElement("RotationPanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field _rotationField;

        [UIElementAction(nameof(OnScaleChanged))]
        [UIElement("ScalePanel", typeof(UIElementVector3Field))]
        private readonly UIElementVector3Field _scaleField;

        [UIElementCallback(true)]
        [UIElementAction(nameof(OnObjectNameChanged))]
        [UIElement("ObjectNameField")]
        private readonly InputField _objectNameField;

        [UIElement("GroupHeader", false)]
        private readonly ModdedObject _groupHeader;

        [UIElement("GroupHolder", false)]
        private readonly ModdedObject _groupHolder;

        [UIElement("Content")]
        private readonly Transform _container;

        [UIElement("FieldsStore", typeof(UIElementPEInspectorFieldsStore), false)]
        private readonly UIElementPEInspectorFieldsStore _fieldsStore;

        private PersonalizationEditorPlacedObject _inspectingObject;

        private int _inspectingObjectUniqueIndex;

        private bool _prevObjectState;

        private bool _disableCallbacks;

        protected override void OnInitialized()
        {
            _inspectingObjectUniqueIndex = -1;
        }

        private void LateUpdate()
        {
            bool newObjectState = _inspectingObject;
            if (newObjectState != _prevObjectState)
            {
                if (!newObjectState)
                {
                    Inspect(null);
                }
                _prevObjectState = newObjectState;
            }
        }

        public PersonalizationEditorPlacedObject GetInspectingObject() => _inspectingObject;

        public int GetInspectingObjectUniqueIndex() => _inspectingObjectUniqueIndex;

        public void Refresh()
        {
            Inspect(PersonalizationEditorObjectManager.Instance.GetInstantiatedObject(_inspectingObjectUniqueIndex));
        }

        public void Inspect(PersonalizationEditorPlacedObject objectBehaviour)
        {
            _inspectingObject = objectBehaviour;

            ModUIs.HideGenericColorPicker();
            populateGroups();

            if (!objectBehaviour)
            {
                _objectNameField.text = string.Empty;
                GlobalEventManager.Instance.Dispatch(PersonalizationEditorObjectManager.OBJECT_SELECTION_CHANGED_EVENT);
                return;
            }

            _disableCallbacks = true;

            _objectNameField.text = objectBehaviour.Name;
            _positionField.Vector = objectBehaviour.transform.localPosition;
            _rotationField.Vector = objectBehaviour.transform.localEulerAngles;
            _scaleField.Vector = objectBehaviour.transform.localScale;

            _disableCallbacks = false;

            GlobalEventManager.Instance.Dispatch(PersonalizationEditorObjectManager.OBJECT_SELECTION_CHANGED_EVENT);
        }

        private void populateGroups()
        {
            if (_container.childCount != 0) TransformUtils.DestroyAllChildren(_container);

            if (!_inspectingObject) return;

            PersonalizationEditorComponent[] editableComponents = _inspectingObject.GetComponents<PersonalizationEditorComponent>();
            foreach (PersonalizationEditorComponent component in editableComponents)
            {
                populateComponent(component);
            }
        }

        private void populateComponent(PersonalizationEditorComponent component)
        {
            UIElementPEInspectorGroupHeader groupHeader = instantiateGroupHeader(component);
            UIElementPEInspectorGroup group = instantiateGroup(component);

            groupHeader.SetGroup(group);
            groupHeader.SetIsExpanded(true);
        }

        private UIElementPEInspectorGroupHeader instantiateGroupHeader(PersonalizationEditorComponent component)
        {
            ModdedObject groupHeaderObject = Instantiate(_groupHeader, _container);
            groupHeaderObject.gameObject.SetActive(true);

            UIElementPEInspectorGroupHeader groupHeader = groupHeaderObject.gameObject.AddComponent<UIElementPEInspectorGroupHeader>();
            groupHeader.InitializeAsElement();
            groupHeader.SetComponent(component);

            return groupHeader;
        }

        private UIElementPEInspectorGroup instantiateGroup(PersonalizationEditorComponent component)
        {
            ModdedObject groupHolderObject = Instantiate(_groupHolder, _container);
            groupHolderObject.gameObject.SetActive(true);

            UIElementPEInspectorGroup groupHolder = groupHolderObject.gameObject.AddComponent<UIElementPEInspectorGroup>();
            groupHolder.InitializeAsElement();
            groupHolder.PopulateFields(component, _fieldsStore);

            return groupHolder;
        }

        public void OnPositionChanged(Vector3 value)
        {
            if (_disableCallbacks) return;

            PersonalizationEditorPlacedObject objectBehaviour = _inspectingObject;
            if (objectBehaviour)
            {
                objectBehaviour.transform.localPosition = value;
                objectBehaviour.SerializedPosition = value;
            }
        }

        public void OnRotationChanged(Vector3 value)
        {
            if (_disableCallbacks) return;

            PersonalizationEditorPlacedObject objectBehaviour = _inspectingObject;
            if (objectBehaviour)
            {
                objectBehaviour.transform.localEulerAngles = value;
                objectBehaviour.SerializedEulerAngles = value;
            }
        }

        public void OnScaleChanged(Vector3 value)
        {
            if (_disableCallbacks) return;

            PersonalizationEditorPlacedObject objectBehaviour = _inspectingObject;
            if (objectBehaviour)
            {
                objectBehaviour.transform.localScale = value;
                objectBehaviour.SerializedScale = value;
            }
        }

        public void OnObjectNameChanged(string str)
        {
            if (_disableCallbacks) return;

            PersonalizationEditorPlacedObject objectBehaviour = _inspectingObject;
            if (objectBehaviour) objectBehaviour.Name = str;
        }
    }
}