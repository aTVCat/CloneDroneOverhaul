using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class RobotControlsExpansion : OverhaulCharacterExpansion
    {
        private const float WEAPON_SWITCH_INTERVAL = 0.15f;

        private float m_TimeToAllowSwitchingWeapons;

        private void Update()
        {
            if (!Owner || Cursor.visible || !Owner.IsPlayerInputEnabled() || !Owner.IsMainPlayer() || !Owner.IsAlive())
                return;

            float unscaledTime = Time.unscaledTime;
            bool allowScrolling = !Owner.IsAimingBow() && unscaledTime >= m_TimeToAllowSwitchingWeapons;
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
