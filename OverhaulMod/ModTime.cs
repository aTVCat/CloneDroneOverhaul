namespace OverhaulMod
{
    public class ModTime : Singleton<ModTime>
    {
        private bool _hasFixedUpdated;

        private int _fixedFrameCount;

        private void FixedUpdate()
        {
            _fixedFrameCount++;
            _hasFixedUpdated = true;
        }

        private void LateUpdate()
        {
            _hasFixedUpdated = false;
        }

        public int GetFixedFrameCount()
        {
            return _fixedFrameCount;
        }

        public bool HasFixedUpdatedThisFrame()
        {
            return _hasFixedUpdated;
        }
    }
}
