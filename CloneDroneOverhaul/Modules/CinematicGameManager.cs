using CloneDroneOverhaul.Utilities;
using ModLibrary;
using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class CinematicGameManager : ModuleBase
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
        float mouseX;
        float mouseY;

        public static bool IsUIHidden;
        public override bool IsEnabled()
        {
            return true;
        }
        public override void OnActivated()
        {
            IsUIHidden = false;
        }
        public override void OnFixedUpdate()
        {
            updateCameraRotator();
        }

        /// <summary>
        /// Camera rolling
        /// </summary>
        private void updateCameraRotator()
        {
            RobotShortInformation playerRaw = CharacterTracker.Instance.GetPlayer().GetRobotInfo();
            if (!playerRaw.IsNull && playerRaw.IsFPM)
            {
                FirstPersonMover player = playerRaw.Instance as FirstPersonMover;
                mouseX = Input.GetAxis("Mouse X") * MultiplerX;
                mouseY = Input.GetAxis("Mouse Y") * MultiplerY;
                if (!Singleton<InputManager>.Instance.IsCursorEnabled() && player.IsPlayerInputEnabled() && !player.IsAimingBow())
                {
                    if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                    {
                        z += (MaxValue - z) * Multipler;
                    }
                    if (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
                    {
                        z += (-MaxValue - z) * Multipler;
                    }
                    if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)))
                    {
                        z += (0 - z) * Multipler;
                    }

                    if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                    {
                        x += (MaxValue - x) * (Multipler * 2);
                    }
                    if (!Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
                    {
                        x += (-(MaxValue * 2) - x) * (Multipler * 2);
                    }
                    if ((!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)))
                    {
                        x += (0 - x) * (Multipler * 2);
                    }

                    if (Input.GetKey(KeyCode.LeftShift))
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
                    if (player.IsDamaged(MechBodyPartType.RightLeg) && !player.IsDamaged(MechBodyPartType.LeftLeg))
                    {
                        offsetZ += (-MaxOffset - offsetZ) * 0.01f;
                    }
                    if ((!player.IsDamaged(MechBodyPartType.RightLeg) && !player.IsDamaged(MechBodyPartType.LeftLeg)) || (player.IsDamaged(MechBodyPartType.RightLeg) && player.IsDamaged(MechBodyPartType.LeftLeg)))
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

                setCameraRoll(playerRaw.RobotCamera, new Vector3(Mathf.Clamp(x, -45, 45) / 4f + offsetX, Mathf.Clamp(y, -45, 45), Mathf.Clamp(z, -45, 45) + offsetZ));
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
        private void setCameraRoll(Camera cam, Vector3 rotation) //Graphics.Additions.Roll Multipler
        {
            bool isSettingEnabled = OverhaulMain.GetSetting<bool>("Graphics.Additions.Camera Rolling");
            if (!isSettingEnabled)
            {
                rotation = Vector3.zero;
            }
            float multipler = 1;
            if (GameModeManager.IsMultiplayer())
            {
                multipler = OverhaulMain.GetSetting<float>("Graphics.Additions.(Multiplayer) Roll Multipler");
            }
            else
            {
                multipler = OverhaulMain.GetSetting<float>("Graphics.Additions.Roll Multipler");
            }
            if (cam != null)
            {
                cam.gameObject.transform.localEulerAngles = rotation * multipler;
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

            bool shouldHide = CutSceneManager.Instance.IsInCutscene() || Utilities.PlayerUtilities.GetPlayerRobotInfo().IsNull || IsUIHidden;
            if (shouldHide)
            {
                GameUIRoot.Instance.SetPlayerHUDVisible(false);
                return;
            }
            GameUIRoot.Instance.SetPlayerHUDVisible(!IsUIHidden);
        }
    }
}
