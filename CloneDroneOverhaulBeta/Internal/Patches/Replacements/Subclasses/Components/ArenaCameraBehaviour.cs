using UnityEngine;

namespace CDOverhaul.Patches
{
    [RequireComponent(typeof(Camera))]
    public class ArenaCameraBehaviour : MonoBehaviour
    {
        private SpeechAudioManager m_SpeechAudioManager;
        private CutSceneManager m_CutSceneManager;
        public bool AreManagersNull => m_SpeechAudioManager == null || m_CutSceneManager == null;

        public Camera TheCamera;

        private float _timeToStopRendering;
        private bool _renderActively;

        public bool DoRender => Time.timeScale != 0f;

        private Vector3 _vectorPrevFrame;

        public void Initialze(in Camera camera)
        {
            TheCamera = camera;
            m_SpeechAudioManager = SpeechAudioManager.Instance;
            m_CutSceneManager = CutSceneManager.Instance;
        }

        public void StartRendering()
        {
            _timeToStopRendering = Time.time + 1f;
        }

        private void Update()
        {
            if (AreManagersNull)
            {
                TheCamera.enabled = true;
                base.enabled = false;
                return;
            }

            Vector3 newPos = base.transform.position;
            if (_vectorPrevFrame != newPos)
                StartRendering();
            _vectorPrevFrame = newPos;

            if (Time.frameCount % 10 == 0)
            {
                if (SpeechAudioManager.Instance.IsAnyoneSpeaking() || CutSceneManager.Instance.IsInCutscene())
                    StartRendering();
            }

            _renderActively = Time.time < _timeToStopRendering;
            if (_renderActively)
            {
                TheCamera.enabled = DoRender;
                return;
            }

            TheCamera.enabled = false;
            if (DoRender && Time.frameCount % 2 == 0)
                TheCamera.Render();
        }
    }
}