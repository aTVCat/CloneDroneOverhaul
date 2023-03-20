using CDOverhaul.HUD;
using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Misc
{
    public class PhotoModeRevampController : OverhaulController
    {
        private static PhotoManager m_Manager;
        public static bool Revamp => m_Manager != null && !m_Manager.enabled;

        public static bool IsInPhotoMode { get; private set; }
        public FlyingCameraController CameraFlyController { get; private set; }

        private EscMenu m_OriginalPauseMenu;
        private OverhaulPauseMenu m_PauseMenu;

        private bool m_WasInputEnabled;

        public override void Initialize()
        {
            m_Manager = PhotoManager.Instance;
            //m_Manager.enabled = false;
        }

        protected override void OnDisposed()
        {
            CameraFlyController = null;
            m_OriginalPauseMenu = null;
            m_PauseMenu = null;
        }

        private void getReferences()
        {
            m_OriginalPauseMenu = GameUIRoot.Instance.EscMenu;
            m_PauseMenu = GetController<OverhaulPauseMenu>();
        }

        private void Update()
        {
            if(!Cursor.visible && !EnableCursorController.HasToEnableCursor() && Input.GetKeyDown(KeyCode.BackQuote) && Revamp)
            {
                TogglePhotoMode();
            }
        }

        public void TogglePhotoMode()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            getReferences();
            if((m_OriginalPauseMenu.gameObject.activeSelf || m_PauseMenu.gameObject.activeSelf) && !IsInPhotoMode)
            {
                return;
            }

            Character player = CharacterTracker.Instance.GetPlayer();
            if (player == null)
            {
                return;
            }
            PlayerInputController c = player.GetComponent<PlayerInputController>();

            Camera camera = player.GetPlayerCamera();
            if(camera == null)
            {
                return;
            }

            IsInPhotoMode = !IsInPhotoMode;
            PhotoManager.Instance.SetPrivateField("_isInPhotoMode", IsInPhotoMode);

            if (IsInPhotoMode)
            {
                if (c != null)
                {
                    m_WasInputEnabled = c.enabled; c.enabled = false;
                }
                TimeManager.Instance.OnGamePaused();
                addControllerIfRequired(camera);
                player.SetCameraAnimatorEnabled(!IsInPhotoMode);
            }
            else
            {
                if (c != null)
                {
                    c.enabled = m_WasInputEnabled;
                }
                TimeManager.Instance.OnGameUnPaused();
                if(CameraFlyController != null)
                {
                    CameraFlyController.gameObject.SetActive(false);
                    player.MovePlayerCameraBack(0f);
                    player.SetCameraAnimatorEnabled(true);
                }
            }

            GameUIRoot.Instance.RefreshCursorEnabled();
        }

        private void addControllerIfRequired(Camera camera)
        {
            if(camera == null)
            {
                return;
            }

            bool shouldAdd = CameraFlyController == null;
            if (shouldAdd)
            {
                CameraFlyController = Instantiate(PhotoManager.Instance.CameraControllerPrefab);
            }
            CameraFlyController.ReparentCameraToController(camera);
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}