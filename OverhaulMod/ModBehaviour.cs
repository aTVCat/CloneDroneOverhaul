using OverhaulMod.Utils;
using System;
using UnityEngine;

namespace OverhaulMod
{
    public class ModBehaviour : MonoBehaviour
    {
        private ModdedObject _moddedObject;
        public ModdedObject moddedObjectReference
        {
            get
            {
                if (!_moddedObject)
                {
                    _moddedObject = base.GetComponent<ModdedObject>();
                }
                return _moddedObject;
            }
        }

        public T GetObject<T>(int index) where T : UnityEngine.Object
        {
            return moddedObjectReference.GetObject<T>(index);
        }
        public UnityEngine.Object GetObject(int index, Type type)
        {
            return moddedObjectReference.GetObject(type, index);
        }

        public T GetObject<T>(string name) where T : UnityEngine.Object
        {
            return moddedObjectReference.GetObject<T>(name);
        }

        public UnityEngine.Object GetObject(string name, Type type)
        {
            return moddedObjectReference.GetObject(type, name);
        }

        public virtual void Awake()
        {

        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void OnDestroy()
        {

        }
    }
}
