using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectBehaviour : MonoBehaviour
    {
        private bool _hasHiddenObjects;

        private Dictionary<GameObject, bool> _hiddenObjects;

        public string Name, Path;

        public int UniqueIndex;

        public Dictionary<string, object> PropertyValues;

        public ItemSpawnInfo ControllerInfo;

        public Vector3 SerializedScale;

        private List<PersonalizationEditorObjectBehaviour> _children;
        public List<PersonalizationEditorObjectBehaviour> Children
        {
            get
            {
                List<PersonalizationEditorObjectBehaviour> list = _children;
                if (list == null)
                {
                    list = new List<PersonalizationEditorObjectBehaviour>();
                    _children = list;
                }
                else
                {
                    list.Clear();
                }

                Transform transform = base.transform;
                if (transform.childCount > 0)
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        Transform child = transform.GetChild(i);
                        if (child)
                        {
                            PersonalizationEditorObjectBehaviour personalizationEditorObjectBehaviour = child.GetComponent<PersonalizationEditorObjectBehaviour>();
                            if (personalizationEditorObjectBehaviour)
                            {
                                list.Add(personalizationEditorObjectBehaviour);
                            }
                        }
                    }

                return list;
            }
        }

        public int ChildrenCount
        {
            get
            {
                Transform transform = base.transform;

                int result = transform.childCount;
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child)
                    {
                        PersonalizationEditorObjectBehaviour personalizationEditorObjectBehaviour = child.GetComponent<PersonalizationEditorObjectBehaviour>();
                        if (personalizationEditorObjectBehaviour)
                        {
                            result += personalizationEditorObjectBehaviour.ChildrenCount;
                        }
                    }
                }
                return result;
            }
        }

        private void Awake()
        {
            _children = new List<PersonalizationEditorObjectBehaviour>();
        }

        private void OnDestroy()
        {
            PersonalizationEditorObjectManager.Instance.RemoveInstantiatedObject(this);
        }

        public void HideChildren()
        {
            if (_hasHiddenObjects)
                return;

            Transform t = base.transform;
            if (t.childCount == 0)
                return;

            _hasHiddenObjects = true;

            if (_hiddenObjects == null)
                _hiddenObjects = new Dictionary<GameObject, bool>();
            else
                _hiddenObjects.Clear();

            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                _hiddenObjects.Add(child.gameObject, child.gameObject.activeSelf);
                child.gameObject.SetActive(false);
            }
        }

        public void ShowChildren()
        {
            if (!_hasHiddenObjects || _hiddenObjects == null || _hiddenObjects.Count == 0)
                return;

            _hasHiddenObjects = false;
            foreach (KeyValuePair<GameObject, bool> kv in _hiddenObjects)
            {
                kv.Key.SetActive(kv.Value);
            }
            _hiddenObjects.Clear();
        }

        public void SetChildrenActive(bool value)
        {
            if (value)
            {
                ShowChildren();
                return;
            }
            HideChildren();
        }

        public T GetPropertyValue<T>(string name, T defaultValue)
        {
            if (PropertyValues.IsNullOrEmpty())
                return defaultValue;

            if (!PropertyValues.TryGetValue(name, out object obj))
                return defaultValue;

            if (typeof(T) == typeof(float) && obj is double)
                return (T)(object)Convert.ToSingle(obj);

            if (typeof(T) == typeof(int) && obj is long)
                return (T)(object)Convert.ToInt32(obj);

            if (typeof(T) == typeof(Color) && obj is string str)
                return (T)(object)ModParseUtils.TryParseColor(str, Color.white);

            return (T)obj;
        }

        public void SetPropertyValue(string name, object value)
        {
            if (PropertyValues == null)
                PropertyValues = new Dictionary<string, object>();

            if (value is Color color)
            {
                value = ColorUtility.ToHtmlStringRGBA(color);
            }

            if (!PropertyValues.ContainsKey(name))
            {
                PropertyValues.Add(name, value);
            }
            else
            {
                PropertyValues[name] = value;
            }
        }

        public PersonalizationEditorObjectInfo Serialize()
        {
            if (!this || !gameObject)
                return null;

            PersonalizationEditorObjectInfo objectInfo = new PersonalizationEditorObjectInfo()
            {
                Name = Name,
                Path = Path,
                UniqueIndex = UniqueIndex,
                PropertyValues = PropertyValues ?? new Dictionary<string, object>(),
                Children = new List<PersonalizationEditorObjectInfo>()
            };
            objectInfo.InitializeTransformArrays();

            if (UniqueIndex == 0) // only root has unique index of zero
            {
                objectInfo.ResetRootTransform();
            }
            else
            {
                objectInfo.SetPosition(base.transform.localPosition);
                objectInfo.SetEulerAngles(base.transform.localEulerAngles);
                objectInfo.SetScale(base.transform.localScale);
            }

            List<PersonalizationEditorObjectBehaviour> list = Children;
            if (list == null)
                return objectInfo;

            foreach (PersonalizationEditorObjectBehaviour c in list)
            {
                if (!c || !c.gameObject)
                    continue;

                PersonalizationEditorObjectInfo cInfo = c.Serialize();
                if (cInfo != null)
                    objectInfo.Children.Add(cInfo);
            }
            return objectInfo;
        }
    }
}
