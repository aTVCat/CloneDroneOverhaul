using ModLibrary;
using OverhaulAPI;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class NewWeaponsRobotExpansion : OverhaulCharacterExpansion
    {
        private OverhaulWeaponModel m_CurrentWeaponModel;
        public OverhaulWeaponModel EquipedOverhaulWeapon => FirstPersonMover.GetEquippedWeaponModel() as OverhaulWeaponModel;

        private FPMoveCommand m_MoveCommand;
        public FPMoveCommand MoveCommand => m_MoveCommand;

        internal List<AddedWeaponModel> AllCustomWeapons;

        private float m_TimeToRefreshWeapons = -1f;

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

        protected override void OnRefresh()
        {
            m_TimeToRefreshWeapons = Time.unscaledTime + 0.2f;
        }

        public override void Start()
        {
            base.Start();
            OnRefresh();
        }

        private void Update()
        {
            if(m_TimeToRefreshWeapons != -1f && Time.unscaledTime >= m_TimeToRefreshWeapons)
            {
                refreshWeapons();
                m_TimeToRefreshWeapons = -1f;
            }
        }

        private void refreshWeapons()
        {
            UpgradeCollection collection = UpgradeCollection;
            if(collection == null)
            {
                return;
            }
            foreach (AddedWeaponModel m in AllCustomWeapons)
            {
                if (m != null && m is OverhaulWeaponModel)
                {
                    (m as OverhaulWeaponModel).OnUpgradesRefreshed(collection);
                }
            }
        }
    }
}