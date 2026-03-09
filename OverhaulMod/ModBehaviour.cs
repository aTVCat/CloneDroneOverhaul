using OverhaulMod.Utils;
using System;
using UnityEngine;

namespace OverhaulMod
{
    public class ModBehaviour : MonoBehaviour
    {
        private ModdedObject _moddedObjectComponent;
        public ModdedObject ModdedObjectComponent
        {
            get
            {
                if (!_moddedObjectComponent)
                {
                    _moddedObjectComponent = base.GetComponent<ModdedObject>();
                }
                return _moddedObjectComponent;
            }
        }

        public T GetObject<T>(int index) where T : UnityEngine.Object
        {
            return ModdedObjectComponent.GetObject<T>(index);
        }
        public UnityEngine.Object GetObject(int index, Type type)
        {
            return ModdedObjectComponent.GetObject(type, index);
        }

        public T GetObject<T>(string name) where T : UnityEngine.Object
        {
            return ModdedObjectComponent.GetObject<T>(name);
        }

        public UnityEngine.Object GetObject(string name, Type type)
        {
            return ModdedObjectComponent.GetObject(type, name);
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
