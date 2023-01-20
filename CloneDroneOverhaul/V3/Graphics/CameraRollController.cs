using CloneDroneOverhaul.V3;
using ModLibrary;
using UnityEngine;
using CloneDroneOverhaul.V3.Base;

namespace CloneDroneOverhaul.V3.Graphics
{
    public class CameraRollController : V3_ModControllerBase
    {
        private float z;
        private float x;
        private float y;
        private float offsetZ;
        private float offsetX;
        public float MaxValue = 2f;
        public float MaxOffset = 3f;
        public float Multipler = 0.04f;
        public float MultiplerX = 5f;
        public float MultiplerY = 10f;
        private float mouseX;
        private float mouseY;

        private bool _settingEnabled;
        private float _multiplerSingleplayer;
        private float _multiplerMultiplayer;

        public static bool IsUIHidden;

        private void Start()
        {
            IsUIHidden = false;
        }

        private void FixedUpdate()
        {
            tryRollCamera();
        }

        /// <summary>
        /// Camera rolling
        /// </summary>
        private void tryRollCamera()
        {
            RobotShortInformation playerRaw = Gameplay.GameStatisticsController.GameStatistics.PlayerRobotInformation;
            if (playerRaw != null && playerRaw.IsFPM)
            {
                FirstPersonMover player = playerRaw.Instance as FirstPersonMover;
                mouseX = Input.GetAxis("Mouse X") * MultiplerX;
                mouseY = Input.GetAxis("Mouse Y") * MultiplerY;
                if (!Singleton<InputManager>.Instance.IsCursorEnabled() && player.IsPlayerInputEnabled() && !player.IsAimingBow())
                {
                    bool a = Input.GetKey(KeyCode.A);
                    bool d = Input.GetKey(KeyCode.D);
                    bool w = Input.GetKey(KeyCode.W);
                    bool s = Input.GetKey(KeyCode.S);
                    bool sh = Input.GetKey(KeyCode.LeftShift);

                    if (a && !d)
                    {
                        z += (MaxValue - z) * Multipler;
                    }
                    else if (!a && d)
                    {
                        z += (-MaxValue - z) * Multipler;
                    }
                    else
                    {
                        z += (0 - z) * Multipler;
                    }

                    if (w && !s)
                    {
                        x += (MaxValue - x) * (Multipler * 2);
                    }
                    else if (!w && s)
                    {
                        x += (-(MaxValue * 2) - x) * (Multipler * 2);
                    }
                    else
                    {
                        x += (0 - x) * (Multipler * 2);
                    }

                    if (sh)
                    {
                        if (player.GetPrivateField<bool>("_isJetpackEngaged"))
                        {
                            x += (24 - x) * 0.1f;
                        }
                    }

                    if (player.GetPrivateField<float>("_distanceAboveGroundY") > 0.1f)
                    {
                        x += (-32 - x) * 0.02f;
                    }
                    if (player.GetPrivateField<bool>("_isKicking"))
                    {
                        x += (-32 - x) * 0.005f;
                    }

                    if (player.IsUsingAnyAbility() || player.GetPrivateField<bool>("_isAirCleaving") || player.GetPrivateField<bool>("_isOnFloorFromKick"))
                    {
                        x += (128 - x) * 0.01f;
                    }

                    if (!Physics.Raycast(player.transform.position + new Vector3(0, 1, 0), -Vector3.up, 10f, PhysicsManager.GetEnvironmentLayerMask()))
                    {
                        x += (64 - x) * 0.02f;
                    }

                    if (player.IsDamaged(MechBodyPartType.LeftLeg) && !player.IsDamaged(MechBodyPartType.RightLeg))
                    {
                        offsetZ += (MaxOffset - offsetZ) * 0.01f;
                    }
                    else if (player.IsDamaged(MechBodyPartType.RightLeg) && !player.IsDamaged(MechBodyPartType.LeftLeg))
                    {
                        offsetZ += (-MaxOffset - offsetZ) * 0.01f;
                    }
                    else
                    {
                        offsetZ += (0 - offsetZ) * 0.01f;
                    }

                    y += (mouseX - y) * 0.04f;
                    offsetX += (-mouseY - offsetX) * 0.04f;
                }
                else
                {
                    resetCameraRotator();
                }

                setCameraRoll(playerRaw.RobotCamera, new Vector3(Mathf.Clamp(x, -25, 25) / 4f + offsetX, Mathf.Clamp(y, -25, 25), Mathf.Clamp(z, -25, 25) + offsetZ));
            }
        }

        private void resetCameraRotator()
        {
            offsetZ = 0;
            offsetX = 0;
            x = 0;
            y = 0;
            z = 0;
        }

        private void setCameraRoll(Camera cam, Vector3 rotation)
        {
            bool isSettingEnabled = _settingEnabled;
            if (!isSettingEnabled)
            {
                rotation = Vector3.zero;
            }

            float multipler;
            if (GameModeManager.IsMultiplayer())
            {
                multipler = _multiplerMultiplayer;
            }
            else
            {
                multipler = _multiplerSingleplayer;
            }

            if (cam != null)
            {
                cam.gameObject.transform.localEulerAngles = rotation * multipler;
            }
        }

        public override void OnSettingRefreshed(in string settingName, in object value)
        {
            if(settingName == "Graphics.Additions.Camera Rolling")
            {
                _settingEnabled = (bool)value;
            }
            if (settingName == "Graphics.Additions.(Multiplayer) Roll Multipler")
            {
                _multiplerMultiplayer = (float)value;
            }
            if (settingName == "Graphics.Additions.Roll Multipler")
            {
                _multiplerSingleplayer = (float)value;
            }
        }

        /// <summary>
        /// Swith hud by pressing F2
        /// </summary>
        public static void SwitchHud()
        {
            IsUIHidden = !IsUIHidden;
            if (GameModeManager.IsBattleRoyale())
            {
                GameUIRoot.Instance.BattleRoyaleUI.gameObject.SetActive(!IsUIHidden);
            }

            bool shouldHide = CutSceneManager.Instance.IsInCutscene() || IsUIHidden;
            if (shouldHide)
            {
                GameUIRoot.Instance.SetPlayerHUDVisible(false);
                return;
            }
            GameUIRoot.Instance.SetPlayerHUDVisible(!IsUIHidden);
        }
    }
}
