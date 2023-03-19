using UnityEngine;

namespace CDOverhaul.ArenaRemaster
{
    public class ArenaRemasterArrowBlinker : OverhaulBehaviour
    {
        private float m_TimeToUpdate = -1;
        private GameObject Off;
        private GameObject On;
        private ArenaRemasterArrowBlinker m_NextArrowBlinker;

        public ArenaRemasterArrowBlinker Initialize(ModdedObject moddedObject, in ArenaRemasterArrowBlinker nextArrow)
        {
            if(moddedObject == null)
            {
                moddedObject = GetComponent<ModdedObject>();
            }

            m_NextArrowBlinker = nextArrow;
            Off = moddedObject.GetObject<Transform>(0).gameObject;
            On = moddedObject.GetObject<Transform>(1).gameObject;
            ChangeState(false);

            return this;
        }

        protected override void OnDisposed()
        {
            Off = null;
            On = null;
            m_NextArrowBlinker = null;
        }

        public void ChangeState(in bool value)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (value)
            {
                m_TimeToUpdate = Time.time + 0.5f;
            }
            Off.SetActive(!value);
            On.SetActive(value);
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (m_TimeToUpdate != -1 && Time.time >= m_TimeToUpdate)
            {
                ChangeState(false);
                m_NextArrowBlinker.ChangeState(true);
                m_TimeToUpdate = -1;
            }
        }
    }
}
