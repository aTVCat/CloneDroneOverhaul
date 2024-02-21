using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectSpawnInfo
    {
        public string Name, Path;

        public List<PersonalizationEditorObjectPropertyInfo> Properties;

        public Func<Transform, GameObject> InstantiateFunction;

        public void AddProperty<T>(string name)
        {
            List<PersonalizationEditorObjectPropertyInfo> list = Properties;
            if (list == null)
            {
                list = new List<PersonalizationEditorObjectPropertyInfo>();
                Properties = list;
            }

            list.Add(new PersonalizationEditorObjectPropertyInfo()
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
