using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectSpawnInfo
    {
        public string Name, Path;

        public List<PersonalizationEditorObjectPropertyValueInfo> Properties;

        public Func<Transform, GameObject> InstantiateFunction;

        public void AddProperty<T>(string name)
        {
            List<PersonalizationEditorObjectPropertyValueInfo> list = Properties;
            if (list == null)
            {
                list = new List<PersonalizationEditorObjectPropertyValueInfo>();
                Properties = list;
            }

            list.Add(new PersonalizationEditorObjectPropertyValueInfo()
            {
                PropertyType = typeof(T),
                PropertyName = name
            });
        }

        public GameObject Instantiate(Transform parent)
        {
            return InstantiateFunction(parent);
        }
    }
}
