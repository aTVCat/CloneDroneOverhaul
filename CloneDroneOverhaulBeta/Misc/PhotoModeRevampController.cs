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

        private byte m_Condition = 0;

        public override void Initialize()
        {
            m_Manager = PhotoManager.Instance;
            m_Manager.enabled = false;

            OverhaulCanvasController c = GetController<OverhaulCanvasController>();
            if(c != null)
            {
                OverhaulPhotoModeControls controls = c.AddHUD<OverhaulPhotoModeControls>(c.HUDModdedObject.GetObject<ModdedObject>(1));
            }
        }

        protected override void OnDisposed()
        {
            CameraFlyController = null;
            m_OriginalPauseMenu = null;
            m_PauseMenu = null;
        }

        public bool IsAbleToTogglePhotoMode()
        {
            return IsInPhotoMode || ((m_OriginalPauseMenu == null || !m_OriginalPauseMenu.gameObject.activeSelf) && (m_PauseMenu == null || !m_PauseMenu.gameObject.activeSelf));
        }

        private void getReferences()
        {
            m_OriginalPauseMenu = GameUIRoot.Instance.EscMenu;
            m_PauseMenu = GetController<OverhaulPauseMenu>();
        }

        private void Update()
        {
            if((!Cursor.visible || IsInPhotoMode) && IsAbleToTogglePhotoMode() && !EnableCursorController.HasToEnableCursor() && Input.GetKeyDown(KeyCode.BackQuote) && Revamp)
            {
                TogglePhotoMode();
            }

            if (IsInPhotoMode)
            {
                bool mouseDown = Input.GetMouseButton(1);

                if (mouseDown)
                {
                    if (m_Condition != 0)
                    {
                        EnableCursorController.RemoveCondition(m_Condition);
                        m_Condition = 0;
                    }
                }
                else
                {
                    if (m_Condition == 0)
                    {
                        byte c = EnableCursorController.AddCondition();
                        m_Condition = c;
                    }
                }
            }
            else
            {
                if (m_Condition != 0)
                {
                    EnableCursorController.RemoveCondition(m_Condition);
                    m_Condition = 0;
                }
            }
        }

        public void TogglePhotoMode()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            getReferences();

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