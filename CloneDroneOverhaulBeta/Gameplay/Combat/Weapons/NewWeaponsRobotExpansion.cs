using ModLibrary;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class NewWeaponsRobotExpansion : OverhaulCharacterExpansion
    {
        private OverhaulWeaponModel m_CurrentWeaponModel;

        private FPMoveCommand m_MoveCommand;
        public FPMoveCommand MoveCommand => m_MoveCommand;

        public override void OnPreCommandExecute(FPMoveCommand command)
        {
            m_MoveCommand = command;
            m_CurrentWeaponModel = null;
            WeaponModel model = FirstPersonMover.GetEquippedWeaponModel();
            if (model != null)
            {
                m_CurrentWeaponModel = model as OverhaulWeaponModel;
                if (m_CurrentWeaponModel != null && !m_CurrentWeaponModel.AllowChangingWeapon)
                {
                    command.Input.Weapon1 = false;
                    command.Input.Weapon2 = false;
                    command.Input.Weapon3 = false;
                    command.Input.Weapon4 = false;
                    command.Input.Weapon5 = false;
                    command.Input.NextWeapon = false;
                }
            }
        }

        public override void OnPostCommandExecute(FPMoveCommand command)
        {
            if(m_CurrentWeaponModel == null)
            {
                return;
            }

            if (command.Input.AttackKeyDown)
            {
                m_CurrentWeaponModel.TryAttack();
            }
        }
    }
}