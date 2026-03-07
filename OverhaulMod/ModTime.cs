namespace OverhaulMod
{
    public class ModTime : Singleton<ModTime>
    {
        private bool m_hasFixedUpdated;

        private int m_fixedFrameCount;

        private void FixedUpdate()
        {
            m_fixedFrameCount++;
            m_hasFixedUpdated = true;
        }

        private void LateUpdate()
        {
            m_hasFixedUpdated = false;
        }

        public int GetFixedFrameCount()
        {
            return m_fixedFrameCount;
        }

        public bool HasFixedUpdatedThisFrame()
        {
            return m_hasFixedUpdated;
        }
    }
}
