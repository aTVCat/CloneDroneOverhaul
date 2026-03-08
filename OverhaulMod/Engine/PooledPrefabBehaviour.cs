using UnityEngine;

namespace OverhaulMod.Engine
{
    public class PooledPrefabBehaviour : MonoBehaviour
    {
        private float _timeToDeactivate;

        private GameObject _objectReference;
        public GameObject objectReference
        {
            get
            {
                if (!_objectReference)
                    _objectReference = base.gameObject;

                return _objectReference;
            }
        }

        private void Update()
        {
            float d = Time.deltaTime;
            float v = _timeToDeactivate - d;
            _timeToDeactivate = v;
            if (v <= 0f)
            {
                objectReference.SetActive(false);
            }
        }

        public void Activate(float time)
        {
            _timeToDeactivate = time;
            objectReference.SetActive(true);
        }
    }
}
