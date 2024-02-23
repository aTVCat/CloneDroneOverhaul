using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class PooledPrefabBehaviour : MonoBehaviour
    {
        private float m_timeToDeactivate;

        private GameObject m_objectReference;
        public GameObject objectReference
        {
            get
            {
                if (!m_objectReference)
                    m_objectReference = base.gameObject;

                return m_objectReference;
            }
        }

        private void Update()
        {
            float d = Time.deltaTime;
            float v = m_timeToDeactivate - d;
            m_timeToDeactivate = v;
            if(v <= 0f)
            {
                objectReference.SetActive(false);
            }
        }

        public void Activate(float time)
        {
            m_timeToDeactivate = time;
            objectReference.SetActive(true);
        }
    }
}
