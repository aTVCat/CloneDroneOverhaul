using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class GameStateChangeController : ModuleBase
    {
        private float _soundVolume;
        private bool _lerpingVolume;
        private float _interpolator;

        protected override bool ExectuteFunctionAnyway()
        {
            return true;
        }

        public override void RunFunction(string name, object[] arguments)
        {
            if (name == "onBoltShutdown")
            {
                _soundVolume = AudioListener.volume;
                _lerpingVolume = true;
                _interpolator = 0f;
            }
        }

        public override void OnNewFrame()
        {
            if (_lerpingVolume)
            {
                _interpolator += 0.75f * Time.deltaTime;
                AudioListener.volume = BaseUtils.SmoothChangeFloat(_soundVolume, 0f, _interpolator);
                if (_interpolator > 0.99f)
                {
                    _lerpingVolume = false;
                }
            }
        }
    }
}
