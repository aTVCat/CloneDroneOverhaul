using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectBehaviour : MonoBehaviour
    {
        private bool m_hasHiddenObjects;

        private Dictionary<GameObject, bool> m_hiddenObjects;

        public string Name, Path;

        public bool IsRoot;

        public int UniqueIndex, NextUniqueIndex;

        public Dictionary<string, object> PropertyValues;

        public PersonalizationControllerInfo ControllerInfo;

        public Vector3 SerializedScale;

        private List<PersonalizationEditorObjectBehaviour> m_children;
        public List<PersonalizationEditorObjectBehaviour> children
        {
            get
            {
                List<PersonalizationEditorObjectBehaviour> list = m_children;
                if (list == null)
                {
                    list = new List<PersonalizationEditorObjectBehaviour>();
                    m_children = list;
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

        public int childrenCount
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
                            result += personalizationEditorObjectBehaviour.childrenCount;
                        }
                    }
                }
                return result;
            }
        }

        private void Awake()
        {
            m_children = new List<PersonalizationEditorObjectBehaviour>();
        }

        private void OnDestroy()
        {
            PersonalizationEditorObjectManager.Instance.RemoveInstantiatedObject(this);
        }

        public void HideChildren()
        {
            if (m_hasHiddenObjects)
                return;

            Transform t = base.transform;
            if (t.childCount == 0)
                return;

            m_hasHiddenObjects = true;

            if (m_hiddenObjects == null)
                m_hiddenObjects = new Dictionary<GameObject, bool>();
            else
                m_hiddenObjects.Clear();

            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                m_hiddenObjects.Add(child.gameObject, child.gameObject.activeSelf);
                child.gameObject.SetActive(false);
            }
        }

        public void ShowChildren()
        {
            if (!m_hasHiddenObjects || m_hiddenObjects == null || m_hiddenObjects.Count == 0)
                return;

            m_hasHiddenObjects = false;
            foreach (KeyValuePair<GameObject, bool> kv in m_hiddenObjects)
            {
                kv.Key.SetActive(kv.Value);
            }
            m_hiddenObjects.Clear();
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
                return (T)(object)ModParseUtils.TryParseToColor(str, Color.white);

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

            if (IsRoot)
                NextUniqueIndex = PersonalizationEditorObjectManager.Instance.GetCurrentUniqueIndex();

            PersonalizationEditorObjectInfo objectInfo = new PersonalizationEditorObjectInfo()
            {
                Name = Name,
                Path = Path,
                IsRoot = IsRoot,
                UniqueIndex = UniqueIndex,
                NextUniqueIndex = NextUniqueIndex,
                PropertyValues = PropertyValues ?? new Dictionary<string, object>(),
                Children = new List<PersonalizationEditorObjectInfo>()
            };
            objectInfo.SetPosition(base.transform.localPosition);
            objectInfo.SetEulerAngles(base.transform.localEulerAngles);
            objectInfo.SetScale(base.transform.localScale);

            List<PersonalizationEditorObjectBehaviour> list = children;
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
