using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.Controllers
{
    /// <summary>
    /// A static class made to control cameras
    /// </summary>
    public static class GameCameraController
    {
        private static Dictionary<Camera, int> _cameraCullingMasks = new Dictionary<Camera, int>();
        private static List<AudioListener> _audioListeners = new List<AudioListener>();

        private const int DONT_RENDER_CULLING_MASK = 0;

        private static int _lastTargetFrame = -1;
        private static bool _renderingGame = true;
        private static bool _audioEnabled = true;

        public static void SetRendererEnabled(bool value, bool loweOrHighFramerate)
        {
            if (!value && _renderingGame)
            {
                Camera[] _cameras = Modules.GameInformationManager.UnoptimizedThings.GetFPSLoweringStuff().AllCameras;
                _cameraCullingMasks.Clear();
                foreach (Camera cam in _cameras)
                {
                    if (cam != null)
                    {
                        _cameraCullingMasks.Add(cam, cam.cullingMask);
                        cam.cullingMask = DONT_RENDER_CULLING_MASK;
                    }
                }

                if (loweOrHighFramerate)
                {
                    SetTargetFramerate(30);
                }
            }
            else if (value && !_renderingGame)
            {
                foreach (Camera cam in _cameraCullingMasks.Keys)
                {
                    cam.cullingMask = _cameraCullingMasks[cam];
                }

                if (loweOrHighFramerate)
                {
                    SetTargetFramerate(_lastTargetFrame);
                }
            }

            _renderingGame = value;
        }

        public static void SetAudioListenersEnabled(bool value)
        {
            if (_audioEnabled && !value)
            {
                _audioListeners.Clear();
                AudioListener[] listeners = UnityEngine.Object.FindObjectsOfType<AudioListener>();
                foreach (AudioListener listener in listeners)
                {
                    _audioListeners.Add(listener);
                    listener.enabled = value;
                }
            }
            else if (!_audioEnabled && value)
            {
                foreach (AudioListener listener in _audioListeners)
                {
                    if (listener != null)
                    {
                        listener.enabled = value;
                    }
                }
            }
            _audioEnabled = value;
        }

        public static void SetTargetFramerate(int value)
        {
            Application.targetFrameRate = value;
        }
    }
}
