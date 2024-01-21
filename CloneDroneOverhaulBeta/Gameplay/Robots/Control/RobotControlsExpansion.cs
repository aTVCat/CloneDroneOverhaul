using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class RobotControlsExpansion : OverhaulCharacterExpansion
    {
        [OverhaulSetting("Gameplay.Control.Switch weapons with mouse wheel", false)]
        public static bool EnableMouseWheel;

        private const float WEAPON_SWITCH_INTERVAL = 0.15f;

        private float m_TimeToAllowSwitchingWeapons;

        private void Update()
        {
            if (!Owner || Cursor.visible || !Owner.IsPlayerInputEnabled() || !Owner.IsMainPlayer() || !Owner.IsAlive())
                return;

            float unscaledTime = Time.unscaledTime;
            bool allowScrolling = EnableMouseWheel && !Owner.IsAimingBow() && unscaledTime >= m_TimeToAllowSwitchingWeapons;
            if (allowScrolling)
            {
                float scroll = Input.mouseScrollDelta.y;
                if (scroll > 0.1f)
                {
                    Owner.GoToNextWeapon();
                    m_TimeToAllowSwitchingWeapons = unscaledTime + WEAPON_SWITCH_INTERVAL;
                }
                else if (scroll < -0.1f)
                {
                    Owner.GoToPreviousWeapon();
                    m_TimeToAllowSwitchingWeapons = unscaledTime + WEAPON_SWITCH_INTERVAL;
                }
            }

            if (GameModeManager.IsSinglePlayer() && Input.GetKeyDown(KeyCode.Alpha0))
            {
                Owner.SetEquippedWeaponType(WeaponType.None, true);
            }
        }
    }
}
