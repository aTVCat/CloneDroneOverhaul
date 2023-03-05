using ModLibrary;

namespace CDOverhaul.Gameplay.Combat
{
    public class NewWeaponsRobotExpansion : OverhaulCharacterExpansion
    {
        private OverhaulWeaponModel m_CurrentWeaponModel;
        public OverhaulWeaponModel EquipedOverhaulWeapon => FirstPersonMover.GetEquippedWeaponModel() as OverhaulWeaponModel;

        private FPMoveCommand m_MoveCommand;
        public FPMoveCommand MoveCommand => m_MoveCommand;

        public override void OnPreCommandExecute(FPMoveCommand command)
        {
            m_MoveCommand = command;
            if (m_CurrentWeaponModel != null && !m_CurrentWeaponModel.AllowRobotToSwitchWeapons)
            {
                command.Input.Weapon1 = false;
                command.Input.Weapon2 = false;
                command.Input.Weapon3 = false;
                command.Input.Weapon4 = false;
                command.Input.Weapon5 = false;
                command.Input.NextWeapon = false;
            }
        }

        public override void OnPostCommandExecute(FPMoveCommand command)
        {
            OverhaulWeaponModel newModel = EquipedOverhaulWeapon;
            if (m_CurrentWeaponModel != newModel)
            {
                if(m_CurrentWeaponModel != null) m_CurrentWeaponModel.OnUnequipped();
                if (newModel != null) newModel.OnEquipped();
            }
            m_CurrentWeaponModel = newModel;

            if (m_CurrentWeaponModel == null)
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