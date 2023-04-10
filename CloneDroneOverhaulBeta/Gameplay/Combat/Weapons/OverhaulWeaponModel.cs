using ModLibrary;
using OverhaulAPI;
using System.Collections.Generic;

namespace CDOverhaul.Gameplay.Combat
{
    public class OverhaulWeaponModel : AddedWeaponModel
    {
        private CharacterModdedAnimationsExpansion m_CharacterModdedAnimationsExpansion;
        public CharacterModdedAnimationsExpansion AnimationController
        {
            get
            {
                FirstPersonMover mover = GetOwner();
                if (mover == null)
                {
                    return null;
                }

                if (m_CharacterModdedAnimationsExpansion == null)
                {
                    m_CharacterModdedAnimationsExpansion = mover.GetComponent<CharacterModdedAnimationsExpansion>();
                }
                return m_CharacterModdedAnimationsExpansion;
            }
        }

        private NewWeaponsRobotExpansion m_NewWeaponsRobotExpansion;
        public NewWeaponsRobotExpansion NewWeaponsController
        {
            get
            {
                FirstPersonMover mover = GetOwner();
                if (mover == null)
                {
                    return null;
                }

                if (m_NewWeaponsRobotExpansion == null)
                {
                    m_NewWeaponsRobotExpansion = mover.GetComponent<NewWeaponsRobotExpansion>();
                }
                return m_NewWeaponsRobotExpansion;
            }
        }

        public bool AllowSwitchingWeapons { get; set; }

        public void SetCanBeEquiped()
        {
            List<WeaponType> list = GetOwner().GetPrivateField<List<WeaponType>>("_equippedWeapons");
            if (!list.Contains(base.WeaponType))
            {
                list.Add(base.WeaponType);
            }
        }

        public void SetCannotBeEquiped(bool setIsDropped = false)
        {
            List<WeaponType> list = GetOwner().GetPrivateField<List<WeaponType>>("_equippedWeapons");
            if (list.Contains(base.WeaponType))
            {
                _ = list.Remove(base.WeaponType);
            }

            List<WeaponType> list2 = GetOwner().GetPrivateField<List<WeaponType>>("_droppedWeapons");
            if (!list2.Contains(base.WeaponType))
            {
                list2.Add(base.WeaponType);
            }
        }

        public virtual void Start()
        {
            AllowSwitchingWeapons = true;
        }

        public virtual void TryAttack()
        {

        }

        public virtual void OnEquipped()
        {

        }

        public virtual void OnUnequipped()
        {

        }

        public virtual void OnUpgradesRefreshed(UpgradeCollection collection)
        {

        }
    }
}