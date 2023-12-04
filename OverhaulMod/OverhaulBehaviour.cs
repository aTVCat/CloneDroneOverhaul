using OverhaulMod.Utils;
using System;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class OverhaulBehaviour : MonoBehaviour
    {
        private ModdedObject m_moddedObject;
        public ModdedObject moddedObject
        {
            get
            {
                if (!m_moddedObject)
                {
                    m_moddedObject = base.GetComponent<ModdedObject>();
                }
                return m_moddedObject;
            }
        }

        public T GetObject<T>(int index) where T : UnityEngine.Object
        {
            return moddedObject.GetObject<T>(index);
        }
        public UnityEngine.Object GetObject(int index, Type type)
        {
            return moddedObject.GetObject(type, index);
        }

        public T GetObject<T>(string name) where T : UnityEngine.Object
        {
            return moddedObject.GetObject<T>(name);
        }

        public UnityEngine.Object GetObject(string name, Type type)
        {
            return moddedObject.GetObject(type, name);
        }
    }
}
