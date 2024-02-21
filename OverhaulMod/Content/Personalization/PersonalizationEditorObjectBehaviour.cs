using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectBehaviour : MonoBehaviour
    {
        public string Name, Path;
        public Dictionary<string, object> PropertyValues;

        private List<PersonalizationEditorObjectBehaviour> m_children;
        public List<PersonalizationEditorObjectBehaviour> children
        {
            get
            {
                List<PersonalizationEditorObjectBehaviour> list = m_children;
                list.Clear();

                Transform transform = base.transform;
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

        private void Awake()
        {
            m_children = new List<PersonalizationEditorObjectBehaviour>();
            PersonalizationEditorObjectManager.Instance.AddInstantiatedObject(this);
        }

        private void OnDestroy()
        {
            PersonalizationEditorObjectManager.Instance.RemoveInstantiatedObject(this);
        }

        public object GetPropertyValue(string name, object defaultValue)
        {
            return !PropertyValues.TryGetValue(name, out object obj) ? defaultValue : obj;
        }

        public void SetPropertyValue(string name, object value)
        {
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
            PersonalizationEditorObjectInfo objectInfo = new PersonalizationEditorObjectInfo()
            {
                Name = Name,
                Path = Path,
                PropertyValues = PropertyValues ?? new Dictionary<string, object>(),
                Children = new List<PersonalizationEditorObjectInfo>()
            };
            objectInfo.SetPosition(base.transform.localPosition);
            objectInfo.SetEulerAngles(base.transform.localEulerAngles);
            objectInfo.SetScale(base.transform.localScale);

            foreach (PersonalizationEditorObjectBehaviour c in children)
                objectInfo.Children.Add(c.Serialize());

            return objectInfo;
        }
    }
}
